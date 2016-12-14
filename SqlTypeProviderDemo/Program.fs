// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data
open FSharp.Data.TypeProviders

[<Literal>]
let compileTimeConnectionString = 
    @"Data Source=localhost;Initial Catalog=AllReady;Integrated Security=True"

type schema = SqlDataConnection<compileTimeConnectionString>
//type Campaigns = SqlEnumProvider<"select name, id from Campaign", compileTimeConnectionString>

[<EntryPoint>]
let main argv = 
    
    //query based type provider in FSharp.Data
    let connectionString = System.Configuration.ConfigurationManager.ConnectionStrings.["ConnectionString"].ConnectionString
    do
        use cmd = new SqlCommandProvider<"select name from Campaign where id>@id" , compileTimeConnectionString>(connectionString)

        let names = cmd.Execute(2) 
        names |> Seq.toList |> List.map System.Console.WriteLine |> ignore
    System.Console.ReadLine() |> ignore

    let db = schema.GetDataContext(connectionString)
    db.DataContext.Log <- System.Console.Out
    let query1 =
        query {
            for row in db.Campaign do
            select row
        }
    query1 |> Seq.iter (fun row -> printfn "%s %d" row.Name row.Id)

    System.Console.ReadLine() |> ignore

    let newRecord = new schema.ServiceTypes.Campaign(Name="Emergency evacuation plan", 
                                                     TimeZoneId = "Unified Zordon Time")

    db.Campaign.InsertOnSubmit(newRecord)
    db.DataContext.SubmitChanges()

    let query2 =
        query {
            for row in db.Campaign do
            where (row.Id > 7)
            select row
        }
    db.Campaign.DeleteAllOnSubmit(query2)
    db.DataContext.SubmitChanges()

    System.Console.ReadLine() |> ignore


    0 // return an integer exit code
