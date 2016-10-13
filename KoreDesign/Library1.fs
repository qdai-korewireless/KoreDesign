namespace KoreDesign
module KoreTypes =
    (************************************************ Business Types ***********************************************************)

    type HostServers = SID | Prism | Proc | FE | BE | API | APP | APPMSG 
    type DBTypes = FE | BE | Logging | Files
    type ServiceRunTypes = Scheduled | Manule

    type WebService = {Name:string; EndPoints:seq<string>; HostedOn:HostServers}
    type WindowsService = {Name:string; RunType:ServiceRunTypes; HostedOn:HostServers}
    type WebSite = {Name:string; HostedOn:HostServers}
    type SQLJob = {Name:string; HostedOn:HostServers; DBType:DBTypes; RunFrequency:string}
    type Database = {Name:string; HostedOn:HostServers; DBType:DBTypes}

    type SIMTypes = Proximus | Tango
    type SIMInternalStates = Active | Test | Ready | TempDeactive | PermanentDeactive | Stock
    type RequestStatus = Ready of int | Waiting of int | Complete of int | Error of int

    type SIM = {SIMID:int;State:SIMInternalStates;SIMType:SIMTypes}
    type TestSIM = TestSIM of SIM
    type ReadySIM = ReadySIM of SIM
    type ActiveSIM = ActiveSIM of SIM
    type StockSIM = StockSIM of SIM

    type SIMStateSettingByTime = {TimeInDays:int}
    type SIMStateSettingByUsage = {Data:int;SMS:int;Voice:int}
    type SIMStateSetting = {SIMType: SIMTypes; FromState: SIMInternalStates; ToState: SIMInternalStates; ByTime:SIMStateSettingByTime;ByUsage:SIMStateSettingByUsage option}
    type SID = SID of WebSite | SIMState of SIMStateSetting
    type SIMStateTransitionTimer = SIMStateTransitionTimer of WindowsService
    type SIMTransitionService = SIMTransitionService of WebService
    type BGCFiona = BGCFiona of WebService
    type BGCListener = BGCListener of WebService
    type Request = {Status: RequestStatus}
    type NewRequest = NewRequest of Request
    type WaitingRequest = WaitingRequest of Request
    type CompletedRequest = CompletedRequest of Request
    type SAG = SAG of WindowsService
    
    (************************************************ Business Logic & Data Flow ***********************************************************)

    type TestToReadySIMStateTransition = StockSIM -> SID -> (TestSIM * SIMStateSettingByTime * SIMStateSettingByUsage) -> SIMStateTransitionTimer -> SIMTransitionService -> ReadySIM
    type ReadyToActiveSIMStateTransition = ReadySIM -> SID -> (ReadySIM * SIMStateSettingByTime) -> SIMStateTransitionTimer -> SIMTransitionService -> ActiveSIM

    type ActivateSIM = StockSIM -> SID -> NewRequest -> SAG -> BGCFiona -> WaitingRequest -> BGCListener -> SAG -> CompletedRequest -> ActiveSIM