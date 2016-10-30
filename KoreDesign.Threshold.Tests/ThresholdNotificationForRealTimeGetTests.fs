
namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
open LanguagePrimitives
open ThresholdTypes
open ThresholdPooledPlanTypes
open ThresholdNotificationForRealTimeGet

module ThresholdNotificationForRealTimeGetTests =

   [<TestFixture>]
    type ``Given data usage for per device`` ()=
        let today = new DateTime(2016,10,7)

        let dummyUsage = {
            MSISDN = "734074328544284";
            IMSI = "478814821557300";
            UsageDate = new DateTime(2016,10,7);
            Usage = 1024L<data>
            PLMN = "BELTB"
            SIMID = 123
            BillingStartDate = new DateTime(2016,10,1)
        }
        let pdsetting:PerDeviceThresholdSettings<data> = {
            DailyThreshold = 1024L<data>; 
            MonthlyThreshold= 10240L<data>;
            ThresholdWarning = 0.5f;
            NotificationEmail = "test@test.com";
            NotificationSMS = "1234567"
        }
        [<Test>] member x.
         ``should not get daily alert when threshold is not breached`` ()=
                let expected = 0 in
                let usage = {dummyUsage with Usage = 1L<data>} in
                let actual = getPerDeviceNotifications pdsetting today usage |> fst in
                actual.Length |> should equal expected

        [<Test>] member x.
         ``should get daily alert warning when warning threshold is breached`` ()=
                let expected = Warning in
                let usage = {dummyUsage with Usage = 1000L<data>} in
                let actual = getPerDeviceNotifications pdsetting today usage |> fst in
                actual.Head.ThresholdType |> should equal expected

        [<Test>] member x.
         ``should get daily alert violation when violation threshold is breached`` ()=
                let expected = Violation in
                let usage = {dummyUsage with Usage = 2048L<data>} in
                let actual = getPerDeviceNotifications pdsetting today usage |> fst in
                actual.Head.ThresholdType |> should equal expected

        [<Test>] member x.
         ``should get monthly alert violation when violation threshold is breached`` ()=
                let expected = Violation in
                let usage = {dummyUsage with Usage = 20480L<data>} in
                let actual = getPerDeviceNotifications pdsetting today usage |> snd in
                actual.Head.ThresholdType |> should equal expected

   [<TestFixture>]
    type ``Given data usage for pooled plan`` ()=
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
        let dummyMonthlySIM:MonthlyPooledPlanThresholdUsageBySim<data> = {
            SIMID = 123
            MonthlyUsage = 1024L<data>
            BillingDays = 30
            PooledPlanThresholdUsage = dummyPPU
        }
        [<Test>] member x.
         ``should not get daily PP alert when threshold is not breached`` ()=
                let expected = 0 in
                let usage = [{dummyThresholdMonitor with UsageTotal = 1L<data>}] in
                let monthSIMs = [dummyMonthlySIM] in
                let actual = getPooledPlanNotifications today monthSIMs usage |> Seq.length in
                actual |> should equal expected

        [<Test>] member x.
         ``should get daily PP warning alert when warning threshold is breached`` ()=
                let expected = Warning in
                let usage = [{dummyThresholdMonitor with UsageTotal = 1024L<data>}] in
                let monthSIMs = [dummyMonthlySIM] in
                let actual = getPooledPlanNotifications today monthSIMs usage |> Seq.head in
                actual.ThresholdType |> should equal expected

        [<Test>] member x.
         ``should get daily PP violation alert when violation threshold is breached`` ()=
                let expected = Violation in
                let usage = [{dummyThresholdMonitor with UsageTotal = 10240L<data>}] in
                let monthSIMs = [dummyMonthlySIM] in
                let actual = getPooledPlanNotifications today monthSIMs usage |> Seq.head in
                actual.ThresholdType |> should equal expected