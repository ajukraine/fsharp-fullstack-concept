module Application

open System

open FsToolkit.ErrorHandling

open Domain
open Persistance

module Persistence = InMemoryPersistence

type AppError =
    | DomainError of string
    | PersistenceError of string

type CreateFeedDto = { Title : string; ExpireInDays : int }

let createFeed (insertFeed: InsertFeed) dto = asyncResult {
    let now = DateTime.Now

    let! feed = Feed.create dto.Title now (now.AddDays((float)dto.ExpireInDays))
                |> Result.mapError DomainError

    do! insertFeed feed
                |> AsyncResult.mapError PersistenceError

    return feed
}

let getAllFeeds (getAllFeeds: GetAllFeeds) = getAllFeeds()

let getFeedByTitle (getFeedByTitle: GetFeedByTitle) title = getFeedByTitle(title)