module Index

open Elmish
open Fable.Remoting.Client
open Shared
open Elmish.UrlParser

type PageModel = 
        HomePageModel
      | FindPageModel of string * string * int
      | BrowsePageModel of string
      | AddLocationPageModel 
      | YourLocationsPageModel
      | LoginPageModel
      | RegisterPageModel
      | LogoutPageModel
      | YourAccountPageModel
      | EditLocationPageModel of string
      | ViewLocationPageModel of string
      | AboutPageModel
      | NotFound
      | Unauthorized

type Model =
    {
       CurrentRoute: Option<string>
       PageModel: PageModel
       CurrentUser: Option<string>
    }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

type ClientRoute =
   | Home
   | Find of string * string * int
   | Browse of string
   | AddLocation 
   | YourLocations
   | Login
   | Register
   | Logout
   | YourAccount
   | EditLocation of string
   | ViewLocation of string
   | About

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

let urlUpdate (result:Option<ClientRoute>) (model : Model) =
  let modelWithNewRoute pm =  { model with CurrentRoute = None; PageModel = pm }
  match result with
    | Some Home -> ( modelWithNewRoute HomePageModel , [] )
    | Some (Find (query, filter, page)) -> ( modelWithNewRoute ((query, filter, page) |> FindPageModel), []  )
    | Some (Browse id) -> ( modelWithNewRoute HomePageModel , []  )
    | Some AddLocation  -> ( modelWithNewRoute AddLocationPageModel , [] )
    | Some YourLocations -> ( modelWithNewRoute YourLocationsPageModel , []  )
    | Some Login -> ( modelWithNewRoute LoginPageModel , [] )
    | Some Register -> ( modelWithNewRoute RegisterPageModel ,[] )
    | Some Logout -> ( modelWithNewRoute LogoutPageModel , []  )
    | Some YourAccount -> ( modelWithNewRoute YourAccountPageModel , [] )
    | Some (EditLocation id) -> ( modelWithNewRoute (id |> EditLocationPageModel) , []  )
    | Some (ViewLocation id) -> ( modelWithNewRoute (id |> ViewLocationPageModel) , []  )
    | Some About -> ( modelWithNewRoute AboutPageModel , [] )
    | None ->
      ( modelWithNewRoute NotFound , [] ) // no matching route - 404

let init (initialRoute:Option<ClientRoute>) : Model * Cmd<Msg> =
    let model = { CurrentRoute = None; CurrentUser = None; PageModel = HomePageModel }

    let cmd =
        Cmd.OfAsync.perform todosApi.getTodos () GotTodos

    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | _-> model , Cmd.none

open Feliz
open Feliz.Bulma
open Fable.React
open Fable.React.Props

let homeView  (dispatch: Msg -> unit) =
    section [ Class "section pt-0 is-relative" ]
      [ img [ Class "is-hidden-touch image is-fullwidth"
              Style [ Position PositionOptions.Absolute
                      Top "0"
                      Bottom "0"
                      Left "0"
                      ObjectFit "cover"
                      Height "100%"
                      Width "50%" ]
              Src "https://upload.wikimedia.org/wikipedia/commons/6/63/Carrollton-viaduct.jpg"
              Alt "" ]
        div [ Class "is-relative" ]
          [ nav [ Class "navbar py-4"
                  Style [ BackgroundColor "transparent" ] ]
              [ div [ Class "navbar-brand" ]
                  [ a [ Class "navbar-item is-hidden-touch"
                        Href "#" ]
                      [ h1 [ Class "title is-1 text has-text-primary"
                             Style [ BackgroundColor "rgba(255, 255, 255, 0.8)"
                                     Padding "10px"
                                     BorderRadius "25px" ] ]
                          [ str "Wilkens Avenue" ] ]
                    a [ Class "navbar-item is-hidden-desktop"
                        Href "#" ]
                      [ h1 [ Class "title is-1 text has-text-primary"
                             Style [ BackgroundColor "rgba(255, 255, 255, 0.8)"
                                     Padding "10px"
                                     BorderRadius "25px" ] ]
                          [ str "Wilkens Avenue" ] ]
                    a [ Class "navbar-burger"
                        Role "button"
                        HTMLAttr.Custom ("aria-label", "menu")
                        AriaExpanded false ]
                      [ span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                          [ ]
                        span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                          [ ]
                        span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                          [ ] ] ]
                div [ Class "navbar-menu" ]
                  [ div [ Class "navbar-end" ]
                      [ a [ Class "navbar-item"
                            Href "#" ]
                          [ str "Home" ]
                        a [ Class "navbar-item"
                            Href "#" ]
                          [ str "Find" ]
                        a [ Class "navbar-item"
                            Href "#" ]
                          [ str "Browse" ]
                        a [ Class "navbar-item"
                            Href "#" ]
                          [ str "Login" ]
                        a [ Class "navbar-item"
                            Href "#" ]
                          [ str "Register" ] ] ] ]
            div [ Class "container" ]
              [ div [ Class "pt-5 columns is-multiline" ]
                  [ div [ Class "column is-12 is-5-desktop ml-auto" ]
                      [ div [ Class "mb-5" ]
                          [ h2 [ Class "mb-5 is-size-1 is-size-3-mobile has-text-weight-bold" ]
                              [ str "An investigation into Southwest Baltimore's Industrial heritage." ]
                            p [ Class "subtitle has-text-grey mb-5" ]
                              [ str "This was largeley created as a technical demo." ]
                            div [ Class "buttons" ]
                              [ a [ Class "button is-primary"
                                    Href "https://github.com/jamesclancy/WilkensAvenue" ]
                                  [ str "View on Github" ]
                                a [ Class "button is-primary"
                                    Href "https://jamesclancy.github.io/categories/wilkensavenue/" ]
                                  [ str "Related Blog Posts" ] ] ] ] ] ] ] ]


let view (model: Model) (dispatch: Msg -> unit) =
    match model.PageModel with
    | HomePageModel ->  homeView dispatch
    | NotFound -> str "404"
    | _ -> str "???"