module App

open Elmish
open Elmish.React
open Elmish.UrlParser
open Elmish
open Index
open Elmish.HMR

#if DEBUG

open Elmish.Debug

#endif

Program.mkProgram Index.init Index.update Index.view
|> Program.toNavigable (parseHash clientRouter) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
