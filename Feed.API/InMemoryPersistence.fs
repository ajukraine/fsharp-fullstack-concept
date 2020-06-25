module InMemoryPersistence

open Domain

let mutable private feeds: Feed list = []

let readFeeds () =
    feeds
    |> Async.result

let readFeed title =
    feeds
    |> List.filter (fun feed -> feed.Title |> String.equals title)
    |> function
        | [feed] -> Ok feed
        | [] -> sprintf "Can't find feed with title '%s'" title |> Error
        | _  -> sprintf "There are multiple feeds with title '%s'" title |> Error
    |> Async.result

let addFeed feed =
    if feeds.Length > 10
        then Error "Database capacity reached"
    else
        feeds <- feed :: feeds
        Ok feeds.Head
    |> Async.result