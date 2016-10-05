namespace KoreDesign.Threshold

[<AutoOpen>]
module ThresholdTypes =

    type ThresholdInterval =
        |Daily
        |Monthly

    type UsageType =
        |Data
        |SMS


    [<Measure>] type pct
    [<Measure>] type b
    [<Measure>] type kb
    [<Measure>] type mb
    [<Measure>] type gb
    [<Measure>] type tb
    [<Measure>] type msg

    let (%=>) = 1.0<pct>/100.0
    let b = 1L<b>
    let msg = 1L<msg>

    let b_kb = 1<b>/1024<kb>
    let kb_mb = 1<kb>/1024<mb>
    let mb_gb = 1<mb>/1024<gb>
    let gb_tb = 1<gb>/1024<tb>

    type ThresholdSettings = {
        DailyDataThreshold:int64<b>; 
        DailySMSThreshold:int64<msg>; 
        MonthlyDataThreshold:int64<b>;
        MonthlySMSThreshold:int64<msg>;
        DailyDataPoolPlanThreshold:decimal<pct>;
        DailySMSPoolPlanThreshold:decimal<pct>;
        MonthlyDataPoolPlanThreshold:decimal<pct>;
        MonthlySMSPoolPlanThreshold:decimal<pct>;
        ThresholdWarning:decimal<pct>;
        NotificationEmail:string;
        NotificationSMS:string
        }