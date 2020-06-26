module Application

open System
open FsToolkit.ErrorHandling
open Domain

module Persistence = InMemoryPersistence

type AppError =
    | DomainError of string
    | PersistenceError of string

type CreateFeedDto = { Title : string; ExpireInDays : int }

let createFeed dto = asyncResult {
    let now = DateTime.Now

    let! feed = Feed.create dto.Title now (now.AddDays((float)dto.ExpireInDays))
                |> Result.mapError DomainError

    do! Persistence.addFeed feed
                |> AsyncResult.mapError PersistenceError

    return feed
}

let getAllFeeds () = Persistence.readFeeds() |> AsyncResult.mapError PersistenceError

let getFeedByTitle = Persistence.readFeed >> AsyncResult.mapError DomainError