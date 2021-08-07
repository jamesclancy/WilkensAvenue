module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open System
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

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

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

type UserSecretsTarget = UserSecretsTarget of unit
let configureHost (hostBuilder : IHostBuilder) =
  hostBuilder.ConfigureAppConfiguration(fun ctx cfg ->

    if ctx.HostingEnvironment.IsDevelopment() then
        cfg.AddUserSecrets<UserSecretsTarget>() |> ignore

    cfg.AddEnvironmentVariables() |> ignore

    if not(cfg.Properties.ContainsKey("GitHubOAuthClientId")) then
        if not(String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID"))) then
            ["GitHubOAuthClientId",  Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID")] |> dict |> cfg.AddInMemoryCollection |> ignore


    if not(cfg.Properties.ContainsKey("GitHubOAuthKey")) then
        if not(String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_CLIENT_KEY"))) then
            ["GitHubOAuthKey",  Environment.GetEnvironmentVariable("GITHUB_CLIENT_KEY")] |> dict |> cfg.AddInMemoryCollection |> ignore

    if not(cfg.Properties.ContainsKey("ConnectionString")) then

        let connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        if not(String.IsNullOrWhiteSpace connectionUrl) then
            let databaseUri = Uri(connectionUrl)

            let db = databaseUri.LocalPath.TrimStart('/');
            let userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            let formatedString =  $"User ID={userInfo.[0]};Password={userInfo.[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;"

            ["ConnectionString",  formatedString] |> dict |> cfg.AddInMemoryCollection |> ignore
  ) |> ignore
  hostBuilder

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
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
