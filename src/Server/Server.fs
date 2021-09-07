module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open Shared.DataTransferFormats
open System
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

open Authentication
open Configuration

let locationInformationApi =
    { getLocation = fun id -> async { return SampleMockData.exampleLocation }
      searchLocations =
          fun req ->
              async {
                  return
                      { SearchRequest = req
                        TotalResults = 90
                        TotalPages = Convert.ToInt32(Math.Ceiling(90m / 16m))
                        CurrentPage = req.CurrentPage
                        Results = Some(List.ofSeq SampleMockData.generateABunchOfItems) }
              }
      updateLocationDetails = fun req -> async { return { ErrorMessage = None } } }

let accountPublishApi  ctx =
    { getCurrentUser = fun unit -> async { return userInformationFromContext ctx } }

let accountInformationApi ctx =
    { login = fun unit -> async { return userInformationFromContext ctx } }

let buildLocationApi next ctx =
    task {
        let handler =
            Remoting.createApi ()
            |> Remoting.withRouteBuilder Route.builderWithoutApiPrefix
            |> Remoting.fromValue locationInformationApi
            |> Remoting.buildHttpHandler

        return! handler next ctx
    }

let buildSecureAccountApi next ctx =
    task {
        let handler =
            Remoting.createApi ()
            |> Remoting.withRouteBuilder Route.builderWithoutApiPrefix
            |> Remoting.fromValue (accountInformationApi ctx)
            |> Remoting.buildHttpHandler

        return! handler next ctx
    }

let buildPublicAccountApi next ctx =
    task {
        let handler =
            Remoting.createApi ()
            |> Remoting.withRouteBuilder Route.builderWithoutApiPrefix
            |> Remoting.fromValue (accountPublishApi ctx)
            |> Remoting.buildHttpHandler

        return! handler next ctx
    }


let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/login"  >=> (authChallenge >=> redirectTo false "/")
             route "/signout" >=> signOut >=>   htmlString "Logged Out."
             subRoute "/api" (buildLocationApi)
             subRoute "/api" (buildPublicAccountApi)
             subRoute "/api" (authChallenge >=> buildSecureAccountApi) ]

let getPort =
    let envPort =
        Environment.GetEnvironmentVariable("PORT")

    if String.IsNullOrWhiteSpace(envPort) then
        "8085"
    else
        envPort

let app =
    application {
        url (sprintf "http://+:%s" getPort)
        host_config configureHost
        use_router routes
        memory_cache
        use_static "public"
        use_gzip
        use_open_id_auth_with_config_from_service_collection openIdConfig
        app_config addAuth
    }

run app
