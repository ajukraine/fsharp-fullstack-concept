open Suave

let app = choose Api.allHandlers

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig app
    0
