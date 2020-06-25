module InMemoryPersistence

open FsToolkit.ErrorHandling
open Domain

let mutable private feeds: Feed list = []

let readFeeds () = asyncResult {
    return feeds }

let readFeed title =
    feeds
    |> List.filter (fun feed -> feed.Title |> String.equals title)
    |> function
        | [feed] -> Ok feed
        | [] -> sprintf "Can't find feed with title '%s'" title |> Error
        | _  -> sprintf "There are multiple feeds with title '%s'" title |> Error
    |> Async.result

let addFeed feed = asyncResult {
    do! feeds.Length <= 10 |> Result.requireTrue "Database capacity reached"

    feeds <- feed :: feeds
}