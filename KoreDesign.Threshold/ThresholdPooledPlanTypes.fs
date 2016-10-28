namespace KoreDesign.Threshold
open System

module ThresholdPooledPlanTypes =

    type PooledPlanThresholdSettings<[<Measure>]'u> = {
        DailyThreshold:float32;
        MonthlyThreshold:float32;
        ThresholdWarning:float32;
        }

    type PooledPlanAlert = {
        AlertID:int
        EnterpriseID:int
        BillingStartDate:DateTime
        AlertDate:DateTime
        ThresholdInterval:ThresholdInterval
        ThresholdType:ThresholdType
    }
    type PooledPlanThresholdMonitor<[<Measure>]'u> = {
        UsageDate:DateTime;
        SIMID:int;
        UsageTotal:int64<'u>;
        BillingStartDate:DateTime;
        PooledPlanThresholdSettings:PooledPlanThresholdSettings<'u>;
        ExceededThresholdType:ThresholdType option;
        DailyAlert:PooledPlanAlert option;
        EnterpriseID:int;
        SIMType:SIMTypes;
    }

    type PooledPlanThresholdUsage<[<Measure>]'u> = {
        MonthlyCommitment:int64<'u>
        MonthlyUsage:int64<'u>
        DailyCommitment:int64<'u>
        DailyUsage:int64<'u>
        DeviceCount:int
        BillingStartDate:DateTime
        EnterpriseID:int
        SIMType:SIMTypes
        PerDeviceCommitment:int64<'u>
        TotalBillableDays:int
        PooledPlanThresholdSettings:PooledPlanThresholdSettings<'u>
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