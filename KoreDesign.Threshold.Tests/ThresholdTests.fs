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

        [<Test>] member x.
         ``should daily, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Violation 101L<b> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Violation 101L<b> in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, data, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Warning 51L<b> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Warning 51L<b> in
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

        [<Test>] member x.
         ``should daily, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Daily Violation 17067L<b> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Monthly Violation 102401L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Daily Warning 8534L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Monthly Warning 51201L in
                expected |> should equal actual
                         

   [<TestFixture>]
    type ``When ThresholdApplyPending is called`` ()=
        let pdsetting:PerDeviceThresholdSettings<b> = {
            DailyThreshold = 0L<b>; 
            MonthlyThreshold= 0L<b>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.com";
            NotificationSMS = "1234567"
        }

        let dummyThresholdMonitor = {
            UsageDate = new DateTime(2016,10,7);
            SIMID = 123;
            UsageTotal = 0L<b>;
            Alert= None;
            BillingStartDate = new DateTime(2016,10,1);
            PerDeviceThresholdSettings = pdsetting;
            ExceededThresholdType = None;
            RunningTotal = 0L<b>
        }
        let dummyUsage = {
            MSISDN = "327700019900021";
            IMSI = "206012213919390";
            UsageDate = new DateTime(2016,10,7);
            Usage = 0L<b>
            PLMN = "BELTB"
            SIMID = 123
            BillingStartDate = new DateTime(2016,10,1)
        }
        [<Test>] member x.
         ``usage is added to the threshold monitor for SIM usage`` ()=
                let expected = 1024L<b> in
                let monitors = [dummyThresholdMonitor] in
                let usage =  {dummyUsage with Usage = 1024L<b>} in
                let actual = (Threshold.monitorUsage monitors usage) |> Seq.head in
                expected |> should equal actual.UsageTotal
        [<Test>] member x.
         ``usage is updated to the threshold monitor for existing SIM usage`` ()=
                let expected = 2048L<b> in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<b>}] in
                let usage =  {dummyUsage with Usage = 1024L<b>} in
                let actual = (Threshold.monitorUsage thresholdMonitor usage) |> Seq.find( fun m -> m.SIMID = dummyUsage.SIMID) in
               actual.UsageTotal |> should equal expected

        [<Test>] member x.
         ``running total is calculated by summing a SIM's previous days usage for current billing period`` ()=
                let expected = 2048L<b> in
                let usageDate = new DateTime(2016,10,8) in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<b>; UsageDate=new DateTime(2016,10,7)};{dummyThresholdMonitor with UsageTotal = 1024L<b>; UsageDate=usageDate}] in
                let usage =  {dummyUsage with Usage = 1024L<b>; UsageDate=usageDate} in
                let actual = (Threshold.calculateRunningTotals thresholdMonitor) |> Seq.find( fun m -> m.SIMID = dummyUsage.SIMID && m.UsageDate = usageDate) in
               actual.RunningTotal |> should equal expected