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
