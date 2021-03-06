﻿namespace KoreDesign.Threshold
open System
open LanguagePrimitives
open ThresholdTypes

module Threshold =

    (*************** Threshold Service - Core Threshold Calculation ***************)

    



    let perDeviceThresholdViolated (setting:PerDeviceThresholdSettings<'u>) interval thresholdType (usage:int64<'u>) =
        match (interval,thresholdType) with
        |Daily,Violation -> usage > setting.DailyThreshold
        |Monthly,Violation -> usage > setting.MonthlyThreshold
        |Daily,Warning -> usage > setting.DailyThreshold %% setting.ThresholdWarning
        |Monthly,Warning -> usage > setting.MonthlyThreshold %% setting.ThresholdWarning


    (*************** Threshold Service - ThresholdApplyPending analysis ***************)
    
    let getExceededThresholdType (setting:PerDeviceThresholdSettings<'u>) (interval:ThresholdInterval) (currUsage:int64<'u>) (newUsage:int64<'u>) = 
        let usage = currUsage + newUsage
        let threshold = match interval with
                        |Daily -> setting.DailyThreshold
                        |Monthly -> setting.MonthlyThreshold

        if currUsage < threshold && usage > threshold then
            Some Violation
        else if currUsage < threshold  %% setting.ThresholdWarning && usage > threshold %% setting.ThresholdWarning then
            Some Warning
        else
            None
    
    //sprThresholdMonitoringUpdateUsage
    let monitorUsage (perDeviceThresholdSettings:PerDeviceThresholdSettings<'u>) (monitors:ThresholdMonitor<'u> list) (usage:Usage<'u>) = 
        let zero = Int32WithMeasure 0
        let zero64 = Int64WithMeasure 0L
        
        let monitor = monitors |> Seq.tryFind (fun m -> m.SIMID = usage.SIMID && m.UsageDate = usage.UsageDate)
        
        match monitor with
        |Some m -> 
            let remain = monitors |> Seq.filter (fun m -> m.SIMID <> usage.SIMID) |> Seq.toList
            {
                m with UsageTotal = usage.Usage + m.UsageTotal; 
                            ExceededThresholdType = (getExceededThresholdType m.PerDeviceThresholdSettings ThresholdInterval.Daily m.UsageTotal usage.Usage);
            }::remain
        |None -> 
                let newMonitor = {
                    UsageDate = usage.UsageDate;
                    SIMID = usage.SIMID;
                    UsageTotal = usage.Usage;
                    BillingStartDate = usage.BillingStartDate;
                    PerDeviceThresholdSettings = perDeviceThresholdSettings;
                    ExceededThresholdType = (getExceededThresholdType perDeviceThresholdSettings ThresholdInterval.Daily zero64 usage.Usage);
                    RunningTotal = Int64WithMeasure 0L;
                    EnterpriseID = -1;
                    SIMType = SIMTypes.Proximus;
                    DailyAlert = None
                }
                newMonitor::monitors
    
    //sprThresholdMonitoringRunningTotalsUpdate
    let calculateRunningTotals (monitors:ThresholdMonitor<'u> list) =
        let updated = seq {for m in monitors ->
                            let filtered = monitors |> Seq.filter (fun i -> i.UsageDate < m.UsageDate && i.SIMID = m.SIMID && i.BillingStartDate = m.BillingStartDate)
                            let total = filtered |> Seq.sumBy(fun i-> i.UsageTotal)
                            {m with RunningTotal = m.UsageTotal + total}} |> Seq.toList
        updated

    //sprThresholdDateInsertNew
    let addUsageDate (tdates:ThresholdDate list) (monitors:ThresholdMonitor<'u> list) =
        let u1 = set tdates
        let u2:Set<ThresholdDate> = set (monitors |> Seq.map (fun m -> {EnterpriseID = m.EnterpriseID;SIMType=m.SIMType;UsageDate = m.UsageDate}))
        Set.toList (u1 + u2)

    //sprThresholdDailyAlertsUpdate & sprThresholdDailyWarningAlertsUpdate
    let rec updateAlert (alerts:DailyAlert<'u> list) (monitors:ThresholdMonitor<'u> list) = 
        let one:int<'u> = Int32WithMeasure 1
        let exceededMonitors = monitors |> Seq.filter (fun m -> m.ExceededThresholdType <> None) |> Seq.toList
        match exceededMonitors with
        |m::rem_monitors -> 
            let alert = alerts |> Seq.tryFind (fun a -> a.AlertDate = m.UsageDate && a.SIMType = m.SIMType && Some a.ThresholdType = m.ExceededThresholdType)
            let max = match alerts with 
                        |[] -> match m.ExceededThresholdType with | Some ThresholdType.Violation -> 8000001 | Some ThresholdType.Warning -> 9000001
                        | _ ->(alerts |> Seq.maxBy (fun a-> a.AlertID)).AlertID
            match alert with
            |Some a -> 
                let rem_alerts = alerts |> Seq.filter (fun i -> i <> a) |>Seq.toList
                let newAlert = {a with NumOfIncidents = a.NumOfIncidents + one}
                updateAlert (newAlert::rem_alerts) rem_monitors
            |None -> 
                let newAlert = {AlertDate = m.UsageDate;EnterpriseID = m.EnterpriseID; ThresholdType = m.ExceededThresholdType.Value; SIMType = m.SIMType ;AlertID = max+1;NumOfIncidents = one}
                updateAlert (newAlert::alerts) rem_monitors
        |[] -> alerts

    //sprThresholdMonitoringUpdateDailyAlertIds
    let updateMonitorAlert (todayDate:DateTime) (alerts:DailyAlert<'u> list) (monitors:ThresholdMonitor<'u> list) = 
        let newMonitors = seq {
                                for m in (monitors |> Seq.filter (fun m -> m.ExceededThresholdType <> None && m.DailyAlert = None)) ->
                                    let alert = alerts |> Seq.tryFind (fun a -> a.EnterpriseID = m.EnterpriseID && Some a.ThresholdType = m.ExceededThresholdType && a.AlertDate = todayDate)
                                    match alert with
                                    |Some a -> {m with DailyAlert = Some a}
                                    |None -> m
                            }
        newMonitors |> Seq.toList

    //sprThresholdSummaryMerge
    let rec updateThresholdSummary (summaries:ThresholdSummary<'u> list) (monitors:ThresholdMonitor<'u> list) =
        let zero = Int32WithMeasure 0
        let zero64 = Int64WithMeasure 0L
        match monitors with
        |m::rem_monitors -> 
            let summary = summaries |> Seq.tryFind (fun s -> m.SIMID = s.SIMID && m.BillingStartDate = s.BillingStartDate && m.EnterpriseID = s.EnterpriseID)
            match summary with
            |Some s -> 
                let rem_summaries = summaries |> Seq.filter (fun i -> i <> s) |>Seq.toList
                let newSummary = {s with MonthTotal = s.MonthTotal + m.UsageTotal;ExceededMonthlyThresholdType = (getExceededThresholdType m.PerDeviceThresholdSettings ThresholdInterval.Monthly s.MonthTotal m.UsageTotal)}
                updateThresholdSummary (newSummary::rem_summaries) rem_monitors
            |None -> 
                let newSummary = {SIMID = m.SIMID; SIMType = m.SIMType;EnterpriseID = m.EnterpriseID; BillingStartDate=m.BillingStartDate;DaysTracked = zero; DaysExceeded = zero; MonthTotal = zero64;ExceededMonthlyThresholdType = (getExceededThresholdType m.PerDeviceThresholdSettings ThresholdInterval.Monthly zero64 m.UsageTotal); MonthlyAlert=None}
                updateThresholdSummary (newSummary::summaries) rem_monitors
        |[] -> summaries

    //sprThresholdSummaryPerDayUpdate
    let rec updateThresholdSummaryPerDay (summaries:ThresholdSummaryPerDay<'u> list) (monitors:ThresholdMonitor<'u> list) =
        match monitors with
        |m::rem_monitors -> 
            let summary = summaries |> Seq.tryFind (fun s -> m.UsageDate = s.UsageDate && m.BillingStartDate = s.BillingStartDate && m.EnterpriseID = s.EnterpriseID && m.SIMType = s.SIMType)
            let runningTotal = summaries |> Seq.filter (fun os -> os.UsageDate<=m.UsageDate && os.SIMType = m.SIMType && os.EnterpriseID = m.EnterpriseID && os.BillingStartDate = m.BillingStartDate)
                                |> Seq.sumBy (fun os -> os.UsageTotal)
            match summary with
            |Some s -> 
                let rem_summaries = summaries |> Seq.filter (fun i -> i <> s) |>Seq.toList
                let newSummary = {s with UsageTotal = s.UsageTotal + m.UsageTotal; RunningTotal = m.UsageTotal + runningTotal}
                updateThresholdSummaryPerDay (newSummary::rem_summaries) rem_monitors
            |None -> 
                let newSummary = {UsageDate = m.UsageDate; SIMType = m.SIMType;EnterpriseID = m.EnterpriseID; BillingStartDate=m.BillingStartDate;UsageTotal = m.UsageTotal;RunningTotal = m.UsageTotal + runningTotal}
                updateThresholdSummaryPerDay (newSummary::summaries) rem_monitors
        |[] -> summaries

    //sprThresholdMonthlyAlertInsertAndUpdate & sprThresholdMonthlyWarningInsertAndUpdate
    let rec updateMonthlyAlert (today:DateTime) (alerts:MonthlyAlert<'u> list) (summaries:ThresholdSummary<'u> list) = 
        let one:int<'u> = Int32WithMeasure 1
        let exceededSummaries = summaries |> Seq.filter (fun m -> m.ExceededMonthlyThresholdType <> None) |> Seq.toList
        match exceededSummaries with
        |m::rem_summaries -> 
            let alert = alerts |> Seq.tryFind (fun a -> a.AlertDate = today && Some a.ThresholdType = m.ExceededMonthlyThresholdType && a.BillingStartDate = m.BillingStartDate && a.EnterpriseID = m.EnterpriseID)
            let max = match alerts with 
                        |[] -> match m.ExceededMonthlyThresholdType with | Some ThresholdType.Violation -> 8010001 | Some ThresholdType.Warning -> 9010001
                        | _ ->(alerts |> Seq.maxBy (fun a-> a.AlertID)).AlertID
            match alert with
            |Some a -> 
                let rem_alerts = alerts |> Seq.filter (fun i -> i <> a) |>Seq.toList
                let newAlert = {a with NumOfSIMs = a.NumOfSIMs + one}
                updateMonthlyAlert today (newAlert::rem_alerts) rem_summaries 
            |None -> 
                let newAlert = {AlertDate = today;EnterpriseID = m.EnterpriseID; ThresholdType = m.ExceededMonthlyThresholdType.Value; AlertID = max+1;NumOfSIMs = one;BillingStartDate = m.BillingStartDate}
                updateMonthlyAlert today (newAlert::alerts) rem_summaries 
        |[] -> alerts
    //sprThresholdMonthlyMonitoringInsert
    let rec insertThresholdMonthlyMonitor (monitors:ThresholdMonthlyMonitor<'u> list) (summaries:ThresholdSummary<'u> list) (alerts:MonthlyAlert<'u> list) (today:DateTime) =
        let one:int<'u> = Int32WithMeasure 1
        let exceededSummaries = summaries |> Seq.filter (fun m -> m.ExceededMonthlyThresholdType <> None) |> Seq.toList
        let todayAlerts = alerts |> Seq.filter (fun a -> a.AlertDate = today)|> Seq.toList
        match todayAlerts with
        |a::rem_alerts -> 
            let sims = exceededSummaries |> Seq.filter (fun s -> Some a.ThresholdType = s.ExceededMonthlyThresholdType && a.BillingStartDate = s.BillingStartDate && a.EnterpriseID = s.EnterpriseID)
            let newMonitors:ThresholdMonthlyMonitor<'u> list = sims |> Seq.map (fun s -> {SIMID = s.SIMID;MonthlyAlert = a;BillingStartDate=s.BillingStartDate}) |> Seq.toList
            insertThresholdMonthlyMonitor (newMonitors @ monitors) summaries rem_alerts today
        |[] -> monitors

    //sprThresholdSummaryUpdateMonthlyAlarmTotals
    let rec updateMonthlyAlertForSummary (summaries:ThresholdSummary<'u> list) (monitors:ThresholdMonthlyMonitor<'u> list) =
        let one:int<'u> = Int32WithMeasure 1
        match monitors with
        |m::rem_monitors -> 
            let summary = summaries |> Seq.find (fun s -> s.SIMID = m.SIMID && s.BillingStartDate = m.BillingStartDate)
            let newSummary = {summary with MonthlyAlert = Some m.MonthlyAlert}
            let rem_summaries = summaries |> Seq.filter (fun s -> s <> summary) |> Seq.toList
            updateMonthlyAlertForSummary (newSummary::rem_summaries) rem_monitors
        |[] -> summaries
