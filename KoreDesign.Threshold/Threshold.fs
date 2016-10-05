namespace KoreDesign.Threshold

module Threshold =
    
    let perDeviceThresholdViolated setting interval usageType usage =
        match (interval,usageType) with
        |Daily,Data -> usage * b > setting.DailyDataThreshold
        |Daily,SMS -> usage * msg > setting.DailySMSThreshold
        |Monthly,Data -> usage * b > setting.MonthlyDataThreshold
        |Monthly,SMS -> usage * msg > setting.MonthlySMSThreshold