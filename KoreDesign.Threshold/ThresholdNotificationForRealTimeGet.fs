namespace KoreDesign.Threshold
open System
open ThresholdTypes
open ThresholdPooledPlanTypes

module ThresholdNotificationForRealTimeGet = 

    let rec getPerDeviceNotificationsRec pdsetting usages monitors dailyAlerts summaries monthlyAlerts =
        match usages with
        |usage::rem_usages -> 
            let newMonitors = usage |> Threshold.monitorUsage pdsetting monitors
            let newDailyAlerts = newMonitors
                                |> Threshold.calculateRunningTotals 
                                |> Threshold.updateAlert dailyAlerts
            let newMonthlyAlerts = newMonitors |> Threshold.updateThresholdSummary summaries
            let newSummaries = newMonitors |> Threshold.updateThresholdSummary summaries
            let newMonthlyAlerts = newSummaries|> Threshold.updateMonthlyAlert usage.UsageDate monthlyAlerts
            getPerDeviceNotificationsRec pdsetting rem_usages newMonitors newDailyAlerts newSummaries newMonthlyAlerts
        |[] -> (dailyAlerts,monthlyAlerts)

    let getPerDeviceNotifications pdsetting usages =
        getPerDeviceNotificationsRec pdsetting usages [] [] [] []

    let rec getPooledPlanNotificationsRec monitors dailySIMs monthlySIMs alerts =
        match monitors with
        |monitor::rem_monitors -> 
            let newDailySIMs = [monitor] |> ThresholdPooledPlan.poolThresholdDailyUpdateUsage monitor.UsageDate dailySIMs
            let newMonthlySIMs = newDailySIMs |> ThresholdPooledPlan.poolThresholdMonthlyUpdateUsage monthlySIMs
            let withDailyAlerts = [monitor] |> ThresholdPooledPlan.insertPooledPlanDailyAlerts monitor.UsageDate alerts newDailySIMs
            let withAllAlerts = [monitor] |> ThresholdPooledPlan.insertPooledPlanMonthlyAlerts monitor.UsageDate withDailyAlerts newMonthlySIMs
            getPooledPlanNotificationsRec rem_monitors newDailySIMs newMonthlySIMs withAllAlerts
        |[] -> alerts

    let getPooledPlanNotifications existingMonthlySIMs monitors =
        getPooledPlanNotificationsRec monitors [] existingMonthlySIMs []
