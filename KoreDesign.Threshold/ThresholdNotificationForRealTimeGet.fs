namespace KoreDesign.Threshold
open System
open ThresholdTypes
open ThresholdPooledPlanTypes

module ThresholdNotificationForRealTimeGet = 
    
    let getPerDeviceNotifications pdsetting today usage =

        let monitor = usage
                    |> Threshold.monitorUsage pdsetting []

        let dailyAlerts = monitor
                        |> Threshold.calculateRunningTotals 
                        |> Threshold.updateAlert []
        let monthlyAlerts = monitor
                            |> Threshold.updateThresholdSummary []
                            |> Threshold.updateMonthlyAlert today []
        dailyAlerts,monthlyAlerts

    let getPooledPlanNotifications today existingMonthlySIMs monitors =
        
        let dailyPoolSIMs = monitors |> ThresholdPooledPlan.poolThresholdDailyUpdateUsage today []
        let monthlyPoolSIMs = dailyPoolSIMs |> ThresholdPooledPlan.poolThresholdMonthlyUpdateUsage existingMonthlySIMs
        let dailyPPAlerts = monitors |> ThresholdPooledPlan.insertPooledPlanDailyAlerts today [] dailyPoolSIMs
        let monthlyPPAlerts = monitors |> ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today [] monthlyPoolSIMs
        dailyPPAlerts @ monthlyPPAlerts
