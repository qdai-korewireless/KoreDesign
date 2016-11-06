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
        
    let updateThresholdMonitor simId usageDate usage =
        use cmd = new SqlCommandProvider<"
            update tbl_sim_threshold_monitoring set GPRSTotal = @usage where simId = @simID and usageDate = @usageDate",connectionString>(connectionString)
        cmd.Execute(usage,simId,usageDate)