module Pages.BrowseLocations

open Contracts
open Feliz
open Feliz.Bulma
open SharedComponents
open System
open Shared.DataTransferFormats
open Shared.StaticData
open Fulma


let addressSection (address: AddressDetailModel option) =

    let tryReturnString (optStr: string option) =
        match optStr with
        | None -> Seq.empty
        | Some s -> seq { yield Html.span s }

    match address with
    | None -> Seq.empty
    | Some a ->
        seq {
            yield
                Html.div [ prop.children [ Html.b [ prop.content "Location" ]
                                           Html.p [ prop.classes [ "m-t-tiny"; "block" ]
                                                    prop.children [ yield! tryReturnString a.Address1
                                                                    yield! tryReturnString a.Address2
                                                                    yield! tryReturnString a.Address3
                                                                    yield Html.br []
                                                                    yield! tryReturnString a.City
                                                                    yield! tryReturnString a.State
                                                                    yield! tryReturnString a.Zipcode
                                                                    yield Html.br []
                                                                    yield
                                                                        Html.span (
                                                                            sprintf "(%f ,%f" a.Latitude a.Longitude
                                                                        ) ] ] ] ]
        }

let renderNeighborhoodInformation (locationSummaryViewModel: LocationSummaryViewModel) (dispatch: Msg -> unit) =
    [ Html.br []
      Html.div [ prop.text locationSummaryViewModel.Neighborhood ] ]

let locationSummaryCardTitlePart (locationSummaryViewModel: LocationSummaryViewModel) (dispatch: Msg -> unit) =
    Bulma.mediaContent [ Bulma.title.p [ Bulma.title.is4
                                         prop.children [ Html.a [ prop.href (
                                                                      sprintf
                                                                          "#/viewlocation/%s"
                                                                          locationSummaryViewModel.Id
                                                                  )
                                                                  prop.className "has-text-black"
                                                                  prop.text locationSummaryViewModel.Name ] ] ]
                         Bulma.subtitle.p [ Bulma.title.is6
                                            prop.children [ yield!
                                                                (renderNeighborhoodInformation
                                                                    locationSummaryViewModel
                                                                    dispatch) ] ] ]



let locationSummaryCard (locationSummaryViewModel: LocationSummaryViewModel) (dispatch: Msg -> unit) =
    Bulma.column [ column.is12Mobile
                   column.is6Tablet
                   column.is3Desktop
                   prop.children [ Bulma.card [ Bulma.cardImage [ Bulma.image [ Html.a [ prop.href
                                                                                             "#/viewlocation/12312"
                                                                                         prop.children [ Html.img [ prop.src
                                                                                                                        locationSummaryViewModel.ThumbnailUrl
                                                                                                                    prop.alt
                                                                                                                        locationSummaryViewModel.Name
                                                                                                                    prop.className
                                                                                                                        "is-slightly-rounded" ] ] ] ] ]
                                                Bulma.cardContent [ locationSummaryCardTitlePart
                                                                        locationSummaryViewModel
                                                                        dispatch ] ] ] ]

let buildTagInput placeHolder possibleValues currentValue onChanged =

    let valueWithoutOption =
        match currentValue with
        | Some s -> s
        | None _ -> []

    TagsInput.input [ tagsInput.placeholder placeHolder
                      tagsInput.defaultValue valueWithoutOption
                      tagsInput.onTagsChanged onChanged
                      tagsInput.allowOnlyAutoCompleteValues true
                      tagsInput.caseSensitive false
                      tagsInput.removeSelectedFromAutoComplete true
                      tagsInput.tagProperties [ tag.isRounded
                                                color.isWarning ]
                      tagsInput.autoTrim true
                      tagsInput.autoCompleteSource (
                          (fun text ->
                              async {
                                  let lowerCaseText = text.ToString().ToLower()

                                  return
                                      possibleValues
                                      |> List.filter (fun x -> x.ToString().ToLower().Contains(lowerCaseText))
                              })
                      ) ]

