module ApiSuave

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling

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

let allHandlers = [
    GET >=> path "/feeds" >=> mapAppResult (Application.getAllFeeds())
    GET >=> pathScan "/feeds/%s" (mapAppResult << Application.getFeedByTitle)
    POST >=> path "/feeds/" >=> (fun ctx -> mapAppResult (mapFromJson ctx |> Application.createFeed) ctx)
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)
]

let run() =
    choose allHandlers |> startWebServer defaultConfig