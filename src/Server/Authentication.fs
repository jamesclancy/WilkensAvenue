module Authentication

open Giraffe
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Saturn
open System

let authChallenge : HttpFunc -> HttpContext -> HttpFuncResult =
    requiresAuthentication (
        Auth.challenge Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme
    )

let addAuth (app: IApplicationBuilder) =
    app.UseCookiePolicy().UseAuthentication()

type Saturn.Application.ApplicationBuilder with
    [<CustomOperation("use_open_id_auth_with_config_from_service_collection")>]
    member __.UseOpenIdAuthWithConfigFromServiceCollection
        (
            state: ApplicationState,
            (config: IServiceCollection -> Action<OpenIdConnect.OpenIdConnectOptions>)
        ) =
        let middleware (app: IApplicationBuilder) = app.UseAuthentication()

        let service (s: IServiceCollection) =
            let authBuilder =
                s.AddAuthentication
                    (fun authConfig ->
                        authConfig.DefaultScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                        authConfig.DefaultChallengeScheme <- OpenIdConnectDefaults.AuthenticationScheme
                        authConfig.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme)

            if not state.CookiesAlreadyAdded then
                authBuilder.AddCookie() |> ignore

            authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, (config s))
            |> ignore

            s

        { state with
              ServicesConfig = service :: state.ServicesConfig
              AppConfigs = middleware :: state.AppConfigs
              CookiesAlreadyAdded = true }

let openIdConfig (services: IServiceCollection) =

    let config =
        services
            .BuildServiceProvider()
            .GetService<IConfiguration>()

    let clientId =
        config.GetValue<string>("OpenIDOAuthClientId")

    let tenantId =
        config.GetValue<string>("OpenIDTenantId")

    let secret =
        config.GetValue<string>("OpenIDOAuthKey")

    let authority =
        config.GetValue<string>("OpenIDAuthority")

    let responseType =
        config.GetValue<string>("OpenIDResponseType")

    let fn =
        (fun (opt: Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions) ->
            opt.ClientId <- clientId
            opt.ClientSecret <- secret
            opt.Authority <- authority
            opt.UseTokenLifetime <- true
            opt.CallbackPath <- Microsoft.AspNetCore.Http.PathString("/signin-oidc")
            opt.ResponseType <- responseType)

    new Action<Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions>(fn)


let userInformationFromContext (ctx :HttpContext) =
    let nameClaim = Seq.filter (fun (x: System.Security.Claims.Claim) -> x.Type = "playerName") ctx.User.Claims |> Seq.toList
    let idClaim = Seq.filter (fun (x: System.Security.Claims.Claim) -> x.Type = "playerId") ctx.User.Claims |> Seq.toList

    match nameClaim, idClaim with
    | [ x ], [ y ] -> Some (x.Value, y.Value)
    | [] , [y] -> Some (y.Value, y.Value)
    | _,_ -> None

let userDisplayNameFromContext (ctx : HttpContext) =
    match userInformationFromContext ctx with
    | None ->  None
    | Some (x, y) -> Some x