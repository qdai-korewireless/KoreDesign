namespace KoreDesign.Threshold
open System
open ThresholdTypes
open ThresholdPooledPlanTypes

module ThresholdNotificationForRealTimeGet = 

    let rec getPerDeviceNotificationsRec pdsetting today usages monitors dailyAlerts summaries monthlyAlerts =
        match usages with
        |usage::rem_usages -> 
            let newMonitors = usage |> Threshold.monitorUsage pdsetting monitors
            let newDailyAlerts = newMonitors
                                |> Threshold.calculateRunningTotals 
                                |> Threshold.updateAlert dailyAlerts
            let newMonthlyAlerts = newMonitors |> Threshold.updateThresholdSummary summaries
            let newSummaries = newMonitors |> Threshold.updateThresholdSummary summaries
            let newMonthlyAlerts = newSummaries|> Threshold.updateMonthlyAlert today monthlyAlerts
            getPerDeviceNotificationsRec pdsetting today rem_usages newMonitors newDailyAlerts newSummaries newMonthlyAlerts
        |[] -> (dailyAlerts,monthlyAlerts)

    let getPerDeviceNotifications pdsetting today usages =
        getPerDeviceNotificationsRec pdsetting today usages [] [] [] []

    let rec getPooledPlanNotificationsRec today monitors dailySIMs monthlySIMs alerts =
        match monitors with
        |monitor::rem_monitors -> 
            let newDailySIMs = [monitor] |> ThresholdPooledPlan.poolThresholdDailyUpdateUsage today dailySIMs
            let newMonthlySIMs = newDailySIMs |> ThresholdPooledPlan.poolThresholdMonthlyUpdateUsage monthlySIMs
            let newDailyAlerts = [monitor] |> ThresholdPooledPlan.insertPooledPlanDailyAlerts today alerts newDailySIMs
            let newMonthlyAlerts = [monitor] |> ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today alerts newMonthlySIMs
            getPooledPlanNotificationsRec today rem_monitors newDailySIMs newMonthlySIMs (newDailyAlerts @ newMonthlyAlerts)
        |[] -> alerts

    let getPooledPlanNotifications today existingMonthlySIMs monitors =
        getPooledPlanNotificationsRec today monitors [] existingMonthlySIMs []
