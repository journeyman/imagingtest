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
    { 
        TimeMs = TimeSpan.ParseExact(splitted.[0], "mm\\:ss\\:fff", CultureInfo.InvariantCulture).TotalMilliseconds
        MemKb = Double.Parse(splitted.[1].Split([|' '|]).[0]) 
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
    let nokiaAvg = readReport <| reportFrom "nokia_warmup_resize_200.txt"
    let exAvg = readReport <| reportFrom "ex_warmup_resize_200.txt"
    reportChart "memory_200.jpg" <| Chart.Bar ["BitmapEx", exAvg.MemKb; "ImagingSdk", nokiaAvg.MemKb]
    reportChart "time_200.jpg" <| Chart.Bar ["BitmapEx", exAvg.TimeMs; "ImagingSdk", nokiaAvg.TimeMs]

    0
