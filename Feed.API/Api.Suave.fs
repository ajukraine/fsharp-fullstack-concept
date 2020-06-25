module Api

open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling

let mapAppResult asyncResult context =
    asyncResult
    |> AsyncResult.map (sprintf "%A" >> OK)
    |> AsyncResult.mapError (function
        | Application.DomainError msg -> BAD_REQUEST msg
        | Application.PersistenceError _ -> INTERNAL_ERROR "Internal Server Error")
    |> AsyncResult.foldResult id id
    |> Async.RunSynchronously
    <| context

let allHandlers = [
    GET >=> path "/feeds" >=> mapAppResult (Application.getAllFeeds())
    GET >=> pathScan "/feeds/%s" (mapAppResult << Application.getFeedByTitle)
    POST >=> pathScan "/feeds/%d" (mapAppResult << Application.createFeed)
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)
]