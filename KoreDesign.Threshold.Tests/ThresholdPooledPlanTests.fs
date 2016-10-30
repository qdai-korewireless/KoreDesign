namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
open LanguagePrimitives
open ThresholdPooledPlanTypes

module ThresholdPooledPlanTests = 

   [<TestFixture>]
    type ``Given pooled plan threshold settings`` ()=
        let deviceCount = 10
        let billableDays = 30
        let commitment = 102400L<data>
        let daysInMonth = 30
        let dataSettings:PooledPlanThresholdSettings<data> = {
            DailyThreshold = 0.5f;
            MonthlyThreshold = 0.5f;
            ThresholdWarning = 0.5f;
            }

        [<Test>] member x.
         ``should daily, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Daily Violation 17067L<data> daysInMonth commitment deviceCount billableDays in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Monthly Violation 102401L daysInMonth commitment deviceCount billableDays in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Daily Warning 8534L daysInMonth commitment deviceCount billableDays in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Monthly Warning 51201L daysInMonth commitment deviceCount billableDays in
                expected |> should equal actual

   [<TestFixture>]
    type ``When sprCalculatePooledPlanThresholds is called`` ()=
        let daysInMonth = 30
        let today = new DateTime(2016,10,7)
        let ppsetting:PooledPlanThresholdSettings<data> = {
            DailyThreshold = 0.5f;
            MonthlyThreshold = 0.5f;
            ThresholdWarning = 0.5f
        }
        let dummyPPU:PooledPlanThresholdUsage<data> = {
            MonthlyCommitment = 0L<data>
            MonthlyUsage = 0L<data>
            MonthlyThreshold = 10240L<data>
            DailyCommitment = 0L<data>
            DailyUsage = 0L<data>
            DailyThreshold = 1024L<data>
            DeviceCount = 1
            BillingStartDate = new DateTime(2016,10,1)
            EnterpriseID = 123456
            SIMType = SIMTypes.Proximus
            PerDeviceCommitment = 1024L<data>
            TotalBillableDays = 30
            PooledPlanThresholdSettings = ppsetting
            PoolLevelID = 5566
        }
        let dummyThresholdMonitor:PooledPlanThresholdMonitor<data> = {
            UsageDate = new DateTime(2016,10,7);
            SIMID = 123;
            UsageTotal = 1024L<data>;
            BillingStartDate = new DateTime(2016,10,1);
            PooledPlanThresholdUsage = dummyPPU
        }
        let dummyDailySIM:DailyPooledPlanThresholdUsageBySim<data> = {
            SIMID = 123
            DailyUsage = 1024L<data>
            UsageDate = new DateTime(2016,10,7)
            CreatedDate = new DateTime(2016,10,7)
            PooledPlanThresholdUsage = dummyPPU
        }
        let dummyMonthlySIM:MonthlyPooledPlanThresholdUsageBySim<data> = {
            SIMID = 123
            MonthlyUsage = 1024L<data>
            BillingDays = 30
            PooledPlanThresholdUsage = dummyPPU
        }
        let dummyAlert:PooledPlanAlert<data> = {
            AlertID = 1
            AlertDate = new DateTime(2016,10,7)
            ThresholdInterval = ThresholdInterval.Daily
            ThresholdType = ThresholdType.Violation
            PooledPlanThresholdUsage = dummyPPU
            AlertsToSend = 1
        }
        [<Test>] member x.
         ``should add daily pooled plan by SIM if not exist`` ()=
                let expected = 1 in
                let dailySIMs = [] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.poolThresholdDailyUpdateUsage today dailySIMs monitors) |> Seq.length in
               actual |> should equal expected
        [<Test>] member x.
         ``should update daily pooled plan by SIM if exist`` ()=
                let expected = 2048L<data> in
                let dailySIMs = [dummyDailySIM] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.poolThresholdDailyUpdateUsage today dailySIMs monitors) |> Seq.head in
               actual.DailyUsage |> should equal expected

        [<Test>] member x.
         ``should update monthly pooled plan by SIM`` ()=
                let expected = 2048L<data> in
                let dailySIMs = [dummyDailySIM] in
                let monthlySIMs = [dummyMonthlySIM] in
                
                let actual = (ThresholdPooledPlan.poolThresholdMonthlyUpdateUsage monthlySIMs dailySIMs) |> Seq.head in
               actual.MonthlyUsage |> should equal expected

        [<Test>] member x.
         ``should update monthly usage for Pooled Plan Threshold Usage`` ()=
                let expected = 2048L<data> in
                let pptus = [{dummyPPU with MonthlyUsage = 1024L<data>}] in
                let monthlySIMs = [dummyMonthlySIM] in
                
                let actual = (ThresholdPooledPlan.updatePooledPlanThresholdMonthlyUsage pptus monthlySIMs) |> Seq.head in
               actual.MonthlyUsage |> should equal expected
        [<Test>] member x.
         ``should insert daily pooled plan alert when threshold exeed for the pool level on a day`` ()=
                let expected = 1 in
                let alerts = [] in
                let dailySIMs = [{dummyDailySIM with DailyUsage = 2048L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanDailyAlerts today alerts dailySIMs monitors) |> Seq.length in
               actual |> should equal expected

        [<Test>] member x.
         ``should not insert daily pooled plan alert when existing alert for the day exists`` ()=
                let expected = 1 in
                let alerts = [dummyAlert] in
                let dailySIMs = [{dummyDailySIM with DailyUsage = 2048L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanDailyAlerts today alerts dailySIMs monitors) |> Seq.length in
               actual |> should equal expected
        [<Test>] member x.
         ``should insert daily Warning pooled plan alert when only daily warning is breached`` ()=
                let expected = Warning in
                let alerts = [] in
                let dailySIMs = [dummyDailySIM] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanDailyAlerts today alerts dailySIMs monitors) |> Seq.head in
               actual.ThresholdType |> should equal expected

        [<Test>] member x.
         ``should not insert daily pooled plan alert when no threshold is breached`` ()=
                let expected = 0 in
                let alerts = [] in
                let dailySIMs = [{dummyDailySIM with DailyUsage = 2L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanDailyAlerts today alerts dailySIMs monitors) |> Seq.length in
               actual |> should equal expected

        [<Test>] member x.
         ``should insert monthly pooled plan alert when threshold exeed for the pool level on the month`` ()=
                let expected = 1 in
                let alerts = [] in
                let monthlySIMs = [{dummyMonthlySIM with MonthlyUsage = 20481L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today alerts monthlySIMs monitors) |> Seq.length in
               actual |> should equal expected
        [<Test>] member x.
         ``should not insert monthly pooled plan alert when existing alert for the month exists`` ()=
                let expected = 1 in
                let alerts = [dummyAlert] in
                let monthlySIMs = [{dummyMonthlySIM with MonthlyUsage = 2048L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today alerts monthlySIMs monitors) |> Seq.length in
               actual |> should equal expected
        [<Test>] member x.
         ``should insert monthly Warning pooled plan alert when only monthly warning is breached`` ()=
                let expected = Warning in
                let alerts = [] in
                let monthlySIMs = [{dummyMonthlySIM with MonthlyUsage = 10200L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today alerts monthlySIMs monitors) |> Seq.head in
               actual.ThresholdType |> should equal expected

        [<Test>] member x.
         ``should not insert monthly pooled plan alert when no threshold is breached`` ()=
                let expected = 0 in
                let alerts = [] in
                let monthlySIMs = [{dummyMonthlySIM with MonthlyUsage = 2L<data>}] in
                let monitors = [dummyThresholdMonitor] in
                
                let actual = (ThresholdPooledPlan.insertPooledPlanMonthlyAlerts today alerts monthlySIMs monitors) |> Seq.length in
               actual |> should equal expected