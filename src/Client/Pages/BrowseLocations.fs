module Pages.BrowseLocations

open Contracts
open Feliz
open Feliz.Bulma
open SharedComponents
open System
open Shared.DataTransferFormats
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
                                         prop.children [ Html.a [ prop.href "#/viewlocation/12312"
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

let buildTagInput placeHolder posibleValues currentValue onChanged =
    TagsInput.input [ tagsInput.placeholder placeHolder
                      tagsInput.defaultValue currentValue
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
                                  do! Async.Sleep 10

                                  return
                                      posibleValues
                                      |> List.filter (fun x -> x.Contains(text))
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
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "onlyFree"
                                                                        prop.text "Only Free" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "onlyOpenAir"
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "onlyOpenAir"
                                                                        prop.text "Only Open Air" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "onlyPrivate"
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "onlyPrivate"
                                                                        prop.text "Only Private" ] ] ]
                  yield! (buildDistanceFiler browseMenuModel dispatch)
                  Bulma.panelHeading [ prop.text "Tags" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ buildTagInput
                                                                                 "Filter to Tags"
                                                                                 [ "Railroad"; "Steel"; "Canning" ]
                                                                                 []
                                                                                 (fun x ->
                                                                                     Fable.Core.JS.console.log (x)) ] ] ]

                  Bulma.panelHeading [ prop.text "Categories" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ buildTagInput
                                                                                 "Filter to Categories"
                                                                                 [ "Railroad"; "Steel"; "Canning" ]
                                                                                 []
                                                                                 (fun x ->
                                                                                     Fable.Core.JS.console.log (x)) ] ] ] ]



let renderLocationSummaryCards items (dispatch: Msg -> unit) =
    items
    |> List.map (fun x -> (locationSummaryCard x dispatch))
    |> Seq.ofList

let displaySearchResults (locations: LocationSummaryViewModel list option) (dispatch: Msg -> unit) =
    match locations with
    | None
    | Some [] -> [ Html.div [ prop.children [ Html.b [ prop.text "nothing found" ] ] ] ]
    | Some items ->
        [ Html.div [ prop.children [ Html.b [ prop.text "150 exciting locations found..." ] ] ]
          Bulma.container [ container.isFluid
                            prop.children [ Bulma.section [ prop.children [ Bulma.columns [ columns.isMultiline
                                                                                            prop.children (
                                                                                                renderLocationSummaryCards
                                                                                                    items
                                                                                                    dispatch
                                                                                            ) ] ] ] ] ] ]

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
                                                                                                                                                                             browseViewModel.Results
                                                                                                                                                                             dispatch ] ] ] ] ] ] ] ] ] ]