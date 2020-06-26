module Api

open Newtonsoft.Json
open System.Text

type Error = { Error : string }

let mapJson = JsonConvert.SerializeObject

let parseJsonBytes<'T> (bytes : byte array) =
    bytes
    |> Encoding.UTF8.GetString
    |> JsonConvert.DeserializeObject<'T>