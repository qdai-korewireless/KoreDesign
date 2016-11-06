namespace KoreDesign.Data
open FSharp.Data.SqlClient
open FSharp.Data

module ThresholdData =

    [<Literal>]
    let connectionString = 
        @"Data Source=.\EXP2012;Initial Catalog=WPG-LOCAL-FE;Integrated Security=True"

    
    let getThresholdMonitor<'u> simId usageDate =
        use cmd = new SqlCommandProvider<"select top 1 * from tbl_sim_threshold_monitoring where SIMID = @simID and UsageDate = @usageDate",connectionString>(connectionString)

        cmd.Execute(simID = simId, usageDate = usageDate)
        