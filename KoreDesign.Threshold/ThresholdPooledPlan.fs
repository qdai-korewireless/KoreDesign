namespace KoreDesign.Threshold
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


    //sprPoolThresholdDailyUpdateUsage
    let rec poolThresholdDailyUpdateUsage (pptu:PooledPlanThresholdUsage<'u>) (dailyPoolSIMs:DailyPooledPlanThresholdUsageBySim<'u> list) (monitors:PooledPlanThresholdMonitor<'u> list) today =
        match monitors with
        |m::rem_monitors -> 
            let poolSIM = dailyPoolSIMs |> Seq.tryFind (fun p -> m.UsageDate = p.UsageDate && m.SIMID = p.SIMID && m.EnterpriseID = p.PooledPlanThresholdUsage.EnterpriseID && m.SIMType = p.PooledPlanThresholdUsage.SIMType)

            match poolSIM with
            |Some p -> 
                let rem_poolSIMs = dailyPoolSIMs |> Seq.filter (fun i -> i <> p) |>Seq.toList
                let newPoolSIM:DailyPooledPlanThresholdUsageBySim<'u> = {p with DailyUsage = p.DailyUsage + m.UsageTotal}
                poolThresholdDailyUpdateUsage pptu (newPoolSIM::rem_poolSIMs) rem_monitors today
            |None -> 
                let newPoolSIM:DailyPooledPlanThresholdUsageBySim<'u> = {UsageDate = m.UsageDate; SIMID = m.SIMID;DailyUsage=m.UsageTotal;CreatedDate=today;PooledPlanThresholdUsage = pptu}
                poolThresholdDailyUpdateUsage pptu (newPoolSIM::dailyPoolSIMs) rem_monitors today
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

        