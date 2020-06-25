module Api

open System
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open FsToolkit.ErrorHandling
open Domain

module Persistence = InMemoryPersistence

let foldResult context ar =
    ar
    |> AsyncResult.mapError (fun webPart -> webPart context)
    |> AsyncResult.foldResult id Async.RunSynchronously

let getAllFeeds =
    GET >=> path "/feeds" >=> fun context ->
        async {
            let! feeds = Persistence.readFeeds()

            return! OK (sprintf "%A" feeds) context
        }

let getFeed =
    GET >=> pathScan "/feeds/%s" (fun title -> fun context ->
        async {
            let! feedResult = Persistence.readFeed title
            
            return!
                match feedResult with
                | Ok feed -> OK (sprintf "%A" feed) context
                | Error error -> BAD_REQUEST error context
        })

let createFeed =
    POST >=> pathScan "/feeds/%d" (fun expireInDays -> fun context ->
        asyncResult {
            let now = DateTime.Now

            let! feed = Feed.create "New feed" now (now.AddDays((float)expireInDays))
                        |> Result.mapError BAD_REQUEST

            let! newFeed = Persistence.addFeed feed
                        |> AsyncResult.mapError INTERNAL_ERROR

            return! CREATED (sprintf "%A" newFeed) context
        } |> foldResult context)

let editFeed =
    PUT >=> pathScan "/feeds/%d" (sprintf "Feed %d" >> NOT_IMPLEMENTED)

let allHandlers = [
    getAllFeeds
    getFeed
    createFeed
    editFeed
]