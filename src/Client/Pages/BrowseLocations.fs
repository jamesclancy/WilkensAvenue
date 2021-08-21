module Pages.BrowseLocations

open Contracts
open Feliz
open Feliz.Bulma
open Fable.React
open Fable.React.Props
open SharedComponents
open System
open Shared
open Fulma

let addressSection (address: AddressDetailModel option) =

    let tryReturnString optStr =
        match optStr with
        | None -> Seq.empty
        | Some s -> seq { yield str s }

    match address with
    | None -> Seq.empty
    | Some a ->
        seq {
            yield
                div [] [
                    b [] [ str "Location" ]
                    p [ Class "m-t-tiny block" ] [
                        yield! tryReturnString a.Address1
                        yield! tryReturnString a.Address2
                        yield! tryReturnString a.Address3
                        yield br []
                        yield! tryReturnString a.City
                        yield! tryReturnString a.State
                        yield! tryReturnString a.Zipcode
                        yield br []
                        yield str (sprintf "(%f " a.Latitude)
                        yield str (sprintf ",%f) " a.Longitude)
                    ]
                ]
        }

let renderNeighborhoodInformation (locationSummaryViewModel: LocationSummaryViewModel) (dispatch: Msg -> unit) =
    [ br []
      div [] [
          str locationSummaryViewModel.Neighborhood
      ] ]

let locationSummaryCard (locationSummaryViewModel: LocationSummaryViewModel) (dispatch: Msg -> unit) =
    div [ Class "column is-12-mobile is-6-tablet is-3-desktop" ] [
        div [ Class "card is-shadowless is-slightly-rounded" ] [
            div [ Class "card-image" ] [
                figure [ Class "image" ] [
                    a [ Href "#/viewlocation/12312" ] [
                        img [ Src locationSummaryViewModel.ThumbnailUrl
                              Alt locationSummaryViewModel.Name
                              Class "is-slightly-rounded" ]
                    ]
                ]
            ]
            div [ Class "card-content" ] [
                div [ Class "content" ] [
                    div [] [
                        span [ Class "title is-4 is-capitalized" ] [
                            a [ Href "#/viewlocation/12312"
                                Class "has-text-black" ] [
                                str locationSummaryViewModel.Name
                            ]
                        ]
                        yield! (renderNeighborhoodInformation locationSummaryViewModel dispatch)
                    ]
                ]
            ]
        ]
    ]


let generateABunchOfItems =
    [ 1 .. 50 ]
    |> Seq.map
        (fun x ->
            { Id = "1"
              Name = "Carrollton Viaduct"
              SubTitle = "America's oldest railroad bridge"

              Summary =
                  Some
                      "The Carrollton Viaduct, located over the Gwynns Falls stream near Carroll Park in southwest Baltimore, Maryland, is the first stone masonry bridge built for railroad use in the United States for the Baltimore and Ohio Railroad, founded 1827, with construction beginning the following year and completed 1829. The bridge is named in honor of Charles Carroll of Carrollton (1737-1832), of Maryland, known for being the last surviving signer of the Declaration of Independence, the only Roman Catholic in the Second Continental Congress (1775-1781), and wealthiest man in the Thirteen Colonies of the time of the American Revolutionary War (1775-1783)."
              ThumbnailUrl =
                  "https://upload.wikimedia.org/wikipedia/commons/thumb/6/63/Carrollton-viaduct.jpg/220px-Carrollton-viaduct.jpg"
              ThumbnailHeight = 150
              ThumbnailWidth = 220

              Neighborhood = "Pigtown"
              NeighborhoodId = "1"

              Address =
                  Some
                      { Address1 = Some "123 Wilkens Ave"
                        Address2 = None
                        Address3 = None
                        City = Some "Baltimore"
                        State = Some "MD"
                        Zipcode = Some "21223"
                        Longitude = 1M
                        Latitude = 1M } })

