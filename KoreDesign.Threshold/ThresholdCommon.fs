namespace KoreDesign.Threshold
open System
open LanguagePrimitives

[<AutoOpen>]
module ThresholdCommon =
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

    let (%%) (threshold:int64<'u>) (warning:float32) =
        Int64WithMeasure ((int64)((float32)threshold * warning))