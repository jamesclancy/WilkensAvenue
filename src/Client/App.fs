module App

open Elmish
open Elmish.React
open Elmish.UrlParser
open Elmish
open Index
open Elmish.HMR

#if DEBUG

open Elmish.Debug
open Browser
open Contracts

#endif

let userInformationUpdateRequired initial =
    let sub dispatch=
        window.setInterval((fun _ ->
            dispatch (UserInformationRequired)), 1000) |> ignore
    Cmd.ofSub sub

Program.mkProgram Index.init Index.update Index.view
|> Program.withSubscription userInformationUpdateRequired
|> Program.toNavigable (parseHash clientRouter) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
