namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
module ThresholdTest = 

   [<TestFixture>]
    type ``Given per-device threshold settings`` ()=
        let dataSettings:PerDeviceThresholdSettings<b> = {
            DailyThreshold = 100L<b>;
            MonthlyThreshold = 100L<b>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        let smsSettings:PerDeviceThresholdSettings<msg> = {
            DailyThreshold = 100L<msg>;
            MonthlyThreshold = 100L<msg>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Violation 101L<b> in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated smsSettings Daily Violation 101L<msg> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Violation 101L<b> in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated smsSettings Monthly Violation 101L<msg> in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, data, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Warning 51L<b> in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated smsSettings Daily Warning 51L<msg> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Warning 51L<b> in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated smsSettings Monthly Warning 51L<msg> in
                expected |> should equal actual

   [<TestFixture>]
    type ``Given pooled plan threshold settings`` ()=
        let dataSettings:PooledPlanThresholdSettings<b> = {
            DeviceCount = 10;
            BillableDays = 30;
            Commitment = 102400L<b>;
            DailyThreshold = 0.5f;
            MonthlyThreshold = 0.5f;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        let smsSettings:PooledPlanThresholdSettings<msg> = {
            DeviceCount = 10;
            BillableDays = 30;
            Commitment = 100L<msg>;
            DailyThreshold = 0.5f;
            MonthlyThreshold = 0.5f;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Daily Violation 17067L<b> in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated smsSettings Daily Violation 17L<msg> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Monthly Violation 102401L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated smsSettings Monthly Violation 101L in
                expected |> should equal actual
        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Daily Warning 8534L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated smsSettings Daily Warning 9L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Monthly Warning 51201L in
                expected |> should equal actual
                         
          [<Test>] member x.
         ``should monthly, sms, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated smsSettings Monthly Warning 51L in
                expected |> should equal actual

   [<TestFixture>]
    type ``When ThresholdApplyPending is called`` ()=
        let dummyThresholdMonitor = {
            UsageDate = DateTime.Today;
            SIMID = 132;
            UsageTotal = 0L<b>;
            Alert= None;
            BillingStartDate = new DateTime(2016,10,1);
            PerDeviceThresholdSettings = {
                                            DailyThreshold = 0L<b>; 
                                            MonthlyThreshold= 0L<b>;
                                            ThresholdWarning = 0.5f;
                                            NotificationEmail = "test@test.com";
                                            NotificationSMS = "1234567"
                                            };
            IsThresholdExceeded = false;
            UsageType = Data
        }
        let dummyUsage = {
                    MSISDN = "327700019900021";
                    IMSI = "206012213919390";
                    UsageDate = new DateTime(2016,10,7);
                    Usage = 0L<b>
                    PLMN = "BELTB"
                }
        [<Test>] member x.
         ``usage is added to the threshold monitor for SIM usage`` ()=
                let expected = 1024L<b> in
                let usage =  {dummyUsage with Usage = 1024L<b>} in
                let actual = Threshold.monitorUsage dummyThresholdMonitor usage in
                expected |> should equal actual.UsageTotal
        [<Test>] member x.
         ``usage is updated to the threshold monitor for existing SIM usage`` ()=
                let expected = 2048L<b> in
                let thresholdMonitor = {dummyThresholdMonitor with UsageTotal = 1024L<b>} in
                let usage =  {dummyUsage with Usage = 1024L<b>} in
                let actual = Threshold.monitorUsage thresholdMonitor usage in
                expected |> should equal actual.UsageTotal