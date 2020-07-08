namespace Feed.AspNetApi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open System.Data.SqlClient
open Domain
open Persistance

[<Route("api/[controller]")>]
[<ApiController>]
type FeedController (getAllFeeds: GetAllFeeds) =
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
    member this.Post([<FromBody>] value:string) =
        ()

    [<HttpPut("{id}")>]
    member this.Put(id:int, [<FromBody>] value:string ) =
        ()

    [<HttpDelete("{id}")>]
    member this.Delete(id:int) =
        ()
