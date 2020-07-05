module Tests

open Expecto
open Domain
open System

let config = { FsCheckConfig.defaultConfig with maxTest = 10_000 }

let ``feed PublishDate should be before ExpiryDate property`` = fun publishDate expiryDate ->
    Feed.create "Some title" publishDate expiryDate |>
        function
        | Ok _ when publishDate >= expiryDate -> false
        | _ -> true

[<Tests>]
let properties =
    testList "Sample" [
        testPropertyWithConfig config "Feed PublishDate should be before ExpiryDate" <|
            ``feed PublishDate should be before ExpiryDate property``
        
        testCase "PublishDate and ExpiryDate can't same" <| fun () ->
            let now = DateTime.Now

            "PublishDate and ExpiryDate can't same" |>
                Expect.isTrue (``feed PublishDate should be before ExpiryDate property`` now now)
                
    ]