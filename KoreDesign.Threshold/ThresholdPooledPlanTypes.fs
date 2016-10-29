namespace KoreDesign.Threshold
open System

module ThresholdPooledPlanTypes =

    type PooledPlanThresholdSettings<[<Measure>]'u> = {
        DailyThreshold:float32;
        MonthlyThreshold:float32;
        ThresholdWarning:float32;
        }

    type PooledPlanThresholdUsage<[<Measure>]'u> = {
        MonthlyCommitment:int64<'u>
        MonthlyUsage:int64<'u>
        MonthlyThreshold:int64<'u>
        DailyCommitment:int64<'u>
        DailyUsage:int64<'u>
        DailyThreshold:int64<'u>
        DeviceCount:int
        BillingStartDate:DateTime
        EnterpriseID:int
        SIMType:SIMTypes
        PerDeviceCommitment:int64<'u>
        TotalBillableDays:int
        PooledPlanThresholdSettings:PooledPlanThresholdSettings<'u>
        PoolLevelID:int
    }

    type PooledPlanAlert<[<Measure>]'u> = {
        AlertID:int
        AlertDate:DateTime
        ThresholdInterval:ThresholdInterval
        ThresholdType:ThresholdType
        PooledPlanThresholdUsage:PooledPlanThresholdUsage<'u>
        AlertsToSend:int
    }
    type PooledPlanThresholdMonitor<[<Measure>]'u> = {
        UsageDate:DateTime;
        SIMID:int;
        UsageTotal:int64<'u>;
        BillingStartDate:DateTime;
        PooledPlanThresholdUsage:PooledPlanThresholdUsage<'u>
    }

    type DailyPooledPlanThresholdUsageBySim<[<Measure>]'u> ={
        SIMID:int
        DailyUsage:int64<'u>
        UsageDate:DateTime
        CreatedDate:DateTime
        PooledPlanThresholdUsage:PooledPlanThresholdUsage<'u>
    }

    type MonthlyPooledPlanThresholdUsageBySim<[<Measure>]'u> ={
        SIMID:int
        MonthlyUsage:int64<'u>
        BillingDays:int
        PooledPlanThresholdUsage:PooledPlanThresholdUsage<'u>
    }