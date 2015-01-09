open System
open System.Net
open System.Text
open System.IO
 
let siteRoot = @"D:\fs\server\mySite"
let host = "http://localhost:7654/"
 
let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
    let hl = new HttpListener()
    hl.Prefixes.Add host
    hl.Start()
    let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
    async {
        while true do
            let! context = task
            Async.Start(handler context.Request context.Response)
    } |> Async.Start
 
[<EntryPoint>]
let main argv = 
    if not <| Directory.Exists(siteRoot) 
    then Directory.CreateDirectory(siteRoot) |> ignore 
    
    listener (fun req resp ->
        async {
            let txt = if req.RawUrl.Contains("?log=") then req.Url.Query.Substring(String.length "?log=") else ""
            File.AppendAllText(Path.Combine(siteRoot, "log.txt"), txt + "\r\n")
            resp.StatusCode <- 200
            resp.ContentType <- "text/html"
            resp.OutputStream.Close()
        })
    
    printf "%s" "press any key to close the server..."
    Console.ReadKey(false)
    0 // return an integer exit code
