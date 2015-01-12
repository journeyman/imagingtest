open System
open System.IO
open System.Globalization
open FSharp.Charting

let reportsDir = "../../../reports"
let reportFrom file = Path.Combine(reportsDir, file)

type TestData =
    {
        TimeMs : float
        MemKb : double
    }

let parseTestData (input: string) = 
    let splitted = input.Split([|','|])
    let memParts = splitted.[1].Split([|' '|])
    let parsed = Double.Parse(memParts.[0])
    let mem = if memParts.[1].Contains("Mb") 
              then parsed * 1024.0 
              else parsed
    { TimeMs = TimeSpan.ParseExact(splitted.[0], "mm\\:ss\\:fff", CultureInfo.InvariantCulture).TotalMilliseconds
      MemKb = mem 
    }

let readReport path = 
    File.ReadLines path
    |> Seq.map parseTestData
    |> fun list -> { TimeMs = list |> Seq.map (fun x -> x.TimeMs) |> Seq.average; MemKb = list |> Seq.map (fun x -> x.MemKb) |> Seq.average }

let reportChart title (chart: ChartTypes.GenericChart) =
    chart.ShowChart() |> ignore
    chart.SaveChartAs(Path.Combine("../../../", title), ChartTypes.ChartImageFormat.Jpeg)
    

[<EntryPoint>]
let main argv = 
    let nokiaWarm200 = readReport <| reportFrom "nokia_warmup_resize_200.txt"
    let nokiaWarm900 = readReport <| reportFrom "nokia_warmup_resize_900.txt"
    let nokia200 = readReport <| reportFrom "nokia_resize_200.txt"
    let exWarm200 = readReport <| reportFrom "ex_warmup_resize_200.txt"
    let exWarm900 = readReport <| reportFrom "ex_warmup_resize_900.txt"
    let ex200 = readReport <| reportFrom "ex_resize_200.txt"

    Chart.Combine([
        Chart.Bar([nokia200.MemKb; nokiaWarm200.MemKb; nokiaWarm900.MemKb], Labels = ["nokia small pics, first start";"nokia small pics";"nokia big pics"]);
        Chart.Bar([ex200.MemKb; exWarm200.MemKb; exWarm900.MemKb], Labels = ["bitmap small pics, first start";"bitmap small pics";"bitmap big pics"]);
    ]).WithXAxis(Enabled=false)
        |> reportChart "memory.jpg"

    Chart.Combine([
        Chart.Bar([nokia200.TimeMs; nokiaWarm200.TimeMs; nokiaWarm900.TimeMs], Labels = ["nokia small pics, first start";"nokia small pics";"nokia big pics"]);
        Chart.Bar([ex200.TimeMs; exWarm200.TimeMs; exWarm900.TimeMs], Labels = ["bitmap small pics, first start";"bitmap small pics";"bitmap big pics"]);
    ]).WithXAxis(Enabled=false)
        |> reportChart "time.jpg"

    0
