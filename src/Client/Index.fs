module Index

open Elmish
open Fable.Remoting.Client
open Shared
open Elmish.UrlParser
open Contracts
open Shared
open Shared.DataTransferFormats


let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

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

open Elmish.Navigation
open System

let modelWithNewPageModel model pm =
    { model with
          CurrentRoute = None
          PageModel = pm }

let mapLocationSearchRequestToBrowseFilterModel (locationSearchResult: LocationSearchRequest) : BrowseFilterModel =
    let searchFilter =
        match locationSearchResult.Query with
        | s when System.String.IsNullOrWhiteSpace(s) -> None
        | s -> Some s

    { SearchFilter = searchFilter
      OnlyFree = locationSearchResult.FilterToFree
      OnlyOpenAir = locationSearchResult.FilterToOpenAir
      OnlyPrivate = locationSearchResult.FilterToPrivate
      FilterToZipCode = locationSearchResult.DistanceFilter.IsSome
      DistanceToFilterTo =
          locationSearchResult.DistanceFilter
          |> Option.map (fun x -> x.OriginZipCode) 
          |> Option.fold (fun x y -> x ) ""
      ZipCodeToFilterTo =
          locationSearchResult.DistanceFilter
          |> Option.map (fun x -> x.MaxDistance.ToString()) 
          |> Option.fold (fun x y -> x ) ""

      AvailableTags = [ "B&O"; "Railroad" ]
      AvailableCategories = [ "Steel"; "Port"; "Colonial" ]
      AvailableNeighborhoods =
          [ "Pigtown"
            "Mt Clare"
            "Pratt Monroe"
            "Carrolltown Ridge" ]


      SelectedTags = locationSearchResult.TagFilterFilter
      SelectedCategories = locationSearchResult.CategoryFilter
      SelectedNeighborhoods = locationSearchResult.NeighborhoodFilter }

let mapBrowseFilterModelToLocationSearchRequest (locationSearchResult:  BrowseFilterModel) : LocationSearchRequest =

    let optionFromString s = match s with
    | s when System.String.IsNullOrWhiteSpace(s) -> None
    | s -> Some s

    let decimalFromString (s : string) =
        match System.Decimal.TryParse s with
        | (true, value) -> value
        | (_, _) -> 15.0m

    let searchFilter =
        match locationSearchResult.SearchFilter with
        | None -> ""
        | Some s -> s

    let filterToDistance : LocationDistanceFilter option = match locationSearchResult.FilterToZipCode with
        | true -> Some { MaxDistance = decimalFromString locationSearchResult.DistanceToFilterTo
                         OriginZipCode = locationSearchResult.ZipCodeToFilterTo
                         MilesFromZipCode = 0m}
        | false -> None


    { Query = searchFilter
      FilterToFree = locationSearchResult.OnlyFree
      FilterToOpenAir = locationSearchResult.OnlyOpenAir
      FilterToPrivate  = locationSearchResult.OnlyPrivate
      DistanceFilter = filterToDistance

      TagFilterFilter =  locationSearchResult.SelectedTags
      CategoryFilter  = locationSearchResult.SelectedCategories
      NeighborhoodFilter  = locationSearchResult.SelectedNeighborhoods
      CurrentPage = 1
      ItemsPerPage = 50}

let mapBrowsePageFilterChangeToLocationSearchRequsst (change: BrowsePageFilterChange) =
    match change with
    | FilterChanged b ->mapBrowseFilterModelToLocationSearchRequest b
    | LoadNextPage  b ->mapBrowseFilterModelToLocationSearchRequest b
    | LoadPreviousPage  b ->mapBrowseFilterModelToLocationSearchRequest b

let mapSearchResultToReceievedBrowsePageResult (searchResult: LocationSearchResult) : Msg =
    { Filter = mapLocationSearchRequestToBrowseFilterModel (searchResult.SearchRequest)
      TotalResults = searchResult.TotalResults
      TotalPages = searchResult.TotalPages
      CurrentPage = searchResult.CurrentPage
      Results = searchResult.Results

    }
    |> ReceievedBrowsePageResult

let urlUpdate (result: Option<ClientRoute>) (model: Model) =
    let modelWithNewRoute = (modelWithNewPageModel model)

    match result with
    | Some Home -> (modelWithNewRoute HomePageModel, [])
    | Some (Find (query, filter, page)) -> (modelWithNewRoute ((query, filter, page) |> FindPageModel), [])
    | Some (Browse id) ->
        (model,
         (Cmd.OfAsync.perform todosApi.searchLocations emptySearchRequest mapSearchResultToReceievedBrowsePageResult))
    | Some AddLocation -> (modelWithNewRoute AddLocationPageModel, [])
    | Some YourLocations -> (modelWithNewRoute YourLocationsPageModel, [])
    | Some Login -> (modelWithNewRoute LoginPageModel, [])
    | Some Register -> (modelWithNewRoute RegisterPageModel, [])
    | Some Logout -> (modelWithNewRoute LogoutPageModel, [])
    | Some YourAccount -> (modelWithNewRoute YourAccountPageModel, [])
    | Some (EditLocation id) -> (modelWithNewRoute (id |> EditLocationPageModel), [])
    | Some (ViewLocation id) -> (model, (Cmd.OfAsync.perform todosApi.getLocation id RecievedLocationDetail))
    | Some About -> (modelWithNewRoute AboutPageModel, [])
    | None -> (modelWithNewRoute NotFound, []) // no matching route - 404

let init (initialRoute: Option<ClientRoute>) : Model * Cmd<Msg> =
    if initialRoute = None then
        { CurrentRoute = None
          CurrentUser = None
          PageModel = HomePageModel },
        Cmd.none
    else
        urlUpdate
            initialRoute
            { CurrentRoute = None
              CurrentUser = None
              PageModel = LoadingScreenPageModel }

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | RecievedLocationDetail d -> modelWithNewPageModel model (d |> ViewLocationPageModel), Cmd.none
    | ReceievedBrowsePageResult d ->
        Console.WriteLine(d)
        modelWithNewPageModel model (d |> BrowsePageModel), Cmd.none
    | BrowsePageFilterChanged d -> 
        Console.WriteLine(d)
        (model,
         (Cmd.OfAsync.perform todosApi.searchLocations (mapBrowsePageFilterChangeToLocationSearchRequsst d) mapSearchResultToReceievedBrowsePageResult))
    | _ -> model, Cmd.none


open Pages.Home
open Pages.LocationDetails
open Pages.BrowseLocations
open Fable.React


let view (model: Model) (dispatch: Msg -> unit) =
    match model.PageModel with
    | HomePageModel -> homeView dispatch
    | LoadingScreenPageModel -> SharedComponents.loadingScreen
    | ViewLocationPageModel d -> locationDetailView d dispatch
    | BrowsePageModel bpm -> browseView bpm dispatch
    | NotFound -> str "404"
    | _ -> str "???"
