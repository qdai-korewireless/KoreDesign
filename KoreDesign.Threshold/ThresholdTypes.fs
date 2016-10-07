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
        |Data of int64<b>
        |SMS of int64<msg>

    type ThresholdType =
        |Violation
        |Warning




    let b = 1L<b>
    let msg = 1L<msg>

    let b_kb = 1<b>/1024<kb>
    let kb_mb = 1<kb>/1024<mb>
    let mb_gb = 1<mb>/1024<gb>
    let gb_tb = 1<gb>/1024<tb>

    type PerDeviceThresholdSettings = {
        DailyDataThreshold:int64<b>; 
        DailySMSThreshold:int64<msg>; 
        MonthlyDataThreshold:int64<b>;
        MonthlySMSThreshold:int64<msg>;
        ThresholdWarning:float32;
        NotificationEmail:string;
        NotificationSMS:string
        }

    type PooledPlanThresholdSettings = {
        DeviceCount: int;
        BillableDays: int;
        DataCommitment: int64<b>;
        SMSCommitment: int64<b>;
        DailyDataThreshold:float32;
        DailySMSThreshold:float32;
        MonthlyDataThreshold:float32;
        MonthlySMSThreshold:float32;
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

    type ThresholdMonitor = {
        UsageDate:DateTime;
        SIMID:int;
        DataTotal:int64<b>;
        SMSTotal:int64<msg>;
        DataAlert:DailyAlert option;
        SMSAlert:DailyAlert option;
        BillingStartDate:DateTime
    }
    
    type Usage = {
        MSISDN:int64;
        IMSI:int;
        UsageDate:DateTime;
        Usage:UsageType
        PLMN:string
    }

    type MonitorUsage = Usage -> ThresholdMonitor -> ThresholdMonitor