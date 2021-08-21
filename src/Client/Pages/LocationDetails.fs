module Pages.LocationDetails

open Contracts
open Feliz
open Feliz.Bulma
open Fable.React
open Fable.React.Props
open SharedComponents
open System
open Shared



let locationDetailView model (dispatch: Msg -> unit) =

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
                    section [ Class "section" ] [
                        h1 [ Class "title" ] [ str "Location" ]
                        p [] [
                            yield! tryReturnString a.Address1
                            yield! tryReturnString a.Address2
                            yield! tryReturnString a.Address3
                            yield! tryReturnString a.City
                            yield! tryReturnString a.State
                            yield! tryReturnString a.Zipcode

                            yield str (sprintf "Latitude: %f" a.Latitude)
                            yield str (sprintf "Longitude: %f" a.Longitude)
                        ]
                    ]
            }

    let displayImages images =
        match images with
        | None -> Seq.empty
        | Some x ->
            seq {
                yield
                    section [ Class "section" ] [
                        yield h1 [ Class "title" ] [ str "Images" ]
                        yield! buildListOfThumbnails x
                        yield str "Upload your own photos!"
                    ]
            }



    section [ Class "section pt-0 is-relative" ] [
        yield! (leftHalfPageImageRotation model.Images)
        (navBar dispatch)
        div [ Class "container" ] [
            div [ Class "pt-5 columns is-multiline" ] [
                div [ Class "column is-12 is-5-desktop ml-auto" ] [
                    div [ Class "mb-5" ] [
                        h2 [ Class "mb-5 is-size-1 is-size-3-mobile has-text-weight-bold" ] [
                            str model.Name
                        ]
                        p [ Class "subtitle has-text-grey mb-5" ] [
                            str model.SubTitle
                        ]
                        yield! sectionOrYieldNothing "" "Summary" model.Summary
                        yield! sectionOrYieldNothing "" "Description" model.Description
                        yield! addressSection model.Address
                        yield! displayImages model.Images
                        yield!
                            modifyTextInParagraphOrYieldNothing
                                "has-text-grey mb-5"
                                (fun x -> "Description from: " + x)
                                model.DescriptionCitation
                        div [ Class "buttons" ] [
                            a [ Class "button is-primary"
                                Href "https://github.com/jamesclancy/WilkensAvenue" ] [
                                str "Directions"
                            ]
                            a [ Class "button is-primary"
                                Href "https://github.com/jamesclancy/WilkensAvenue" ] [
                                str "Upload Images"
                            ]
                            a [ Class "button is-primary"
                                Href "https://github.com/jamesclancy/WilkensAvenue" ] [
                                str "Submit Update to Description"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
