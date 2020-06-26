module ApiSuave

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.ServerErrors
open Suave.RequestErrors
open Response

open FsToolkit.ErrorHandling

open Api

let mapFromJson<'T> (context : HttpContext) =
    //do! obj.ReferenceEquals (dto, null) |> Result.requireFalse (DomainError "Request is empty")
    parseJsonBytes<'T> context.request.rawForm

let mapAppError appError =
    match appError with
        | Application.DomainError msg -> (HTTP_400, msg)
        | Application.PersistenceError _ -> (HTTP_500,  "Internal Server Error" )

let mapToApiError (httpCode, error) = (httpCode, { Error = error })

let mapToHttpResponse (httpCode, content) =
    content |> mapJson |> UTF8.bytes |> response httpCode

let mapAppResult httpCode asyncResult = fun context -> async {
    let! result = asyncResult

    let httpStatus httpCode content = (httpCode, content)
    
    let response =
        match result with
        | Error appError -> appError |> mapAppError |> mapToApiError |> mapToHttpResponse
        | Ok res -> res |> httpStatus httpCode |> mapToHttpResponse

    return! response context
}

let (@) g f arg = f (g arg) arg

let allHandlers = [
    GET >=> path "/feeds" >=> (Application.getAllFeeds() |> mapAppResult HTTP_200)
    POST >=> path "/feeds" >=> (mapFromJson >> Application.createFeed) @ mapAppResult HTTP_201
    GET >=> pathScan "/feeds/%s" (Application.getFeedByTitle >> mapAppResult HTTP_200)
    PUT >=> pathScan "/feeds/%s" (sprintf "Feed %s" >> asyncResult.Return >> mapAppResult HTTP_501)
]

let run() =
    choose allHandlers |> startWebServer defaultConfig