namespace KoreDesign.Threshold

[<AutoOpen>]
module ThresholdTypes =

    type ThresholdInterval =
        |Daily
        |Monthly

    type UsageType =
        |Data
        |SMS
    type ThresholdType =
        |Violation
        |Warning


    [<Measure>] type pct
    [<Measure>] type b
    [<Measure>] type kb
    [<Measure>] type mb
    [<Measure>] type gb
    [<Measure>] type tb
    [<Measure>] type msg

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
        Commitment: int64<b>;
        DailyDataThreshold:float32;
        DailySMSThreshold:float32;
        MonthlyDataThreshold:float32;
        MonthlySMSThreshold:float32;
        ThresholdWarning:float32;
        NotificationEmail:string;
        NotificationSMS:string
        }