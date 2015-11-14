// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"..\packages\FSharp.Data.2.2.5\lib\net40"
#I @"..\packages\TweetSharp-Unofficial.2.3.1.2\lib\4.0"
#I @"..\packages\Newtonsoft.Json.7.0.1\lib\net40"
#I @"..\packages\SQLProvider.0.0.9-alpha\lib\net40"
#I @"..\packages\FSharp.Data.SqlClient.1.7.7\lib\net40"
    
#r "FSharp.Data.dll"
#r "TweetSharp.dll"
#r "Hammock.dll"
#r "Newtonsoft.Json.dll"
#r "FSharp.Data.SQLProvider.dll"
#r "FSharp.Data.SqlClient.dll"

#load "Settings.fs"
#load "Common.fs"

open FSharp.Data
open FSharp.Data.Sql
open FSharp.Data.Sql.Common
open Newtonsoft.Json
open TweetSharp
open Settings.Twitter
open Settings.Database


// Define your library scripting code here

