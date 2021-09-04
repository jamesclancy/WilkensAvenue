module Contracts

open Shared
open System
open Shared.DataTransferFormats


type BrowseFilterModel =
    { SearchFilter: string option
      OnlyFree: bool
      OnlyOpenAir: bool
      OnlyPrivate: bool
      FilterToZipCode: bool
      DistanceToFilterTo: string
      ZipCodeToFilterTo: string

      AvailableTags: string list
      AvailableCategories: string list
      AvailableNeighborhoods: string list

      TotalResults: int
      TotalPages: int
      CurrentPage: int
      ResultsPerPage: int

      SelectedTags: string list option
      SelectedCategories: string list option
      SelectedNeighborhoods: string list option }

type BrowsePageModel =
    { Filter: BrowseFilterModel


      Results: LocationSummaryViewModel list option }

type UpdateLocationDetailState =
    { EditingSummary: bool
      NewSummaryContent: string
      EditingDescription: bool
      NewDescriptionContent: string }

type PageModel =
    | HomePageModel
    | FindPageModel of string * string * int
    | BrowsePageModel of BrowsePageModel
    | AddLocationPageModel
    | YourLocationsPageModel
    | LoginPageModel
    | RegisterPageModel
    | LogoutPageModel
    | YourAccountPageModel
    | EditLocationPageModel of string
    | ViewLocationPageModel of LocationDetailModel * UpdateLocationDetailState
    | AboutPageModel
    | NotFound
    | Unauthorized
    | LoadingScreenPageModel

type Model =
    { CurrentRoute: Option<string>
      PageModel: PageModel
      CurrentUser: Option<string> }

type BrowsePageFilterChange =
    | FilterChanged of BrowseFilterModel
    | LoadNextPage of BrowseFilterModel
    | LoadPreviousPage of BrowseFilterModel

type LocationDetailUpdate =
    | SummaryStartEdit
    | SummaryTextChanged of string
    | SummaryTextSaved
    | SummaryTextChangeCanceled
    | DescriptionStartEdit
    | DescriptionTextChanged of string
    | DescriptionTextSaved
    | DescriptionTextChangeCanceled

type Msg =
    | ReceivedLocationDetail of LocationDetailModel
    | BrowsePageFilterChanged of BrowsePageFilterChange
    | ReceivedBrowsePageResult of BrowsePageModel
    | LocationDetailUpdated of LocationDetailUpdate


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
