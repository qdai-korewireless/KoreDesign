namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
open LanguagePrimitives

module ThresholdTest = 

   [<TestFixture>]
    type ``Given per-device threshold settings`` ()=
        let dataSettings:PerDeviceThresholdSettings<data> = {
            DailyThreshold = 100L<data>;
            MonthlyThreshold = 100L<data>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Violation 101L<data> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Violation 101L<data> in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, data, per-device sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Daily Warning 51L<data> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, per-device sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.perDeviceThresholdViolated dataSettings Monthly Warning 51L<data> in
                expected |> should equal actual

   [<TestFixture>]
    type ``Given pooled plan threshold settings`` ()=
        let dataSettings:PooledPlanThresholdSettings<data> = {
            DeviceCount = 10;
            BillableDays = 30;
            Commitment = 102400L<data>;
            DailyThreshold = 0.5f;
            MonthlyThreshold = 0.5f;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.come";
            NotificationSMS = "2041234567"}

        [<Test>] member x.
         ``should daily, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = Threshold.pooledPlanThresholdViolated dataSettings Daily Violation 17067L<data> in
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
        let pdsetting:PerDeviceThresholdSettings<data> = {
            DailyThreshold = 0L<data>; 
            MonthlyThreshold= 0L<data>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.com";
            NotificationSMS = "1234567"
        }

        let dummyThresholdMonitor = {
            UsageDate = new DateTime(2016,10,7);
            SIMID = 123;
            UsageTotal = 0L<data>;
            BillingStartDate = new DateTime(2016,10,1);
            PerDeviceThresholdSettings = pdsetting;
            ExceededThresholdType = None;
            RunningTotal = 0L<data>
            EnterpriseID = 123456;
            SIMType = SIMTypes.Proximus;
            DailyAlert = None;
        }
        let dummyUsage = {
            MSISDN = "327700019900021";
            IMSI = "206012213919390";
            UsageDate = new DateTime(2016,10,7);
            Usage = 0L<data>
            PLMN = "BELTB"
            SIMID = 123
            BillingStartDate = new DateTime(2016,10,1)
        }
        let dummyUsageDate:ThresholdDate = {
            EnterpriseID = 123456;
            SIMType = SIMTypes.Proximus;
            UsageDate = new DateTime(2016,10,7)
        }
        let dummyAlert = {
            EnterpriseID = 123456;
            SIMType = SIMTypes.Proximus;
            AlertID = 8000001;
            NumOfIncidents = 0<data>;
            AlertDate = new DateTime(2016,10,7);
            ThresholdType = ThresholdType.Violation
        }
        let dummySummary = {
            SIMID = 123;
            SIMType = SIMTypes.Proximus;
            EnterpriseID = 123456;
            BillingStartDate = new DateTime(2016,10,1);
            DaysTracked = 0<data>;
            DaysExceeded = 0<data>;
            MonthTotal = 0L<data>;
            ExceededMonthlyThresholdType = Some ThresholdType.Violation        
        }
        let dummySummaryByDay = {
            UsageDate = new DateTime(2016,10,7);
            SIMType = SIMTypes.Proximus;
            EnterpriseID = 123456;
            BillingStartDate = new DateTime(2016,10,1);
            UsageTotal = 0L<data>;
            RunningTotal = 0L<data>;
        }
        let dummyMonthlyAlert = {
            EnterpriseID = 123456;
            AlertID = 8010001;
            NumOfSIMs = 0<data>;
            AlertDate = new DateTime(2016,10,7);
            BillingStartDate = new DateTime(2016,10,1);
            ThresholdType = ThresholdType.Violation
        }
        [<Test>] member x.
         ``usage is added to the threshold monitor for SIM usage`` ()=
                let expected = 1024L<data> in
                let monitors = [dummyThresholdMonitor] in
                let usage =  {dummyUsage with Usage = 1024L<data>} in
                let actual = (Threshold.monitorUsage monitors usage) |> Seq.head in
                expected |> should equal actual.UsageTotal
        [<Test>] member x.
         ``usage is updated to the threshold monitor for existing SIM usage`` ()=
                let expected = 2048L<data> in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>}] in
                let usage =  {dummyUsage with Usage = 1024L<data>} in
                let actual = (Threshold.monitorUsage thresholdMonitor usage) |> Seq.find( fun m -> m.SIMID = dummyUsage.SIMID) in
               actual.UsageTotal |> should equal expected

        [<Test>] member x.
         ``running total is calculated by summing a SIM's previous days usage for current billing period`` ()=
                let expected = 2048L<data> in
                let usageDate = new DateTime(2016,10,8) in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=new DateTime(2016,10,7)};{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=usageDate}] in
                let usage =  {dummyUsage with Usage = 1024L<data>; UsageDate=usageDate} in
                let actual = (Threshold.calculateRunningTotals thresholdMonitor) |> Seq.find( fun m -> m.SIMID = dummyUsage.SIMID && m.UsageDate = usageDate) in
               actual.RunningTotal |> should equal expected

        [<Test>] member x.
         ``insert usage date if not already exists`` ()=
                let expected = 3 in
                let usageDates = [{dummyUsageDate with UsageDate = new DateTime(2016,10,7)};{dummyUsageDate with UsageDate = new DateTime(2016,10,10)}] in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=new DateTime(2016,10,8)};{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=new DateTime(2016,10,7)}] in
                let actual = (Threshold.addUsageDate usageDates thresholdMonitor) |> Seq.length in
               actual |> should equal expected

        [<Test>] member x.
         ``insert new alert when the usage exceed threshold violation for the first time of a compony today`` ()=
                let expected = 8000002 in
                let alerts = [] in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=new DateTime(2016,10,8); ExceededThresholdType = Some ThresholdType.Violation}] in
                let actual = (Threshold.updateAlert alerts thresholdMonitor) |> Seq.head in
               actual.AlertID |> should equal expected

        [<Test>] member x.
         ``increase alert daily incidents count if SIM for the company exceeds threshold violation`` ()=
                let expected = 2<data> in
                let alerts = [{dummyAlert with NumOfIncidents = 1<data>}] in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>; UsageDate=new DateTime(2016,10,7);ExceededThresholdType = Some ThresholdType.Violation; DailyAlert = Some dummyAlert}] in
                let actual = (Threshold.updateAlert alerts thresholdMonitor) |> Seq.head in
               actual.NumOfIncidents |> should equal expected

        [<Test>] member x.
         ``when monitored usage exceed threshold, alert should be assigned`` ()=
                let expected = 8000001 in
                let alerts = [dummyAlert] in
                let thresholdMonitor = [{dummyThresholdMonitor with ExceededThresholdType = Some ThresholdType.Violation}] in
                let actual = (Threshold.updateMonitorAlert (new DateTime(2016,10,7)) alerts thresholdMonitor) |> Seq.head in
               actual.DailyAlert.Value.AlertID |> should equal expected

        [<Test>] member x.
         ``when summary does not have new SIM usage, then add it`` ()=
                let expected = 1 in
                let thresholdMonitor = [dummyThresholdMonitor] in
                let summary = [] in
                let actual = (Threshold.updateThresholdSummary summary thresholdMonitor) |> Seq.length in
               actual |> should equal expected
        [<Test>] member x.
         ``when summary does not have new SIM usage, and add usage breach threshold warning, then the warning exceeded property is set`` ()=
                let expected = Some ThresholdType.Warning in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 52224L<data>; PerDeviceThresholdSettings = {dummyThresholdMonitor.PerDeviceThresholdSettings with MonthlyThreshold = 102400L<data>}}] in
                let summary = [] in
                let actual = (Threshold.updateThresholdSummary summary thresholdMonitor) |> Seq.head in
               actual.ExceededMonthlyThresholdType |> should equal expected

        [<Test>] member x.
         ``when summary has existing SIM, then the monitor usage should add new usage`` ()=
                let expected = 2048L<data> in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>}] in
                let summary = [{dummySummary with MonthTotal = 1024L<data>}] in
                let actual = (Threshold.updateThresholdSummary summary thresholdMonitor) |> Seq.head in
               actual.MonthTotal |> should equal expected

        [<Test>] member x.
         ``when summary has existing SIM, and new usage breach monthly threshold, then the violation exceeded property is set`` ()=
                let expected = Some ThresholdType.Violation in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 102400L<data>; PerDeviceThresholdSettings = {dummyThresholdMonitor.PerDeviceThresholdSettings with MonthlyThreshold = 102400L<data>}}] in
                let summary = [{dummySummary with MonthTotal = 1024L<data>}] in
                let actual = (Threshold.updateThresholdSummary summary thresholdMonitor) |> Seq.head in
               actual.ExceededMonthlyThresholdType |> should equal expected
        [<Test>] member x.
         ``when summary by day does not have new SIM usage, then add it`` ()=
                let expected = 1 in
                let thresholdMonitor = [dummyThresholdMonitor] in
                let summary = [] in
                let actual = (Threshold.updateThresholdSummaryPerDay summary thresholdMonitor) |> Seq.length in
               actual |> should equal expected

        [<Test>] member x.
         ``when summary by day has existing usage on a date, then add usage`` ()=
                let expected = 2048L<data> in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>}] in
                let summary = [{dummySummaryByDay with UsageTotal = 1024L<data>; RunningTotal = 1024L<data>}] in
                let actual = (Threshold.updateThresholdSummaryPerDay summary thresholdMonitor) |> Seq.head in
               actual.UsageTotal |> should equal expected
        [<Test>] member x.
         ``when summary by day has existing usage on a date, then running total is updated`` ()=
                let expected = 3072L<data> in
                let thresholdMonitor = [{dummyThresholdMonitor with UsageTotal = 1024L<data>}] in
                let summary = [{dummySummaryByDay with UsageTotal = 1024L<data>};{dummySummaryByDay with UsageTotal = 1024L<data>;UsageDate=new DateTime(2016,10,2)}] in
                let actual = (Threshold.updateThresholdSummaryPerDay summary thresholdMonitor) |> Seq.head in
               actual.RunningTotal |> should equal expected


        [<Test>] member x.
         ``insert new monthly alert when the usage exceed threshold violation for the first time of a compony today`` ()=
                let expected = 1 in
                let alerts = [] in
                let summary = [{dummySummary with MonthTotal = 1024L<data>; ExceededMonthlyThresholdType = Some ThresholdType.Violation}] in
                let actual = (Threshold.updateMonthlyAlert alerts summary (new DateTime(2016,10,7))) |> Seq.length in
               actual |> should equal expected

        [<Test>] member x.
         ``increase alert monthly SIMs count if SIM for the company exceeds threshold violation`` ()=
                let expected = 2 in
                let alerts = [{dummyMonthlyAlert with NumOfSIMs = 1<data>}] in
                let summary = [{dummySummary with MonthTotal = 1024L<data>; ExceededMonthlyThresholdType = Some ThresholdType.Violation}] in
                let actual = (Threshold.updateMonthlyAlert alerts summary (new DateTime(2016,10,7))) |> Seq.head in
               actual.NumOfSIMs |> should equal expected
        [<Test>] member x.
         ``when monitored usage exceed monthly threshold, alert id should be assigned`` ()=
                let expected = 8010002 in
                let alerts = [dummyMonthlyAlert] in
                let summary = [{dummySummary with MonthTotal = 1024L<data>; ExceededMonthlyThresholdType = Some ThresholdType.Violation}] in
                let actual = (Threshold.updateMonthlyAlert alerts summary (new DateTime(2016,10,8))) |> Seq.head in
               actual.AlertID |> should equal expected