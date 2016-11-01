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

    let getPooledPlanNotifications today existingMonthlySIMs monitors =
        
        let dailyPoolSIMs = monitors |> ThresholdPooledPlan.poolThresholdDailyUpdateUsage today []
        let monthlyPoolSIMs = dailyPoolSIMs |> ThresholdPooledPlan.poolThresholdMonthlyUpdateUsage existingMonthlySIMs
        let dailyPPAlerts = monitors |> ThresholdPooledPlan.insertPooledPlanDailyAlerts today [] dailyPoolSIMs
        let monthlyPPAlerts = monitors |> ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today [] monthlyPoolSIMs
        dailyPPAlerts @ monthlyPPAlerts