let leftMenu =
    Bulma.panel [ Bulma.panelHeading [ prop.text "Filter" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ Bulma.control.hasIconsLeft
                                                             prop.children [ Bulma.input.text [ prop.placeholder
                                                                                                    "Search" ]
                                                                             Bulma.icon [ Bulma.icon.isLeft
                                                                                          prop.children [ Html.i [ prop.className
                                                                                                                       "fas fa-search" ] ] ] ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "mycheck"
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "mycheck"
                                                                        prop.text "Only Free" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "mycheck"
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "mycheck"
                                                                        prop.text "Only Open Air" ] ] ]
                  Bulma.panelBlock.div [ Bulma.field.div [ Checkradio.checkbox [ prop.id "mycheck"
                                                                                 color.isDanger ]
                                                           Html.label [ prop.htmlFor "mycheck"
                                                                        prop.text "Only Private" ] ] ]
                  Bulma.panelHeading [ prop.text "Distance" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ Bulma.field.div [ Checkradio.checkbox [ prop.id
                                                                                                                         "mycheck"
                                                                                                                     color.isDanger ]
                                                                                               Html.label [ prop.htmlFor
                                                                                                                "mycheck"
                                                                                                            prop.text
                                                                                                                "Filter to zip code" ] ]
                                                                             Bulma.field.div [ Bulma.control.div [ Bulma.input.text [ prop.required
                                                                                                                                          true
                                                                                                                                      prop.placeholder
                                                                                                                                          "Miles from" ] ] ]
                                                                             Bulma.field.div [ Bulma.control.div [ Bulma.input.text [ prop.required
                                                                                                                                          true
                                                                                                                                      prop.placeholder
                                                                                                                                          "Zip Code" ] ] ] ] ] ]

                  Bulma.panelHeading [ prop.text "Tags" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ TagsInput.input [ tagsInput.placeholder
                                                                                                   "Filter to Tags"
                                                                                               tagsInput.defaultValue []
                                                                                               tagsInput.onTagsChanged
                                                                                                   (fun x ->
                                                                                                       Fable.Core.JS.console.log (
                                                                                                           x
                                                                                                       ))
                                                                                               tagsInput.allowOnlyAutoCompleteValues
                                                                                                   true
                                                                                               tagsInput.caseSensitive
                                                                                                   false
                                                                                               tagsInput.removeSelectedFromAutoComplete
                                                                                                   true
                                                                                               tagsInput.tagProperties [ tag.isRounded
                                                                                                                         color.isWarning ]
                                                                                               tagsInput.autoTrim true
                                                                                               tagsInput.autoCompleteSource (
                                                                                                   (fun text ->
                                                                                                       async {
                                                                                                           do!
                                                                                                               Async.Sleep
                                                                                                                   10

                                                                                                           return
                                                                                                               [ "Railroad"
                                                                                                                 "Steel"
                                                                                                                 "Canning" ]
                                                                                                               |> List.filter
                                                                                                                   (fun x ->
                                                                                                                       x.Contains(
                                                                                                                           text
                                                                                                                       ))
                                                                                                       })
                                                                                               ) ] ] ] ]

                  Bulma.panelHeading [ prop.text "Categories" ]
                  Bulma.panelBlock.div [ Bulma.control.div [ prop.children [ TagsInput.input [ tagsInput.placeholder
                                                                                                   "Filter to Categories"
                                                                                               tagsInput.defaultValue []
                                                                                               tagsInput.onTagsChanged
                                                                                                   (fun x ->
                                                                                                       Fable.Core.JS.console.log (
                                                                                                           x
                                                                                                       ))
                                                                                               tagsInput.allowOnlyAutoCompleteValues
                                                                                                   true
                                                                                               tagsInput.caseSensitive
                                                                                                   false
                                                                                               tagsInput.removeSelectedFromAutoComplete
                                                                                                   true
                                                                                               tagsInput.tagProperties [ tag.isRounded
                                                                                                                         color.isWarning ]
                                                                                               tagsInput.autoTrim true
                                                                                               tagsInput.autoCompleteSource (
                                                                                                   (fun text ->
                                                                                                       async {
                                                                                                           do!
                                                                                                               Async.Sleep
                                                                                                                   10

                                                                                                           return
                                                                                                               [ "Railroad"
                                                                                                                 "Steel"
                                                                                                                 "Canning" ]
                                                                                                               |> List.filter
                                                                                                                   (fun x ->
                                                                                                                       x.Contains(
                                                                                                                           text
                                                                                                                       ))
                                                                                                       })
                                                                                               ) ] ] ] ] ]

let browseView (dispatch: Msg -> unit) =

    let renderLocationSummaryCards items =
        items
        |> Seq.map (fun x -> (locationSummaryCard x dispatch))

    section [ Class "section pt-0 is-relative" ] [
        (navBar dispatch)
        div [ Class "container" ] [
            section [ Class "section" ] [
                div [ Class "columns is-multiline" ] [
                    div [ Class "column is-12-mobile is-6-tablet is-3-desktop" ] [
                        leftMenu
                    ]
                    div [ Class "column is-12-mobile is-6-tablet is-9-desktop" ] [
                        div [] [
                            b [] [
                                str "150 exciting locations found..."
                            ]
                        ]
                        div [ Class "container" ] [
                            section [ Class "section" ] [
                                div [ Class "columns is-multiline" ] [
                                    yield! renderLocationSummaryCards generateABunchOfItems
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
