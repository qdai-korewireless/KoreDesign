open System.Diagnostics
open System.IO
open System

let (|EndsWith|_|) extension (file : string) = 
    if file.EndsWith(extension) 
    then Some("test result") 
    else None

let testActivePattern p =
    match p with
    |EndsWith ".txt" s -> printfn "%s text file: %s" s p
    |_ -> printfn "file: %s" p

testActivePattern "abc.txt"

let rec allFilesUnder baseFolder = 
    seq {
        yield! Directory.GetFiles(baseFolder)
        for subDir in Directory.GetDirectories(baseFolder) do
            yield! allFilesUnder subDir 
        }



let shellExecute program args =
    let startInfo = new ProcessStartInfo()
    startInfo.FileName <- program
    startInfo.Arguments <- args
    startInfo.UseShellExecute <- true

    let proc = Process.Start(startInfo)
    proc.WaitForExit()
    ()


allFilesUnder "D:\Dev\End-To-End\KoreDesign.Threshold"
|> Seq.filter (fun a ->
                    match a with
                    | EndsWith ".txt" _
                        -> true
                    | _ -> false)
|> Seq.iter (fun f -> printfn "%A" f)