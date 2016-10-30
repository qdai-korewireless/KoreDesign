
namespace KoreDesign.Threshold.Tests
open FsUnit
open NUnit.Framework
open KoreDesign.Threshold
open System
open LanguagePrimitives
open ThresholdTypes
open ThresholdNotificationForRealTimeGet

module ThresholdNotificationForRealTimeGetTests =

   [<TestFixture>]
    type ``Given data usage`` ()=
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

