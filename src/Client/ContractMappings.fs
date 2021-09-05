module ContractMappings

open Contracts
open Shared.DataTransferFormats


let mapLocationSearchRequestToBrowseFilterModel
    (locationSearchResult: LocationSearchRequest)
    totalPages
    totalResults
    : BrowseFilterModel =
    let searchFilter =
        match locationSearchResult.Query with
        | s when System.String.IsNullOrWhiteSpace(s) -> None
        | s -> Some s

    { TotalResults = totalResults
      TotalPages = totalPages
      CurrentPage = locationSearchResult.CurrentPage
      SearchFilter = searchFilter
      ResultsPerPage = locationSearchResult.ItemsPerPage
      OnlyFree = locationSearchResult.FilterToFree
      OnlyOpenAir = locationSearchResult.FilterToOpenAir
      OnlyPrivate = locationSearchResult.FilterToPrivate
      FilterToZipCode = locationSearchResult.DistanceFilter.IsSome
      DistanceToFilterTo =
          locationSearchResult.DistanceFilter
          |> Option.map (fun x -> x.OriginZipCode)
          |> Option.fold (fun x y -> x) ""
      ZipCodeToFilterTo =
          locationSearchResult.DistanceFilter
          |> Option.map (fun x -> x.MaxDistance.ToString())
          |> Option.fold (fun x y -> x) ""

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

let mapBrowseFilterModelToLocationSearchRequest (locationSearchResult: BrowseFilterModel) : LocationSearchRequest =

    let decimalFromString (s: string) =
        match System.Decimal.TryParse s with
        | (true, value) -> value
        | (_, _) -> 15.0m

    let searchFilter =
        match locationSearchResult.SearchFilter with
        | None -> ""
        | Some s -> s

    let filterToDistance : LocationDistanceFilter option =
        match locationSearchResult.FilterToZipCode with
        | true ->
            Some
                { MaxDistance = decimalFromString locationSearchResult.DistanceToFilterTo
                  OriginZipCode = locationSearchResult.ZipCodeToFilterTo
                  MilesFromZipCode = 0m }
        | false -> None


    { Query = searchFilter
      FilterToFree = locationSearchResult.OnlyFree
      FilterToOpenAir = locationSearchResult.OnlyOpenAir
      FilterToPrivate = locationSearchResult.OnlyPrivate
      DistanceFilter = filterToDistance

      TagFilterFilter = locationSearchResult.SelectedTags
      CategoryFilter = locationSearchResult.SelectedCategories
      NeighborhoodFilter = locationSearchResult.SelectedNeighborhoods
      CurrentPage = locationSearchResult.CurrentPage
      ItemsPerPage = 16 }

let mapBrowsePageFilterChangeToLocationSearchRequest (change: BrowsePageFilterChange) =
    match change with
    | FilterChanged b -> mapBrowseFilterModelToLocationSearchRequest b
    | LoadNextPage b -> mapBrowseFilterModelToLocationSearchRequest b
    | LoadPreviousPage b -> mapBrowseFilterModelToLocationSearchRequest b

let mapSearchResultToReceievedBrowsePageResult (searchResult: LocationSearchResult) : Msg =
    { Filter =
          mapLocationSearchRequestToBrowseFilterModel
              (searchResult.SearchRequest)
              searchResult.TotalPages
              searchResult.TotalResults
      Results = searchResult.Results }
    |> ReceivedBrowsePageResult

let mapLocationDetailToEditRequest (id: string) currentEditState =
    { Id = id
      Summary = currentEditState.NewSummaryContent
      Description = currentEditState.NewDescriptionContent }

let mapUpdateResultsToUpdateDetailResponseReceived
    (d: LocationDetailUpdate)
    (currPage: LocationDetailModel)
    (editState: UpdateLocationDetailState)
    response
    =

    (currPage,
     { editState with
           ErrorMessage = response.ErrorMessage })
    |> LocationDetailUpdateServerResponseReceived
