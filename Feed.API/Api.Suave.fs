module ApiSuave

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling
open System.Data.SqlClient

open Api


let mapAppResult asyncResult context =
    asyncResult
    |> AsyncResult.map (mapJson >> OK)
    |> AsyncResult.mapError (function
        | Application.DomainError msg -> (BAD_REQUEST, msg)
        | Application.PersistenceError _ -> (INTERNAL_ERROR,  "Internal Server Error" ))
    |> AsyncResult.mapError (fun (status, error) -> status (mapJson { Error = error }))
    |> AsyncResult.foldResult id id
    |> Async.RunSynchronously
    <| context

let sqlConnectionString = "Data Source=localhost,1435;Initial Catalog=feeddb;User ID=sa;Password=jHvogy9@M%2c"
let createFeed =
    let connection = new SqlConnection(sqlConnectionString)
    connection.Open()
    Application.createFeed <| Persistance.Sql.insertFeed connection

let getFeedByTitle =
    let connection = new SqlConnection(sqlConnectionString)
    connection.Open()
    Application.getFeedByTitle <| Persistance.Sql.getFeedByTitle connection
    
let getAllFeeds () =
    let connection = new SqlConnection(sqlConnectionString)
    connection.Open()
    Application.getAllFeeds (Persistance.Sql.getAllFeeds connection)

let processParameterlessAction action (ctx: HttpContext) = async {
    let! result = action ()
    return! mapJson result |> Successful.OK <| ctx
}

let processParameterizedAction action param = async {
    let! result = action param
    return mapJson result |> Successful.OK
}

let gavno = processParameterizedAction getFeedByTitle

let allHandlers = [
    GET >=> path "/feeds" >=> processParameterlessAction getAllFeeds
    //GET >=> pathScan "/feeds/%s" (processParameterizedAction getFeedByTitle)
    POST >=> path "/feeds" >=> (fun ctx -> mapAppResult (mapFromJson ctx |> createFeed) ctx)
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)
]

let run() =
    choose allHandlers |> startWebServer defaultConfig