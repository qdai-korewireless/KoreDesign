namespace KoreDesign.Threshold
open System
[<AutoOpen>]
module ThresholdTypes =
    [<Measure>] type pct
    [<Measure>] type b
    [<Measure>] type kb
    [<Measure>] type mb
    [<Measure>] type gb
    [<Measure>] type tb
    [<Measure>] type msg

    type ThresholdInterval =
        |Daily
        |Monthly

    type UsageType =
        |Data
        |SMS

    type UsageTypeUsage =
        |DataUsage of int64<b> 
        |SMSUsage of int64<msg> 

    type ThresholdType =
        |Violation
        |Warning

    let b = 1L<b>
    let msg = 1L<msg>

    let b_kb = 1<b>/1024<kb>
    let kb_mb = 1<kb>/1024<mb>
    let mb_gb = 1<mb>/1024<gb>
    let gb_tb = 1<gb>/1024<tb>


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

    type DailyAlert = {
        EnterpriseID:int;
        SIMTypeID:int;
        AlertID:int;
        NumOfSIMs:int;
        NumOfIncidents:int;
        UsageType:int;
        AlertDate:DateTime;
        ThresholdType:ThresholdType
    }

    type ThresholdMonitor<[<Measure>]'u> = {
        UsageDate:DateTime;
        SIMID:int;
        UsageTotal:int64<'u>;
        Alert:DailyAlert option;
        BillingStartDate:DateTime;
        PerDeviceThresholdSettings:PerDeviceThresholdSettings<'u>
        IsThresholdExceeded:bool;
        UsageType:UsageType
    }
    
    type Usage<[<Measure>]'u> = {
        MSISDN:string;
        IMSI:string;
        UsageDate:DateTime;
        Usage:int64<'u>;
        PLMN:string
    }

    type MonitorUsage<[<Measure>]'u> = Usage<'u> -> ThresholdMonitor<'u> -> ThresholdMonitor<'u>