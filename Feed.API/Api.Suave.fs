module Api

open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling
open MBrace.FsPickler.Json

type ApiError = { Error: string }

let jsonSerializer = FsPickler.CreateJsonSerializer(indent = true, omitHeader = true)
let mapJson = jsonSerializer.PickleToString

let mapAppResult asyncResult context =
    asyncResult
    |> AsyncResult.map (mapJson >> OK)
    |> AsyncResult.mapError (function
        | Application.DomainError msg ->  (BAD_REQUEST, msg)
        | Application.PersistenceError _ -> (INTERNAL_ERROR,  "Internal Server Error" ))
    |> AsyncResult.mapError (fun (status, error) -> status (mapJson { Error = error }))
    |> AsyncResult.foldResult id id
    |> Async.RunSynchronously
    <| context

let allHandlers = [
    GET >=> path "/feeds" >=> mapAppResult (Application.getAllFeeds())
    GET >=> pathScan "/feeds/%s" (mapAppResult << Application.getFeedByTitle)
    POST >=> pathScan "/feeds/%d" (mapAppResult << Application.createFeed)
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)
]