namespace Tweeter.Core

open System
open System.Linq
open FSharp.Data
open FSharp.Data.Sql
open FSharp.Data.Sql.Common
open Newtonsoft.Json
open TweetSharp
open Tweeter.Core.Common
open Settings.Database

module Grabber =

    let getLast _ =
        query { for x in Database.``[dbo].[Tweets]`` do 
                sortByDescending x.Date 
                select x } 
        |> Seq.map extractFullTweet 
        |> Seq.filter Option.isSome 
        |> Seq.map Option.get
        |> Seq.head

    let insertTweet (tweet:TwitterStatus) =
        use cmd = new SqlCommandProvider<"insert into Tweets (Id, Date, Message, Json) values (@Id, @Date, @Message, @Json)", connectionString>()
        cmd.Execute(tweet.Id, tweet.CreatedDate, tweet.Text, serialize tweet)

    let insertTweetSafely tweet =
        try 
            insertTweet tweet |> ignore
            printfn "inserted : %s" tweet.Text 
        with ex -> printfn "error : %s for %s" ex.Message tweet.Text

    let createTimelineOptions (lastId) =
        let options = TweetSharp.ListTweetsOnHomeTimelineOptions()
        options.Count <- Nullable 190
        options.SinceId <- Option.toNullable lastId
        options
    
    let rec captureTweets' iterationIndex = 
        let lastId = try Some (getLast().Id) with ex -> None
        let options = createTimelineOptions (lastId)
        let tweets = Twitter.ListTweetsOnHomeTimeline(options) |> Seq.toList
        tweets |> Seq.filter (fun x -> lastId.IsSome && x.Id <> lastId.Value) |>  Seq.iter insertTweetSafely
        System.Threading.Thread.Sleep 60000
        captureTweets' iterationIndex + 1
    
    let captureTweets () = captureTweets' 1

module Analyzer = 
    let getAll _ =
        query { for x in Database.``[dbo].[Tweets]`` do 
                sortBy x.Date 
                select x } 
        |> Seq.map extractFullTweet 
        |> Seq.filter Option.isSome 
        |> Seq.map Option.get

    let extractAuthorName (tweet:TwitterStatus) =
        if Seq.length tweet.Entities = 0 then tweet.Author.ScreenName
        else 
            match (Seq.head tweet.Entities).EntityType with
            | TwitterEntityType.Media -> "media"
            | TwitterEntityType.Mention -> "mention"
            | TwitterEntityType.HashTag -> "hashtag"
            | _ -> "prout"
    let printAuthors _ = 
        getAll() 
        |> Seq.map extractAuthorName
        |> Seq.iter (printfn "%s")
