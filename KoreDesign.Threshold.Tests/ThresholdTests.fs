namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
module ThresholdTest = 

   [<TestFixture>]
    type ``Given threshold settings`` ()=
        let settings = {
            DailyDataThreshold = 100L<b>;
            DailySMSThreshold = 100L<msg>;
            MonthlyDataThreshold = 100L<b>;
            MonthlySMSThreshold = 100L<msg>;
            DailyDataPoolPlanThreshold = 50.0M<pct>;
            DailySMSPoolPlanThreshold = 50.0M<pct>;
            MonthlyDataPoolPlanThreshold = 50.0M<pct>;
            MonthlySMSPoolPlanThreshold = 50.0M<pct>;
            ThresholdWarning = 50.0m<pct>;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily Data 101L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily SMS 101L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly Data 101L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly SMS 101L in
                expected |> should equal actual