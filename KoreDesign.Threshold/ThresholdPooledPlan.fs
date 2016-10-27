namespace KoreDesign.Threshold
open System
open ThresholdPooledPlanTypes
module ThresholdPooledPlan =
    
    let daysInMonth = 30

    let getPooledPlanDailyCommmitment (setting:PooledPlanThresholdSettings<'u>) =
        setting.Commitment / (int64)daysInMonth * (int64)setting.DeviceCount 

    let getPooledPlanMonthlyCommmitment (setting:PooledPlanThresholdSettings<'u>) =
        setting.Commitment / (int64)daysInMonth * (int64)setting.BillableDays

    let pooledPlanThresholdViolated (setting:PooledPlanThresholdSettings<'u>) interval thresholdType usage =
        match (interval,thresholdType) with
        |Daily,Violation -> usage > (getPooledPlanDailyCommmitment setting) %% setting.DailyThreshold
        |Monthly,Violation -> usage > (getPooledPlanMonthlyCommmitment setting) %% setting.MonthlyThreshold
        |Daily,Warning -> usage > (getPooledPlanDailyCommmitment setting) %% setting.DailyThreshold %% setting.ThresholdWarning
        |Monthly,Warning -> usage > (getPooledPlanMonthlyCommmitment setting) %% setting.MonthlyThreshold %% setting.ThresholdWarning


    //sprPoolThresholdDailyUpdateUsage
    let rec poolThresholdDailyUpdateUsage (pptu:PooledPlanThresholdUsage<'u>) (dailyPoolSIMs:DailyPooledPlanThresholdUsageBySim<'u> list) (monitors:ThresholdTypes.ThresholdMonitor<'u> list) today =
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