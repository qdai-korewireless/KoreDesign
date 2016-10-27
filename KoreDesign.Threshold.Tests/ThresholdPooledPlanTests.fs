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
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Daily Violation 17067L<data> in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, threshold breached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Monthly Violation 102401L in
                expected |> should equal actual

        [<Test>] member x.
         ``should daily, data, pooled plan sim, warning breached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Daily Warning 8534L in
                expected |> should equal actual

          [<Test>] member x.
         ``should monthly, data, pooled plan sim, warningbreached by set usage value`` ()=
                let expected = true in
                let actual = ThresholdPooledPlan.pooledPlanThresholdViolated dataSettings Monthly Warning 51201L in
                expected |> should equal actual

