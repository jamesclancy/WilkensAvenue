module App

open Elmish
open Elmish.React

#if DEBUG
open Elmish.Debug
open Elmish.HMR
open Elmish
open Index
open Elmish.UrlParser


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
