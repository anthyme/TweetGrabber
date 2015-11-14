module Analyzer

open TweetSharp
open Common

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

