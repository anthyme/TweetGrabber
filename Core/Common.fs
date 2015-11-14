module Common

open FSharp.Data
open FSharp.Data.Sql
open FSharp.Data.Sql.Common
open Newtonsoft.Json
open TweetSharp
open Settings.Twitter
open Settings.Database

let Twitter = TweetSharp.TwitterService(key,secret)
Twitter.AuthenticateWith(token,tokenSecret)

type TwitterDatabase = 
    SqlDataProvider<ConnectionString = connectionString,DatabaseVendor = DatabaseProviderTypes.MSSQLSERVER,
                    IndividualsAmount = 1000,UseOptionTypes = true>
let Database = TwitterDatabase.GetDataContext()

let deserialize json = try Some <| JsonConvert.DeserializeObject<TwitterStatus> json with ex -> None
let serialize obj = JsonConvert.SerializeObject obj
let extractFullTweet (dbTweet:TwitterDatabase.dataContext.``[dbo].[Tweets]Entity``) = dbTweet.Json |> deserialize
