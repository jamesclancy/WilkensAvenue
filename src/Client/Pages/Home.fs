module Pages.Home

open Contracts
open Feliz
open Feliz.Bulma
open Fable.React
open Fable.React.Props
open SharedComponents
open System
open Shared

let homeView  (dispatch: Msg -> unit) =
    let homeImageRotation =  Some [
                            {
                                Id = "1"
                                Name = "From wikipedia"
                                Order = 1
                                IsCurrentlySelected= true

                                FullSizeUrl= "https://upload.wikimedia.org/wikipedia/commons/6/63/Carrollton-viaduct.jpg"
                                ThumbnailUrl= "https://upload.wikimedia.org/wikipedia/commons/thumb/6/63/Carrollton-viaduct.jpg/220px-Carrollton-viaduct.jpg"

                                FullSizeHeight= 2950
                                FullSizeWidth= 3708
                                ThumbnailHeight= 150
                                ThumbnailWidth= 220

                                Author= None
                                Description= None
                                Copyright= None
                                MoreInforamtionLink= None


                                IsOwnedByCurrentUser= false
                                SubmissionDate= DateTimeOffset.Now
                                Submitter= "#1 User"
                                SubmitterId= "1"
                            } ] 
        
    section [ Class "section pt-0 is-relative" ]
      [ yield! (leftHalfPageImageRotation homeImageRotation)
        (navBar dispatch)
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
                                  [ str "Related Blog Posts" ] ] ] ] ] ] ]