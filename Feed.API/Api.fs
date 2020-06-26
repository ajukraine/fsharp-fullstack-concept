module Api

open Newtonsoft.Json
open System.Text
open FsToolkit.ErrorHandling
open Suave

type Error = { Error : string }

let mapJson = JsonConvert.SerializeObject

let parseJsonBytes<'T> (bytes : byte array) =
    bytes
    |> Encoding.UTF8.GetString
    |> JsonConvert.DeserializeObject<'T>

let mapFromJson<'T> (context : HttpContext) =
    //do! obj.ReferenceEquals (dto, null) |> Result.requireFalse (DomainError "Request is empty")
    parseJsonBytes<'T> context.request.rawForm