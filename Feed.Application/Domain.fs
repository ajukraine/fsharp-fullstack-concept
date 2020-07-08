module Domain

open System

type Feed = {
    Title: string
    PublishDate: DateTime
    ExpiryDate: DateTime
}

module Feed =
    let private validateDates feed =
        if feed.PublishDate > feed.ExpiryDate
            then Error "PublishDate can't be less than ExpiryDate"
        else Ok feed
    
    let create title publishDate expiryDate =
        {
            Title = title
            PublishDate = publishDate
            ExpiryDate = expiryDate
        }
        |> validateDates