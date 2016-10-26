namespace KoreDesign.Threshold
open System
[<AutoOpen>]
module ThresholdTypes =
    [<Measure>] type data
    [<Measure>] type sms

    type SIMTypes =
    |Proximus
    |Tango

    type ThresholdInterval =
        |Daily
        |Monthly

    type UsageType =
        |Data
        |SMS

    type UsageTypeUsage =
        |DataUsage of int64<data> 
        |SMSUsage of int64<sms> 

    type ThresholdType =
        |Violation
        |Warning

    type PerDeviceThresholdSettings<[<Measure>]'u> = {
        DailyThreshold:int64<'u>; 
        MonthlyThreshold:int64<'u>;
        ThresholdWarning:float32;
        NotificationEmail:string;
        NotificationSMS:string
        }

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

    type DailyAlert<[<Measure>]'u> = {
        EnterpriseID:int;
        SIMType:SIMTypes;
        AlertID:int;
        NumOfIncidents:int<'u>; //There is NumOfSIMs field in proc, but I can't find difference from NumOfIncidents calculation
        AlertDate:DateTime;
        ThresholdType:ThresholdType
    }

    type MonthlyAlert<[<Measure>]'u> = {
        EnterpriseID:int;
        AlertID:int;
        NumOfSIMs:int<'u>; 
        AlertDate:DateTime;
        ThresholdType:ThresholdType;
        BillingStartDate:DateTime
    }

    type ThresholdMonitor<[<Measure>]'u> = {
        UsageDate:DateTime;
        SIMID:int;
        UsageTotal:int64<'u>;
        BillingStartDate:DateTime;
        PerDeviceThresholdSettings:PerDeviceThresholdSettings<'u>;
        ExceededThresholdType:ThresholdType option;
        DailyAlert:DailyAlert<'u> option;
        RunningTotal: int64<'u>;
        EnterpriseID:int;
        SIMType:SIMTypes;
    }
    
    type Usage<[<Measure>]'u> = {
        MSISDN:string;
        IMSI:string;
        UsageDate:DateTime;
        Usage:int64<'u>;
        PLMN:string;
        SIMID: int;
        BillingStartDate: DateTime;
    }

    type ThresholdDate = {
        EnterpriseID:int;
        SIMType:SIMTypes;
        UsageDate:DateTime
    }

    type ThresholdSummary<[<Measure>]'u> = {
        SIMID:int;
        SIMType:SIMTypes;
        EnterpriseID:int;
        BillingStartDate:DateTime;
        DaysTracked:int<'u>;
        DaysExceeded:int<'u>;
        MonthTotal:int64<'u>;
        ExceededMonthlyThresholdType:ThresholdType option
        MonthlyAlert:MonthlyAlert<'u> option
    }

    type ThresholdSummaryPerDay<[<Measure>]'u> = {
        UsageDate:DateTime
        SIMType:SIMTypes
        EnterpriseID:int
        UsageTotal:int64<'u>
        RunningTotal:int64<'u>
        BillingStartDate:DateTime
    }

    type ThresholdMonthlyMonitor<[<Measure>]'u> ={
        SIMID:int
        MonthlyAlert:MonthlyAlert<'u>
        BillingStartDate:DateTime
    }
    type MonitorUsage<[<Measure>]'u> = ThresholdMonitor<'u> list -> Usage<'u> -> ThresholdMonitor<'u>