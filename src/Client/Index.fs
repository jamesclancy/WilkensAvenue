module Index

open Elmish
open Fable.Remoting.Client
open Shared
open Elmish.UrlParser
open Contracts
open Shared
open Shared.DataTransferFormats
open ContractMappings
open Elmish.Navigation
open System

let locationInformationApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ILocationInformationApi>

let secureAccountApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ISecureAccountApi>

let publicAccountApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IPublicAccountApi>

let clientRouter : UrlParser.Parser<(ClientRoute -> ClientRoute), ClientRoute> =
    oneOf [
        map About (s "about")
        map (fun x y z -> (x, y, z) |> Find) (s "find" </> str </> str </> i32)
        map Browse (s "browse" </> str)
        map AddLocation (s "addlocation")
        map YourLocations (s "yourlocations")
        map Login (s "login")
        map Register (s "register")
        map Logout (s "logout")
        map YourAccount (s "youraccount")
        map EditLocation (s "editlocation" </> str)
        map ViewLocation (s "viewlocation" </> str)
        map Home (s "")
    ]

let modelWithNewPageModel model pm =
    { model with
          CurrentRoute = None
          PageModel = pm }


let urlUpdate (result: Option<ClientRoute>) (model: Model) : Model * Cmd<Msg> =
    let modelWithNewRoute = (modelWithNewPageModel model) 

    match result with
    | Some Home -> (modelWithNewRoute HomePageModel, [])
    | Some (Find (query, filter, page)) -> (modelWithNewRoute ((query, filter, page) |> FindPageModel), [])
    | Some (Browse id) ->
        (model,
         (Cmd.OfAsync.perform
             locationInformationApi.searchLocations
             emptySearchRequest
             mapSearchResultToReceievedBrowsePageResult))
    | Some AddLocation -> (modelWithNewRoute AddLocationPageModel, [])
    | Some YourLocations -> (modelWithNewRoute YourLocationsPageModel, [])
    | Some Login -> (modelWithNewRoute LoginPageModel, [])
    | Some Register -> (modelWithNewRoute RegisterPageModel, [])
    | Some Logout -> (modelWithNewRoute LogoutPageModel, [])
    | Some YourAccount -> (modelWithNewRoute YourAccountPageModel, [])
    | Some (EditLocation id) -> (modelWithNewRoute (id |> EditLocationPageModel), [])
    | Some (ViewLocation id) ->
        (model, (Cmd.OfAsync.perform locationInformationApi.getLocation id ReceivedLocationDetail))
    | Some About -> (modelWithNewRoute AboutPageModel, [])
    | None -> (modelWithNewRoute NotFound, []) // no matching route - 404

let init (initialRoute: Option<ClientRoute>) : Model * Cmd<Msg> =
    if initialRoute = None then
        { CurrentRoute = None
          CurrentUser = None
          PageModel = HomePageModel
          MenuBurgerExpanded = false },
        Cmd.ofMsg UserInformationRequired
    else
        let (model, cmds) = urlUpdate
                                initialRoute
                                { CurrentRoute = None
                                  CurrentUser = None
                                  PageModel = LoadingScreenPageModel
                                  MenuBurgerExpanded = false }
        let defaultCmd = Cmd.ofMsg UserInformationRequired
        let cmdBatch = seq { defaultCmd
                             cmds
                           }
        model, Cmd.batch cmdBatch



let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg, model.PageModel with
    | ToggleBurger, _ ->
        { model with
              MenuBurgerExpanded = not model.MenuBurgerExpanded },
        Cmd.none
    | UserInformationRequired, _ ->
        model,
        Cmd.OfAsync.perform publicAccountApi.getCurrentUser () (fun x -> x.CurrentUserName |> UserInformationFetched)
    | UserInformationFetched u, _ -> { model with CurrentUser = u }, Cmd.none
    | ReceivedLocationDetail d, _ ->
        modelWithNewPageModel
            model
            ((d, Pages.LocationDetails.defaultEditState d)
             |> ViewLocationPageModel),
        Cmd.none
    | ReceivedBrowsePageResult d, _ -> modelWithNewPageModel model (d |> BrowsePageModel), Cmd.none
    | BrowsePageFilterChanged d, _ ->
        (model,
         (Cmd.OfAsync.perform
             locationInformationApi.searchLocations
             (mapBrowsePageFilterChangeToLocationSearchRequest d)
             mapSearchResultToReceievedBrowsePageResult))
    | LocationDetailUpdated d, ViewLocationPageModel (currPage, currentEditState) ->
        let (pm, ed, cmd) =
            Pages.LocationDetails.updateLocationDetailsModel
                d
                currPage
                currentEditState
                locationInformationApi.updateLocationDetails

        { model with
              PageModel = (pm, ed) |> ViewLocationPageModel },
        cmd

    | _, _ -> model, Cmd.none


open Pages.Home
open Pages.LocationDetails
open Pages.BrowseLocations
open Fable.React


let view (model: Model) (dispatch: Msg -> unit) =
    match model.PageModel with
    | HomePageModel -> homeView model dispatch
    | LoadingScreenPageModel -> SharedComponents.loadingScreen
    | ViewLocationPageModel (d, e) -> locationDetailView model d e dispatch
    | BrowsePageModel bpm -> browseView model bpm dispatch
    | NotFound -> str "404"
    | _ -> str "???"
