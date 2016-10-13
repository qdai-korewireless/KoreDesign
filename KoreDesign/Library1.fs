namespace KoreDesign
module KoreTypes =

    type ComponentTypes =
    |WebService of string
    |WindowsService
    |WebSite
    |SQLJob
    |Database

    type RunTypes =
    |Scheduled
    |Manule

    type HostServers =
    |SID
    |Prism
    |Proc
    |FE
    |BE
    |API
    |APP
    |APPMSG

    type DBTypes =
    |FE
    |BE
    |Logging
    |Files

    type WebService = {Name:string; EndPoints:seq<string>; HostedOn:HostServers}
    type WindowsService = {Name:string; RunType:RunTypes; HostedOn:HostServers}
    type WebSite = {Name:string; HostedOn:HostServers}
    type SQLJob = {Name:string; HostedOn:HostServers; DBType:DBTypes; RunFrequency:string}
    type Database = {Name:string; HostedOn:HostServers; DBType:DBTypes}

    type SIMTypes =
    |Proximus
    |Tango
    type SIMInternalStates =
    |Active
    |Test
    |Ready
    |TempDeactive
    |PermanentDeactive
    |Stock

    type SIM = {SIMID:int;State:SIMInternalStates}
    type SIMStateSettingByTime = {TimeInDays:int}
    type SIMStateSettingByUsage = {Data:int;SMS:int;Voice:int}
    type SIMStateSetting = {SIMType: SIMTypes; FromState: SIMInternalStates; ToState: SIMInternalStates; ByTime:SIMStateSettingByTime;ByUsage:SIMStateSettingByUsage option}

    type SID = SimState of SIM * SIMStateSetting
    
    type SIMStateTransitionTimer = SIMStateTransitionTimer of SID
    type SIMTransitionService = SIMTransitionService of SIMStateTransitionTimer

    let runTestToReadySIMStateTransition() =
        let setting = {SIMType = Proximus; FromState = Test; ToState = Ready; ByTime = {TimeInDays = 1}; ByUsage = None}
        let sim = {SIMID=1;State=Test}
        SID.SimState (sim,setting) |> SIMStateTransitionTimer |> SIMTransitionService |> printfn "%A"

