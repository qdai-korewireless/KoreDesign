namespace KoreDesign.Threshold

module Threshold =

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

    let pooledPlanThresholdViolated (setting:PooledPlanThresholdSettings) interval usageType thresholdType usage =
        match (interval,usageType,thresholdType) with
        |Daily,Data,Violation -> usage > usage %% setting.DailyDataThreshold
        |Daily,SMS,Violation -> usage > usage %% setting.DailySMSThreshold
        |Monthly,Data,Violation -> usage > usage %% setting.MonthlyDataThreshold
        |Monthly,SMS,Violation -> usage > usage %% setting.MonthlySMSThreshold
        |Daily,Data,Warning -> usage > usage %% setting.DailyDataThreshold %% setting.ThresholdWarning
        |Daily,SMS,Warning -> usage  > usage %% setting.DailySMSThreshold %% setting.ThresholdWarning
        |Monthly,Data,Warning -> usage > usage %% setting.MonthlyDataThreshold %% setting.ThresholdWarning
        |Monthly,SMS,Warning -> usage > usage %% setting.MonthlySMSThreshold %% setting.ThresholdWarning