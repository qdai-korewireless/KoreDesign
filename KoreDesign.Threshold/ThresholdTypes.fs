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
        SIMTypeID:SIMTypes;
        AlertID:int;
        NumOfIncidents:int<'u>;
        AlertDate:DateTime;
        ThresholdType:ThresholdType
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
        SIMTypeID:SIMTypes;
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
        SIMTypeID:SIMTypes;
        UsageDate:DateTime
    }

    type MonitorUsage<[<Measure>]'u> = ThresholdMonitor<'u> list -> Usage<'u> -> ThresholdMonitor<'u>