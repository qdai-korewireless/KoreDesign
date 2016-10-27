namespace KoreDesign.Threshold
open System

module ThresholdPooledPlanTypes =

    type PooledPlanThresholdSettings<[<Measure>]'u> = {
        DeviceCount: int;
        BillableDays: int;
        Commitment: int64<'u>;
        DailyThreshold:float32;
        MonthlyThreshold:float32;
        ThresholdWarning:float32;
        NotificationEmail:string;
        NotificationSMS:string
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