module Persistance.Sql

open FSharp.Data
open System.Data.SqlClient
open Domain
open FsToolkit.ErrorHandling

type PersistanceError = PersistanceError of string

[< Literal>]
let CompilePurposesConnectionString = @"Data Source=localhost,1435;Initial Catalog=feeddb;User ID=sa;Password=jHvogy9@M%2c"

type InsertOneContact = SqlCommandProvider<"""
    INSERT INTO [Feed]
    (Title, PublishDate, ExpiryDate)
    VALUES(@title, @publishDate, @expiryDate)""", CompilePurposesConnectionString>

let insertFeed (sqlConnection: SqlConnection) (feed: Feed) = asyncResult {
    use cmd = new InsertOneContact (sqlConnection)
    let! code = cmd.AsyncExecute(feed.Title, feed.PublishDate, feed.ExpiryDate)

    do! (code = 1) |> Result.requireTrue "Insert failed"
}

type GetAllContract = SqlCommandProvider<"""
    SELECT Title, PublishDate, ExpiryDate FROM [Feed]""", CompilePurposesConnectionString>

let getAllFeeds: (SqlConnection -> GetAllFeeds) = fun (sqlConnection: SqlConnection) () -> async {
    use cmd = new GetAllContract (sqlConnection)
    let! records = cmd.AsyncExecute()

    let mapToDomain (record: GetAllContract.Record) =
        { Title = record.Title; PublishDate = record.PublishDate; ExpiryDate = record.ExpiryDate }

    return records 
        |> Seq.map mapToDomain
        |> List.ofSeq
}

type GetByTitleContract = SqlCommandProvider<"""
    SELECT TOP 1 Title, PublishDate, ExpiryDate FROM [Feed] 
    WHERE Title = @title""", CompilePurposesConnectionString>

let getFeedByTitle (sqlConnection: SqlConnection) title = async {
    use cmd = new GetByTitleContract (sqlConnection)
    let! records = cmd.AsyncExecute(title)

    let mapToDomain (record: GetByTitleContract.Record) =
        { Title = record.Title; PublishDate = record.PublishDate; ExpiryDate = record.ExpiryDate }

    return records |> Seq.map mapToDomain |> Seq.head
}