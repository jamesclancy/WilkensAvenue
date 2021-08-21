module Configuration

open System
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

type UserSecretsTarget = UserSecretsTarget of unit

let configureHost (hostBuilder: IHostBuilder) =
    hostBuilder.ConfigureAppConfiguration
        (fun ctx cfg ->

            if ctx.HostingEnvironment.IsDevelopment() then
                cfg.AddUserSecrets<UserSecretsTarget>() |> ignore

            cfg.AddEnvironmentVariables() |> ignore

            if not (cfg.Properties.ContainsKey("OpenIDOAuthClientId")) then
                if not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPEN_ID_CLIENT_ID"))) then
                    [ "OpenIDOAuthClientId", Environment.GetEnvironmentVariable("OPEN_ID_CLIENT_ID") ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore

            if not (cfg.Properties.ContainsKey("OpenIDOAuthKey")) then
                if not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPEN_ID_CLIENT_KEY"))) then
                    [ "OpenIDOAuthKey", Environment.GetEnvironmentVariable("OPEN_ID_CLIENT_KEY") ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore

            if not (cfg.Properties.ContainsKey("OpenIDTenantId")) then
                if not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPEN_ID_TENANT_ID"))) then
                    [ "OpenIDTenantId", Environment.GetEnvironmentVariable("OPEN_ID_TENANT_ID") ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore

            if not (cfg.Properties.ContainsKey("OpenIDAuthority")) then
                if not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPEN_ID_AUTHORITY"))) then
                    [ "OpenIDAuthority", Environment.GetEnvironmentVariable("OPEN_ID_AUTHORITY") ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore

            if not (cfg.Properties.ContainsKey("OpenIDResponseType")) then
                if not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OPEN_ID_RESPONSE_TYPE"))) then
                    [ "OpenIDResponseType", Environment.GetEnvironmentVariable("OPEN_ID_RESPONSE_TYPE") ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore


            if not (cfg.Properties.ContainsKey("ConnectionString")) then

                let connectionUrl =
                    Environment.GetEnvironmentVariable("DATABASE_URL")

                if not (String.IsNullOrWhiteSpace connectionUrl) then
                    let databaseUri = Uri(connectionUrl)

                    let db = databaseUri.LocalPath.TrimStart('/')

                    let userInfo =
                        databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries)

                    let formatedString =
                        $"User ID={userInfo.[0]};Password={userInfo.[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={
                                                                                                                                       db
                        };Pooling=true;SSL Mode=Require;Trust Server Certificate=True;"

                    [ "ConnectionString", formatedString ]
                    |> dict
                    |> cfg.AddInMemoryCollection
                    |> ignore)
    |> ignore

    hostBuilder
