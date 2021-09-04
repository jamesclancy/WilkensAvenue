module Pages.LocationDetails

open Contracts
open Feliz
open Feliz.Bulma
open SharedComponents
open System
open Shared
open Shared.DataTransferFormats
open Feliz.Quill

let locationDetailView (model: LocationDetailModel) (dispatch: Msg -> unit) =

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
                    Bulma.section [ Bulma.title "Location"
                                    Html.p [ prop.children [ yield! tryReturnString a.Address1
                                                             yield! tryReturnString a.Address2
                                                             yield! tryReturnString a.Address3
                                                             yield! tryReturnString a.City
                                                             yield! tryReturnString a.State
                                                             yield! tryReturnString a.Zipcode

                                                             yield Html.span (sprintf "Latitude: %f" a.Latitude)
                                                             yield Html.span (sprintf "Longitude: %f" a.Longitude) ] ] ]
            }

    let displayImages images =
        match images with
        | None -> Seq.empty
        | Some x ->
            seq {
                yield
                    Bulma.section [ prop.children [ yield Bulma.title "Images"
                                                    yield! buildListOfThumbnails x
                                                    yield Html.span "Upload your own photos!" ] ]
            }

    let rec editablePageSection (sectionTitle: string) previousValue boolEditingEnabled =
      match previousValue with
      | None -> editablePageSection sectionTitle (Some "") boolEditingEnabled
      | Some s ->
        if not boolEditingEnabled then 
            seq {
                yield
                    Bulma.section [ prop.children [ Bulma.title [ prop.children [ Html.span [prop.text sectionTitle]; Html.i [ prop.className "fas fa-edit is-pulled-right" ]  ] ]
                                                    Html.p [ prop.text s ] ] ]
            }
        else
                 seq {
                            yield
                                Bulma.section [ prop.children [ Bulma.title [ prop.text sectionTitle  ]
                                                                Quill.editor [
                                                                    editor.toolbar [
                                                                        [ Header (ToolbarHeader.Dropdown [1..4]) ]
                                                                        [ ForegroundColor; BackgroundColor ]
                                                                        [ Bold; Italic; Underline; Strikethrough; Blockquote; Code ]
                                                                        [ OrderedList; UnorderedList; DecreaseIndent; IncreaseIndent; CodeBlock ]
                                                                    ]
                                                                    editor.defaultValue s
                                                                    editor.onTextChanged (fun x -> Console.WriteLine(x))
                                                                ]
                                                                Bulma.button.button [
                                                                    button.isLarge
                                                                    prop.text "Save"
                                                                ] ] ]
                        }

    let pageContent (model: LocationDetailModel) =
        [ Bulma.title [ title.is1
                        prop.className "mb-5"
                        prop.text model.Name ]
          Bulma.title [ title.is2
                        prop.className "has-text-grey "
                        prop.text model.SubTitle ]
          yield! editablePageSection "Summary" model.Summary true
          yield! editablePageSection "Description" model.Description false
          yield! addressSection model.Address
          yield! displayImages model.Images
          yield!
              modifyTextInParagraphOrYieldNothing
                  "has-text-grey mb-5"
                  (fun x -> "Description from: " + x)
                  model.DescriptionCitation
          Html.div [ prop.className "buttons"
                     prop.children [ Html.a [ prop.className "button is-primary"
                                              prop.href "https://github.com/jamesclancy/WilkensAvenue"
                                              prop.text "Find Directions" ]
                                     Html.a [ prop.className "button is-primary"
                                              prop.href "https://github.com/jamesclancy/WilkensAvenue"
                                              prop.text "Upload Images" ]
                                     Html.a [ prop.className "button is-primary"
                                              prop.href "https://github.com/jamesclancy/WilkensAvenue"
                                              prop.text "Submit Update to Description" ] ] ] ]

    halfPageImagePage model.Images (pageContent model) dispatch
