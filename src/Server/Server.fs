module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open System
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

open Authentication
open Configuration

type Storage() =
    let todos = ResizeArray<_>()

    member __.GetTodos() = List.ofSeq todos

    member __.AddTodo(todo: Todo) =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"

let storage = Storage()

storage.AddTodo(Todo.create "Create new SAFE project")
|> ignore

storage.AddTodo(Todo.create "Write your app")
|> ignore

storage.AddTodo(Todo.create "Ship it !!!")
|> ignore

let todosApi =
    { getTodos = fun () -> async { return storage.GetTodos() }
      addTodo =
          fun todo ->
              async {
                  match storage.AddTodo todo with
                  | Ok () -> return todo
                  | Error e -> return failwith e
              } }

let webApp : HttpFunc -> HttpContext -> HttpFuncResult  =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let buildApi next ctx = task {
    let handler =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue todosApi
        |> Remoting.buildHttpHandler
    return! handler next ctx }

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        route "/loggedinhomepage" >=> (authChallenge >=>  htmlString "You are logged in now.")
        subRoute "/api" (authChallenge >=> buildApi)
    ]

let getPort =
    let envPort = Environment.GetEnvironmentVariable("PORT")
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
