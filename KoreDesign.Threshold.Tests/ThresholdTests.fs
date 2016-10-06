namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
module ThresholdTest = 

   [<TestFixture>]
    type ``Given per-device threshold settings`` ()=
        let settings:PerDeviceThresholdSettings = {
            DailyDataThreshold = 100L<b>;
            DailySMSThreshold = 100L<msg>;
            MonthlyDataThreshold = 100L<b>;
            MonthlySMSThreshold = 100L<msg>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily Data Violation 101L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily SMS Violation 101L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly Data Violation 101L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly SMS Violation 101L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, data, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily Data Warning 51L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Daily SMS Warning 51L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly Data Warning 51L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated settings Monthly SMS Warning 51L in
                expected |> should equal actual

   [<TestFixture>]
    type ``Given pooled plan threshold settings`` ()=
        let settings:PooledPlanThresholdSettings = {
            DeviceCount = 10;
            BillableDays = 30;
            Commitment = 102400L<b>;
            DailyDataThreshold = 0.5f;
            DailySMSThreshold = 0.5f;
            MonthlyDataThreshold = 0.5f;
            MonthlySMSThreshold = 0.5f;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily Data Violation 101L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily SMS Violation 101L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly Data Violation 101L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly SMS Violation 101L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily Data Warning 51L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily SMS Warning 51L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly Data Warning 51L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly SMS Warning 51L in
                expected |> should equal actual