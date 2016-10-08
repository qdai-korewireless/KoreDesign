namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
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
            DataCommitment = 102400L<b>;
            SMSCommitment = 100L<b>;
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
                let actual = Threshold.pooledPlanThresholdViolated settings Daily Data Violation 17067L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily SMS Violation 17L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly Data Violation 102401L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly SMS Violation 101L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily Data Warning 8534L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Daily SMS Warning 9L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly Data Warning 51201L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated settings Monthly SMS Warning 51L in
                expected |> should equal actual

   [<TestFixture>]
    type ``When ThresholdApplyPending is called`` ()=
        let dummyThresholdMonitor = {
            UsageDate = DateTime.Today;
            SIMID = 132;
            DataTotal = 0L<b>;
            SMSTotal = 0L<msg>;
            DataAlert= None;
            SMSAlert = None;
            BillingStartDate = new DateTime(2016,10,1)
        }
        let dummyUsage = {
                    MSISDN = "327700019900021";
                    IMSI = "206012213919390";
                    UsageDate = new DateTime(2016,10,7);
                    Usage = DataUsage 0L<b>
                    PLMN = "BELTB"
                }
        [<Test>] member x.
         ``usage is added to the threshold monitor for new SIM usage`` ()=
                let expected = 1024L<b> in
                let usage =  {dummyUsage with Usage = DataUsage 1024L<b>} in
                let actual = Threshold.monitorUsage dummyThresholdMonitor usage in
                expected |> should equal actual.DataTotal
        [<Test>] member x.
         ``usage is updated to the threshold monitor for existing SIM usage`` ()=
                let expected = 2048L<b> in
                let thresholdMonitor = {dummyThresholdMonitor with DataTotal = 1024L<b>} in
                let usage =  {dummyUsage with Usage = DataUsage 1024L<b>} in
                let actual = Threshold.monitorUsage thresholdMonitor usage in
                expected |> should equal actual.DataTotal