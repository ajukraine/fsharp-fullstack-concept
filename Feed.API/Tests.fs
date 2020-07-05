module Tests

open Expecto

[<Tests>]
let properties =
    testList "Sample" [
        testProperty "None" <| ()
        testProperty "Poggers" <| false
    ]