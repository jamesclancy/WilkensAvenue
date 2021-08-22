module Pages.Home

open Contracts
open Feliz
open Feliz.Bulma
open SharedComponents
open System
open Shared

let homeView (dispatch: Msg -> unit) =
    let homeImageRotation =
        Some [ { Id = "1"
                 Name = "From wikipedia"
                 Order = 1
                 IsCurrentlySelected = true

                 FullSizeUrl = "https://upload.wikimedia.org/wikipedia/commons/6/63/Carrollton-viaduct.jpg"
                 ThumbnailUrl =
                     "https://upload.wikimedia.org/wikipedia/commons/thumb/6/63/Carrollton-viaduct.jpg/220px-Carrollton-viaduct.jpg"

                 FullSizeHeight = 2950
                 FullSizeWidth = 3708
                 ThumbnailHeight = 150
                 ThumbnailWidth = 220

                 Author = None
                 Description = None
                 Copyright = None
                 MoreInforamtionLink = None


                 IsOwnedByCurrentUser = false
                 SubmissionDate = DateTimeOffset.Now
                 Submitter = "#1 User"
                 SubmitterId = "1" } ]

    let pageContent =
        [ Bulma.title [ title.is2
                        prop.className "mb-5"
                        prop.text "An investigation into Southwest Baltimore's Industrial heritage." ]
          Bulma.title [ title.is4
                        prop.className "has-text-grey "
                        prop.text "This was largely created as a technical demo." ]
          Html.div [ prop.className "buttons"
                     prop.children [ Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.a [ prop.className "button is-primary"
                                              prop.href "https://github.com/jamesclancy/WilkensAvenue"
                                              prop.text "View on Github" ]
                                     Html.a [ prop.className "button is-primary"
                                              prop.href "https://jamesclancy.github.io/categories/wilkensavenue/"
                                              prop.text "Related Blog Posts" ]
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br []
                                     Html.br [] ] ] ]

    halfPageImagePage homeImageRotation pageContent dispatch
