module Application

open System
open FsToolkit.ErrorHandling
open Domain

module Persistence = InMemoryPersistence

type AppError =
    | DomainError of string
    | PersistenceError of string

let createFeed expireInDays = asyncResult {
    let now = DateTime.Now

    let! feed = Feed.create "New feed" now (now.AddDays((float)expireInDays))
                |> Result.mapError DomainError

    do! Persistence.addFeed feed
                |> AsyncResult.mapError PersistenceError

    return feed
}