let buildDistanceFiler (browseMenuModel: BrowseFilterModel) (dispatch: BrowsePageFilterChange -> unit) =
    [ Bulma.panelHeading [ prop.text "Distance" ]
      Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ Bulma.field.div [ Checkradio.checkbox [ prop.id
                                                                                                             "filterToZip"
                                                                                                         color.isDanger
                                                                                                         prop.isChecked
                                                                                                             browseMenuModel.FilterToZipCode
                                                                                                         prop.onChange
                                                                                                             (fun x ->
                                                                                                                 dispatch (
                                                                                                                     { browseMenuModel with
                                                                                                                           FilterToZipCode =
                                                                                                                               x }
                                                                                                                     |> FilterChanged
                                                                                                                 )) ]
                                                                                   Html.label [ prop.htmlFor
                                                                                                    "filterToZip"
                                                                                                prop.text
                                                                                                    "Filter to zip code" ] ]
                                                                 Bulma.field.div [ Bulma.control.div [ Bulma.input.text [ prop.required
                                                                                                                              true
                                                                                                                          prop.disabled (
                                                                                                                              not
                                                                                                                                  browseMenuModel.FilterToZipCode
                                                                                                                          )
                                                                                                                          prop.onChange
                                                                                                                              (fun x ->
                                                                                                                                  dispatch (
                                                                                                                                      { browseMenuModel with
                                                                                                                                            DistanceToFilterTo =
                                                                                                                                                x }
                                                                                                                                      |> FilterChanged
                                                                                                                                  ))
                                                                                                                          prop.placeholder
                                                                                                                              "Miles from" ] ] ]
                                                                 Bulma.field.div [ Bulma.control.div [ Bulma.input.text [ prop.required
                                                                                                                              true
                                                                                                                          prop.disabled (
                                                                                                                              not
                                                                                                                                  browseMenuModel.FilterToZipCode
                                                                                                                          )
                                                                                                                          prop.onChange
                                                                                                                              (fun x ->
                                                                                                                                  dispatch (
                                                                                                                                      { browseMenuModel with
                                                                                                                                            ZipCodeToFilterTo =
                                                                                                                                                x }
                                                                                                                                      |> FilterChanged
                                                                                                                                  ))
                                                                                                                          prop.placeholder
                                                                                                                              "Zip Code" ] ] ] ] ] ] ]


let leftMenu (browseMenuModel: BrowseFilterModel) (dispatch: BrowsePageFilterChange -> unit) =
    Bulma.panel [ Bulma.panelHeading [ prop.text "Filter" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ Bulma.control.hasIconsLeft
                                                             prop.children [ Bulma.input.text [ prop.placeholder
                                                                                                    "Search" ]
                                                                             Bulma.icon [ Bulma.icon.isLeft
                                                                                          prop.children [ Html.i [ prop.className
                                                                                                                       "fas fa-search" ] ] ] ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "onlyFree"
                                                                                 color.isDanger
                                                                                 prop.isChecked browseMenuModel.OnlyFree
                                                                                 prop.onChange
                                                                                     (fun x ->
                                                                                         dispatch (
                                                                                             { browseMenuModel with
                                                                                                   OnlyFree = x }
                                                                                             |> FilterChanged
                                                                                         )) ]
                                                           Html.label [ prop.htmlFor "onlyFree"
                                                                        prop.text "Only Free" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "onlyOpenAir"
                                                                                 color.isDanger
                                                                                 prop.isChecked
                                                                                     browseMenuModel.OnlyOpenAir
                                                                                 prop.onChange
                                                                                     (fun x ->
                                                                                         dispatch (
                                                                                             { browseMenuModel with
                                                                                                   OnlyOpenAir = x }
                                                                                             |> FilterChanged
                                                                                         )) ]
                                                           Html.label [ prop.htmlFor "onlyOpenAir"
                                                                        prop.text "Only Open Air" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "onlyPrivate"
                                                                                 color.isDanger
                                                                                 prop.isChecked
                                                                                     browseMenuModel.OnlyPrivate
                                                                                 prop.onChange
                                                                                     (fun x ->
                                                                                         dispatch (
                                                                                             { browseMenuModel with
                                                                                                   OnlyPrivate = x }
                                                                                             |> FilterChanged
                                                                                         )) ]
                                                           Html.label [ prop.htmlFor "onlyPrivate"
                                                                        prop.text "Only Private" ] ] ]
                  yield! (buildDistanceFiler browseMenuModel dispatch)
                  Bulma.panelHeading [ prop.text "Tags" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ buildTagInput
                                                                                 "Filter to Tags"
                                                                                 Shared.StaticData.possibleTags
                                                                                 browseMenuModel.SelectedTags
                                                                                 (fun x ->
                                                                                     dispatch (
                                                                                         { browseMenuModel with
                                                                                               SelectedTags = x |> Some }
                                                                                         |> FilterChanged
                                                                                     )) ] ] ]

                  Bulma.panelHeading [ prop.text "Categories" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ buildTagInput
                                                                                 "Filter to Categories"
                                                                                 Shared.StaticData.possibleCategories
                                                                                 browseMenuModel.SelectedCategories
                                                                                 (fun x ->
                                                                                     dispatch (
                                                                                         { browseMenuModel with
                                                                                               SelectedCategories =
                                                                                                   x |> Some }
                                                                                         |> FilterChanged
                                                                                     )) ] ] ]
                  Bulma.panelHeading [ prop.text "Neighborhoods" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ buildTagInput
                                                                                 "Filter to Categories"
                                                                                 Shared.StaticData.possibleNeighborhoods
                                                                                 browseMenuModel.SelectedNeighborhoods
                                                                                 (fun x ->
                                                                                     dispatch (
                                                                                         { browseMenuModel with
                                                                                               SelectedNeighborhoods =
                                                                                                   x |> Some }
                                                                                         |> FilterChanged
                                                                                     )) ] ] ] ]



