﻿namespace KoreDesign.Threshold
open System
open ThresholdPooledPlanTypes
module ThresholdPooledPlan =

    let getPooledPlanDailyCommmitment (daysInMonth:int) (commitment:int64<'u>) (deviceCount:int) =
        commitment / (int64)daysInMonth * (int64)deviceCount 

    let getPooledPlanMonthlyCommmitment (daysInMonth:int) (commitment:int64<'u>) (billableDays:int)=
        commitment / (int64)daysInMonth * (int64)billableDays

    let pooledPlanThresholdViolated (setting:PooledPlanThresholdSettings<'u>) interval thresholdType usage daysInMonth commitment deviceCount billableDays=
        match (interval,thresholdType) with
        |Daily,Violation -> usage > (getPooledPlanDailyCommmitment daysInMonth commitment deviceCount ) %% setting.DailyThreshold
        |Monthly,Violation -> usage > (getPooledPlanMonthlyCommmitment daysInMonth commitment billableDays) %% setting.MonthlyThreshold
        |Daily,Warning -> usage > (getPooledPlanDailyCommmitment daysInMonth commitment deviceCount ) %% setting.DailyThreshold %% setting.ThresholdWarning
        |Monthly,Warning -> usage > (getPooledPlanMonthlyCommmitment daysInMonth commitment billableDays) %% setting.MonthlyThreshold %% setting.ThresholdWarning

    let getExceededThresholdType (pptu:PooledPlanThresholdUsage<'u>) (interval:ThresholdInterval)  (usage:int64<'u>) = 
        let threshold = match interval with
                        |Daily -> pptu.DailyThreshold
                        |Monthly -> pptu.MonthlyThreshold

        if usage > threshold then
            Some Violation
        else if usage > threshold %% pptu.PooledPlanThresholdSettings.ThresholdWarning then
            Some Warning
        else
            None

    //sprPoolThresholdDailyUpdateUsage
    let rec poolThresholdDailyUpdateUsage today (dailyPoolSIMs:DailyPooledPlanThresholdUsageBySim<'u> list) (monitors:PooledPlanThresholdMonitor<'u> list) =
        match monitors with
        |m::rem_monitors -> 
            let poolSIM = dailyPoolSIMs |> Seq.tryFind (fun p -> m.UsageDate = p.UsageDate && m.SIMID = p.SIMID && m.PooledPlanThresholdUsage.PoolLevelID = p.PooledPlanThresholdUsage.PoolLevelID)

            match poolSIM with
            |Some p -> 
                let rem_poolSIMs = dailyPoolSIMs |> Seq.filter (fun i -> i <> p) |>Seq.toList
                let newPoolSIM:DailyPooledPlanThresholdUsageBySim<'u> = {p with DailyUsage = p.DailyUsage + m.UsageTotal}
                poolThresholdDailyUpdateUsage today (newPoolSIM::rem_poolSIMs) rem_monitors 
            |None -> 
                let newPoolSIM:DailyPooledPlanThresholdUsageBySim<'u> = {UsageDate = m.UsageDate; SIMID = m.SIMID;DailyUsage=m.UsageTotal;CreatedDate=today;PooledPlanThresholdUsage = m.PooledPlanThresholdUsage}
                poolThresholdDailyUpdateUsage today (newPoolSIM::dailyPoolSIMs) rem_monitors 
        |[] -> dailyPoolSIMs

    //sprPoolThresholdMonthlyUpdateUsage
    let rec poolThresholdMonthlyUpdateUsage (monthlyPoolSIMs:MonthlyPooledPlanThresholdUsageBySim<'u> list) (dailyPoolSIMs:DailyPooledPlanThresholdUsageBySim<'u> list) =
        match dailyPoolSIMs with
        |ds::rem_poolSIMs -> 
            let poolSIM = monthlyPoolSIMs |> Seq.find (fun ms -> ms.PooledPlanThresholdUsage = ds.PooledPlanThresholdUsage && ms.SIMID = ds.SIMID)

            let rem_mpoolSIMs = monthlyPoolSIMs |> Seq.filter (fun i -> i <> poolSIM) |>Seq.toList
            let newMPoolSIM = {poolSIM with MonthlyUsage = poolSIM.MonthlyUsage + ds.DailyUsage}
            poolThresholdMonthlyUpdateUsage (newMPoolSIM::rem_mpoolSIMs) rem_poolSIMs

        |[] -> monthlyPoolSIMs

    //update PooledPlanThresholdUsage monthly usage
    let updatePooledPlanThresholdMonthlyUsage (pptus:PooledPlanThresholdUsage<'u> list) (monthlyPoolSIMs:MonthlyPooledPlanThresholdUsageBySim<'u> list) =
        seq{ for pptu in pptus ->
                let sum = monthlyPoolSIMs |> Seq.filter(fun s -> s.PooledPlanThresholdUsage.PoolLevelID = pptu.PoolLevelID) |> Seq.sumBy ( fun s -> s.MonthlyUsage)
                {pptu with MonthlyUsage = pptu.MonthlyUsage + sum}
            }

     //insert daily alert
    let rec insertPooledPlanDailyAlerts today (alerts:PooledPlanAlert<'u> list) (dailyPoolSIMs:DailyPooledPlanThresholdUsageBySim<'u> list) (monitors:PooledPlanThresholdMonitor<'u> list) =
        match monitors with
        |m::rem_monitors -> 
            let poolSIMs = dailyPoolSIMs |> Seq.filter (fun p -> p.CreatedDate = today && m.PooledPlanThresholdUsage.PoolLevelID = p.PooledPlanThresholdUsage.PoolLevelID)
            let poolDailyUsage = poolSIMs |> Seq.sumBy (fun s -> s.DailyUsage)
            let thre = getExceededThresholdType m.PooledPlanThresholdUsage Daily poolDailyUsage
            match thre with
            |Some t->
                let alert = alerts|> Seq.tryFind (fun a -> a.AlertDate = today && a.PooledPlanThresholdUsage.PoolLevelID = m.PooledPlanThresholdUsage.PoolLevelID && a.ThresholdInterval = Daily)
                match alert with
                |Some a -> 
                    insertPooledPlanDailyAlerts today alerts dailyPoolSIMs rem_monitors 
                |None -> 
                    let nextID = match alerts with
                                    |[]->1
                                    |_->
                                        let todayAlert = alerts|> Seq.tryFind (fun a -> a.AlertDate = today && a.ThresholdInterval = Daily && a.PooledPlanThresholdUsage.EnterpriseID = m.PooledPlanThresholdUsage.EnterpriseID) 
                                        match todayAlert with
                                        |Some a -> a.AlertID
                                        |None -> (alerts|> Seq.maxBy (fun a -> a.AlertID)).AlertID + 1
                                        
                                    
                    let newDailyAlert:PooledPlanAlert<'u> = {AlertID = nextID; AlertDate=today;ThresholdInterval = Daily;ThresholdType=t;PooledPlanThresholdUsage=m.PooledPlanThresholdUsage;AlertsToSend=1}
                    insertPooledPlanDailyAlerts today (newDailyAlert::alerts) dailyPoolSIMs rem_monitors 
            |None ->
                insertPooledPlanDailyAlerts today alerts dailyPoolSIMs rem_monitors 
        |[] -> alerts

     //insert monthly alert
    let rec insertPooledPlanMonthlyAlerts today (alerts:PooledPlanAlert<'u> list) (monthlyPoolSIMs:MonthlyPooledPlanThresholdUsageBySim<'u> list) (monitors:PooledPlanThresholdMonitor<'u> list) =
        match monitors with
        |m::rem_monitors -> 
            let poolSIMs = monthlyPoolSIMs |> Seq.filter (fun p -> m.PooledPlanThresholdUsage.PoolLevelID = p.PooledPlanThresholdUsage.PoolLevelID)
            let poolMonthlyUsage = poolSIMs |> Seq.sumBy (fun s -> s.MonthlyUsage)
            let thre = getExceededThresholdType m.PooledPlanThresholdUsage Monthly poolMonthlyUsage
            match thre with
            |Some t->
                let alert = alerts|> Seq.tryFind (fun a -> a.PooledPlanThresholdUsage.PoolLevelID = m.PooledPlanThresholdUsage.PoolLevelID && a.ThresholdInterval = Monthly)
                match alert with
                |Some a -> 
                    insertPooledPlanMonthlyAlerts today alerts monthlyPoolSIMs rem_monitors 
                |None -> 
                    let fitleredAlerts = alerts |> Seq.filter (fun a -> a.ThresholdInterval = Monthly && a.ThresholdType = t) |> Seq.toList
                    let nextID = match fitleredAlerts with
                                    |[]-> 1 //todo: change ID to KORE ID formats
                                    |_-> (alerts |> Seq.maxBy (fun a -> a.AlertID)).AlertID + 1
                                    
                    let newMonthlyAlert:PooledPlanAlert<'u> = {AlertID = nextID; AlertDate=today;ThresholdInterval = Monthly;ThresholdType=t;PooledPlanThresholdUsage=m.PooledPlanThresholdUsage;AlertsToSend=1}
                    insertPooledPlanMonthlyAlerts today (newMonthlyAlert::alerts) monthlyPoolSIMs rem_monitors 
            |None ->
                insertPooledPlanMonthlyAlerts today alerts monthlyPoolSIMs rem_monitors 
        |[] -> alerts
