namespace KoreDesign.Threshold
open System
open LanguagePrimitives

module Threshold =
    let daysInMonth = 30
    let (%%) (threshold:int64<'u>) (warning:float32) =
        Int64WithMeasure ((int64)((float32)threshold * warning))

    let perDeviceThresholdViolated (setting:PerDeviceThresholdSettings<'u>) interval thresholdType (usage:int64<'u>) =
        match (interval,thresholdType) with
        |Daily,Violation -> usage > setting.DailyThreshold
        |Monthly,Violation -> usage > setting.MonthlyThreshold
        |Daily,Warning -> usage > setting.DailyThreshold %% setting.ThresholdWarning
        |Monthly,Warning -> usage > setting.MonthlyThreshold %% setting.ThresholdWarning

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


    let getExceededThresholdType (setting:PerDeviceThresholdSettings<'u>) (currUsage:int64<'u>) (newUsage:int64<'u>) = 
        if currUsage + newUsage > setting.DailyThreshold then
            Some Violation
        else if currUsage + newUsage > setting.DailyThreshold %% setting.ThresholdWarning then
            Some Warning
        else
            None

    let monitorUsage (monitors:ThresholdMonitor<'u> list) (usage:Usage<'u>) = 
        let perDeviceThresholdSettings:PerDeviceThresholdSettings<'u> = {
                                                                        DailyThreshold = Int64WithMeasure 0L; 
                                                                        MonthlyThreshold= Int64WithMeasure 0L;
                                                                        ThresholdWarning = 0.5f;
                                                                        NotificationEmail = "";
                                                                        NotificationSMS = "";
                                                                    }
        
        let monitor = monitors |> Seq.tryFind (fun m -> m.SIMID = usage.SIMID && m.UsageDate = usage.UsageDate)
        
        match monitor with
        |Some m -> 
            let remain = monitors |> Seq.filter (fun m -> m.SIMID <> usage.SIMID) |> Seq.toList
            {
                m with UsageTotal = usage.Usage + m.UsageTotal; 
                            ExceededThresholdType = (getExceededThresholdType m.PerDeviceThresholdSettings m.UsageTotal usage.Usage);
            }::remain
        |None -> 
                let newMonitor = {
                    UsageDate = usage.UsageDate;
                    SIMID = usage.SIMID;
                    UsageTotal = usage.Usage;
                    BillingStartDate = usage.BillingStartDate;
                    PerDeviceThresholdSettings = perDeviceThresholdSettings;
                    ExceededThresholdType = (getExceededThresholdType perDeviceThresholdSettings (Int64WithMeasure 0L) usage.Usage);
                    RunningTotal = Int64WithMeasure 0L;
                    EnterpriseID = -1;
                    SIMTypeID = SIMTypes.Proximus;
                    DailyAlert = None
                }
                newMonitor::monitors

    let calculateRunningTotals (monitors:ThresholdMonitor<'u> list) =
        let updated = seq {for m in monitors ->
                            let filtered = monitors |> Seq.filter (fun i -> i.UsageDate < m.UsageDate && i.SIMID = m.SIMID && i.BillingStartDate = m.BillingStartDate)
                            let total = filtered |> Seq.sumBy(fun i-> i.UsageTotal)
                            {m with RunningTotal = m.UsageTotal + total}}
        updated

    let addUsageDate (tdates:ThresholdDate list) (monitors:ThresholdMonitor<'u> list) =
        let u1 = set tdates
        let u2= set (monitors |> Seq.map (fun m -> {EnterpriseID = m.EnterpriseID;SIMTypeID=m.SIMTypeID;UsageDate = m.UsageDate}))
        Set.ofSeq (u1 + u2)

    let updateAlert (thresholdType:ThresholdType) (alerts:DailyAlert<'u> list) (monitors:ThresholdMonitor<'u> list) = 
        let one:int<'u> = Int32WithMeasure 1
        let max = match alerts with 
                    |[] -> match thresholdType with | ThresholdType.Violation -> 8000001 |ThresholdType.Warning -> 9000001
                    | _ ->(alerts |> Seq.maxBy (fun a-> a.AlertID)).AlertID
        let newAlerts = seq {for m in (monitors |> Seq.filter (fun m -> m.ExceededThresholdType = Some thresholdType))->
                                let find = alerts |> Seq.tryFind (fun a -> a.AlertDate = m.UsageDate && a.SIMTypeID = m.SIMTypeID && Some a.ThresholdType = m.ExceededThresholdType)
                                match find with
                                |Some a -> {a with NumOfIncidents = a.NumOfIncidents + one}
                                |None -> {AlertDate = m.UsageDate;EnterpriseID = m.EnterpriseID; ThresholdType = thresholdType; SIMTypeID = m.SIMTypeID ;AlertID = max+1;NumOfIncidents = one}
                                }
        newAlerts