let renderLocationSummaryCards items (dispatch: Msg -> unit) =
    items
    |> List.map (fun x -> (locationSummaryCard x dispatch))
    |> Seq.ofList

let displaySearchResults (browseViewModel: BrowsePageModel) (dispatch: Msg -> unit) =
    let menuDispatch menuBrowse =
        menuBrowse |> BrowsePageFilterChanged |> dispatch

    let locations = browseViewModel.Results

    match locations with
    | None
    | Some [] -> [ Html.div [ prop.children [ Html.b [ prop.text "nothing found" ] ] ] ]
    | Some items ->
        [ Html.div [ prop.children [ Html.b [ prop.text (
                                                  sprintf
                                                      "A total of %i results found. You are on page %i of %i"
                                                      browseViewModel.Filter.TotalResults
                                                      browseViewModel.Filter.CurrentPage
                                                      browseViewModel.Filter.TotalPages
                                              ) ] ] ]
          Bulma.container [ container.isFluid
                            prop.children [ Bulma.section [ prop.children [ Bulma.columns [ columns.isMultiline
                                                                                            prop.children (
                                                                                                renderLocationSummaryCards
                                                                                                    items
                                                                                                    dispatch
                                                                                            ) ] ] ]
                                            Bulma.section [ prop.children [ Bulma.pagination [ pagination.isCentered
                                                                                               pagination.isLarge
                                                                                               prop.children [ Bulma.paginationPrevious.button [ prop.text
                                                                                                                                                     "Previous"
                                                                                                                                                 prop.disabled (
                                                                                                                                                     browseViewModel.Filter.CurrentPage
                                                                                                                                                     <= 1
                                                                                                                                                 )
                                                                                                                                                 prop.onClick
                                                                                                                                                     (fun _ ->
                                                                                                                                                         menuDispatch (
                                                                                                                                                             { browseViewModel.Filter with
                                                                                                                                                                   CurrentPage =
                                                                                                                                                                       browseViewModel.Filter.CurrentPage
                                                                                                                                                                       - 1 }
                                                                                                                                                             |> LoadPreviousPage
                                                                                                                                                         )) ]
                                                                                                               Bulma.paginationNext.button [ prop.text
                                                                                                                                                 "Next"
                                                                                                                                             prop.disabled (
                                                                                                                                                 browseViewModel.Filter.TotalPages
                                                                                                                                                 <= browseViewModel.Filter.CurrentPage
                                                                                                                                             )
                                                                                                                                             prop.onClick
                                                                                                                                                 (fun _ ->
                                                                                                                                                     menuDispatch (
                                                                                                                                                         { browseViewModel.Filter with
                                                                                                                                                               CurrentPage =
                                                                                                                                                                   browseViewModel.Filter.CurrentPage
                                                                                                                                                                   + 1 }
                                                                                                                                                         |> LoadNextPage
                                                                                                                                                     )) ]

                                                                                                                ]

                                                                                                ]

                                                                             ]

                                                             ]

                                             ]

                             ] ]

let browseView (browseViewModel: BrowsePageModel) (dispatch: Msg -> unit) =

    let menuDispatch menuBrowse =
        menuBrowse |> BrowsePageFilterChanged |> dispatch

    Bulma.section [ prop.classes [ "pt-0"; "is-relative" ]
                    prop.children [ (navBar dispatch)
                                    Bulma.container [ container.isFluid
                                                      prop.children [ Bulma.section [ prop.children [ Bulma.columns [ columns.isMultiline
                                                                                                                      prop.children [ Bulma.column [ column.is12Mobile
                                                                                                                                                     column.is6Tablet
                                                                                                                                                     column.is3Desktop
                                                                                                                                                     column.is2FullHd
                                                                                                                                                     prop.children [ leftMenu
                                                                                                                                                                         browseViewModel.Filter
                                                                                                                                                                         menuDispatch ] ]
                                                                                                                                      Bulma.column [ column.is12Mobile
                                                                                                                                                     column.is6Tablet
                                                                                                                                                     column.is9Desktop
                                                                                                                                                     column.is10FullHd
                                                                                                                                                     prop.children [ yield!
                                                                                                                                                                         displaySearchResults
                                                                                                                                                                             browseViewModel
                                                                                                                                                                             dispatch ] ] ] ] ] ] ] ] ] ]
