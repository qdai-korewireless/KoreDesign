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


    let isUsageExceededDailyThreshold thresholdMonitor usage = 
        let dailyThreshold = thresholdMonitor.PerDeviceThresholdSettings.DailyThreshold
        let currentUsage = thresholdMonitor.UsageTotal
        currentUsage + usage > dailyThreshold

    let monitorUsage (monitor:ThresholdMonitor<'u>) (usage:Usage<'u>) = 
        {monitor with UsageTotal = usage.Usage + monitor.UsageTotal; IsThresholdExceeded = (isUsageExceededDailyThreshold monitor usage.Usage)}
                 

