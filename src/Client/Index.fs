module Index

open Elmish
open Fable.Remoting.Client
open Shared
open Elmish.UrlParser
open Contracts
open Shared


let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

let clientRouter : UrlParser.Parser<(ClientRoute -> ClientRoute),ClientRoute> =
    oneOf
        [ map About (s "about")
          map (fun x y z -> (x, y, z) |> Find) (s "find" </> str </> str </> i32)
          map Browse (s "browse" </> str)
          map AddLocation (s "addlocation")
          map YourLocations (s "yourlocations")
          map Login (s "login")
          map Register (s "register")
          map Logout (s "logout")
          map YourAccount (s "youraccount")
          map EditLocation (s "editlocation" </> str )
          map ViewLocation (s "viewlocation" </> str )
          map Home (s "")
          ]

open Elmish.Navigation

let modelWithNewPageModel model pm =  { model with CurrentRoute = None; PageModel = pm }

let urlUpdate (result:Option<ClientRoute>) (model : Model) =
  let modelWithNewRoute = (modelWithNewPageModel model)
  System.Console.Write(result)
  match result with
    | Some Home -> ( modelWithNewRoute HomePageModel , [] )
    | Some (Find (query, filter, page)) -> ( modelWithNewRoute ((query, filter, page) |> FindPageModel), []  )
    | Some (Browse id) -> ( modelWithNewRoute (BrowsePageModel id) , []  )
    | Some AddLocation  -> ( modelWithNewRoute AddLocationPageModel , [] )
    | Some YourLocations -> ( modelWithNewRoute YourLocationsPageModel , []  )
    | Some Login -> ( modelWithNewRoute LoginPageModel , [] )
    | Some Register -> ( modelWithNewRoute RegisterPageModel ,[] )
    | Some Logout -> ( modelWithNewRoute LogoutPageModel , []  )
    | Some YourAccount -> ( modelWithNewRoute YourAccountPageModel , [] )
    | Some (EditLocation id) -> ( modelWithNewRoute (id |> EditLocationPageModel) , []  )
    | Some (ViewLocation id) -> ( model , (Cmd.OfAsync.perform todosApi.getLocation id RecievedLocationDetail)  )
    | Some About -> ( modelWithNewRoute AboutPageModel , [] )
    | None ->
      ( modelWithNewRoute NotFound , [] ) // no matching route - 404

let init (initialRoute:Option<ClientRoute>) : Model * Cmd<Msg> =
    if initialRoute = None then 
         { CurrentRoute = None; CurrentUser = None; PageModel = HomePageModel }, Cmd.none
    else 
        urlUpdate initialRoute { CurrentRoute = None; CurrentUser = None; PageModel = LoadingScreenPageModel }

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | RecievedLocationDetail d -> modelWithNewPageModel model (d |> ViewLocationPageModel), Cmd.none
    | _-> model , Cmd.none


open Pages.Home
open Pages.LocationDetails
open Pages.BrowseLocations
open Fable.React


let view (model: Model) (dispatch: Msg -> unit) =
    match model.PageModel with
    | HomePageModel ->  homeView dispatch
    | LoadingScreenPageModel -> SharedComponents.loadingScreen
    | ViewLocationPageModel d -> locationDetailView d dispatch
    | BrowsePageModel d -> browseView dispatch
    | NotFound -> str "404"
    | _ -> str "???"