module Domain

open System
open FsToolkit.ErrorHandling

type Feed = {
    Title: string
    PublishDate: DateTime
    ExpiryDate: DateTime
}

module Feed =
    let private validateDates feed = result {
        do! feed.PublishDate < feed.ExpiryDate
            |> Result.requireTrue "PublishDate should be less than ExpiryDate"

        return feed
    }
    
    let create title publishDate expiryDate =
        {
            Title = title
            PublishDate = publishDate
            ExpiryDate = expiryDate
        }
        |> validateDates
