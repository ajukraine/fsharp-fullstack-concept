module Api

open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling

open Application

module Persistence = InMemoryPersistence

let mapAppResult asyncResult context =
    asyncResult
    |> AsyncResult.map (sprintf "%A" >> OK)
    |> AsyncResult.mapError (function
        | DomainError msg -> BAD_REQUEST msg
        | PersistenceError _ -> INTERNAL_ERROR "Internal Server Error")
    |> AsyncResult.foldResult id id
    |> Async.RunSynchronously
    <| context

let allHandlers = [
    GET >=> path "/feeds" >=>
        (Persistence.readFeeds() |> AsyncResult.mapError PersistenceError |> mapAppResult)
    GET >=> pathScan "/feeds/%s"
        (Persistence.readFeed >> AsyncResult.mapError DomainError >> mapAppResult)
    POST >=> pathScan "/feeds/%d"
        (Application.createFeed >> mapAppResult)
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)
]