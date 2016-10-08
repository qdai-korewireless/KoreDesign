namespace KoreDesign.Threshold
open System
module Threshold =
    let daysInMonth = 30
    let (%%) (threshold:int64<'u>) warning =
        (int64)((float32)threshold * warning)

    let perDeviceThresholdViolated (setting:PerDeviceThresholdSettings) interval usageType thresholdType usage =
        match (interval,usageType,thresholdType) with
        |Daily,Data,Violation -> usage * b > setting.DailyDataThreshold
        |Daily,SMS,Violation -> usage * msg > setting.DailySMSThreshold
        |Monthly,Data,Violation -> usage * b > setting.MonthlyDataThreshold
        |Monthly,SMS,Violation -> usage * msg > setting.MonthlySMSThreshold
        |Daily,Data,Warning -> usage > setting.DailyDataThreshold %% setting.ThresholdWarning
        |Daily,SMS,Warning -> usage  > setting.DailySMSThreshold %% setting.ThresholdWarning
        |Monthly,Data,Warning -> usage > setting.MonthlyDataThreshold %% setting.ThresholdWarning
        |Monthly,SMS,Warning -> usage > setting.MonthlySMSThreshold %% setting.ThresholdWarning

    let getPooledPlanDailyCommmitment (setting:PooledPlanThresholdSettings) usageType =
        let commitment = match usageType with
                            |Data -> setting.DataCommitment 
                            |SMS -> setting.SMSCommitment 
        commitment / (int64)daysInMonth * (int64)setting.DeviceCount 

    let getPooledPlanMonthlyCommmitment (setting:PooledPlanThresholdSettings) usageType =
        let commitment = match usageType with
                            |Data -> setting.DataCommitment 
                            |SMS -> setting.SMSCommitment 
        commitment / (int64)daysInMonth * (int64)setting.BillableDays

    let pooledPlanThresholdViolated (setting:PooledPlanThresholdSettings) interval usageType thresholdType usage =
        match (interval,usageType,thresholdType) with
        |Daily,Data,Violation -> usage > (getPooledPlanDailyCommmitment setting Data) %% setting.DailyDataThreshold
        |Daily,SMS,Violation -> usage > (getPooledPlanDailyCommmitment setting SMS) %% setting.DailySMSThreshold
        |Monthly,Data,Violation -> usage > (getPooledPlanMonthlyCommmitment setting Data) %% setting.MonthlyDataThreshold
        |Monthly,SMS,Violation -> usage > (getPooledPlanMonthlyCommmitment setting SMS) %% setting.MonthlySMSThreshold
        |Daily,Data,Warning -> usage > (getPooledPlanDailyCommmitment setting Data) %% setting.DailyDataThreshold %% setting.ThresholdWarning
        |Daily,SMS,Warning -> usage  > (getPooledPlanDailyCommmitment setting SMS) %% setting.DailySMSThreshold %% setting.ThresholdWarning
        |Monthly,Data,Warning -> usage > (getPooledPlanMonthlyCommmitment setting Data) %% setting.MonthlyDataThreshold %% setting.ThresholdWarning
        |Monthly,SMS,Warning -> usage > (getPooledPlanMonthlyCommmitment setting SMS) %% setting.MonthlySMSThreshold %% setting.ThresholdWarning


    let monitorUsage (monitor:ThresholdMonitor) (usage:Usage) = 
        match usage.Usage with
            |DataUsage u -> 
                {monitor with DataTotal = u + monitor.DataTotal}
            |SMSUsage u -> 
                {monitor with SMSTotal = u + monitor.SMSTotal}
                 

