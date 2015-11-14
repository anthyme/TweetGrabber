module Grabber

open System
open System.Linq
open FSharp.Data
open FSharp.Data.Sql
open FSharp.Data.Sql.Common
open Newtonsoft.Json
open TweetSharp
open Settings.Database
open Common

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
