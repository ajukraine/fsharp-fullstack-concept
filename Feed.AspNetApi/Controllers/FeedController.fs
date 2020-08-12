namespace Feed.AspNetApi.Controllers

open Microsoft.AspNetCore.Mvc
open Domain
open Persistance
open Application

// record types should be mutalbe to be model binded as action method param
[<CLIMutable>]
type CreateFeedRequest = {
    Title: string;
    ExpireInDays: int;
}

[<Route("api/[controller]")>]
[<ApiController>]
type FeedController (getAllFeeds: GetAllFeeds, insertFeed: InsertFeed) =
    inherit ControllerBase()

    [<HttpGet>]
    member this.Get() = async {
        let! values = Application.getAllFeeds getAllFeeds
        return ActionResult<Feed list>(values)
    }

    [<HttpGet("{id}")>]
    member this.Get(id:int) =
        let value = "value"
        ActionResult<string>(value)

    [<HttpPost>]
    member this.Post([<FromBody>] request: CreateFeedRequest) = async {
        let! result = Application.createFeed insertFeed { Title = request.Title; ExpireInDays = request.ExpireInDays }

        return (match result with 
            | Ok some -> this.Ok(some) :> IActionResult
            | Error -> this.BadRequest() :> IActionResult)
    }

    [<HttpPut("{id}")>]
    member this.Put(id:int, [<FromBody>] value:string ) =
        ()

    [<HttpDelete("{id}")>]
    member this.Delete(id:int) =
        ()
