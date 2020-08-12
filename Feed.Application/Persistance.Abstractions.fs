namespace Persistance

open Domain

type InsertFeed = Feed -> Async<Result<unit, string>>
type GetAllFeeds = unit -> Async<Feed list>
type GetFeedByTitle = string -> Async<Feed option>