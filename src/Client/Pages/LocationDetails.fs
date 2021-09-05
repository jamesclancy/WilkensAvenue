module Pages.LocationDetails

open Contracts
open Feliz
open Feliz.Bulma
open SharedComponents
open System
open Shared
open Shared.DataTransferFormats
open Feliz.Quill
open Elmish


let stringEmptyOrValue (s: string option) =
    Option.fold (fun x y -> x) String.Empty s

let defaultEditState (model: LocationDetailModel) : UpdateLocationDetailState =
    { EditingSummary = false
      NewSummaryContent = stringEmptyOrValue model.Summary
      EditingDescription = false
      NewDescriptionContent = stringEmptyOrValue model.Description
      ErrorMessage = None }

let updateLocationDetailsModel
    (d: LocationDetailUpdate)
    (currPage: LocationDetailModel)
    (editState: UpdateLocationDetailState)
    updateMethod
    =

    let buildCmdForSave =
        Cmd.OfAsync.perform
            updateMethod
            (ContractMappings.mapLocationDetailToEditRequest currPage.Id editState)
            (ContractMappings.mapUpdateResultsToUpdateDetailResponseReceived d currPage editState)


    match d with
    | SummaryStartEdit -> currPage, { editState with EditingSummary = true }, Cmd.none
    | SummaryTextChanged s ->
        Console.WriteLine(s)
        currPage, { editState with NewSummaryContent = s }, Cmd.none
    | SummaryTextSaved ->
        { currPage with
              Summary = Some editState.NewSummaryContent },
        { editState with
              EditingSummary = false }, buildCmdForSave
    | SummaryTextChangeCanceled ->
        currPage,
        { editState with
              EditingSummary = false
              NewSummaryContent = stringEmptyOrValue currPage.Summary }, Cmd.none
    | DescriptionStartEdit ->
        currPage,
        { editState with
              EditingDescription = true }, Cmd.none
    | DescriptionTextChanged s ->
        currPage,
        { editState with
              NewDescriptionContent = s }, Cmd.none
    | DescriptionTextSaved ->
        { currPage with
              Description = Some editState.NewDescriptionContent },
        { editState with
              EditingDescription = false }, buildCmdForSave
    | DescriptionTextChangeCanceled ->
        currPage,
        { editState with
              EditingDescription = false
              NewDescriptionContent = stringEmptyOrValue currPage.Description }, Cmd.none



let locationDetailView (model: LocationDetailModel) (editState: UpdateLocationDetailState) (dispatch: Msg -> unit) =

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

    let rec editablePageSection
        (sectionTitle: string)
        previousValue
        boolEditingEnabled
        (editFunction: string -> unit)
        (saveChanges: unit -> unit)
        (cancelChanges: unit -> unit)
        (startEditing: unit -> unit)
        =
        match previousValue with
        | None ->
            editablePageSection
                sectionTitle
                (Some "")
                boolEditingEnabled
                editFunction
                saveChanges
                cancelChanges
                startEditing
        | Some s ->
            if not boolEditingEnabled then
                seq {
                    yield
                        Bulma.section [ prop.children [ Bulma.title [ prop.children [ Html.span [ prop.text sectionTitle ]
                                                                                      Html.i [ prop.className
                                                                                                   "fas fa-edit is-pulled-right"
                                                                                               prop.onClick
                                                                                                   (fun x ->
                                                                                                       startEditing ()) ] ] ]
                                                        Html.p [ prop.innerHtml s ] ] ]
                }
            else
                seq {
                    yield
                        Bulma.section [ prop.children [ Bulma.title [ prop.text sectionTitle ]
                                                        Quill.editor [ editor.toolbar [ [ Header(
                                                                                              ToolbarHeader.Dropdown
                                                                                                  [ 1 .. 4 ]
                                                                                          ) ]
                                                                                        [ ForegroundColor
                                                                                          BackgroundColor ]
                                                                                        [ Bold
                                                                                          Italic
                                                                                          Underline
                                                                                          Strikethrough
                                                                                          Blockquote
                                                                                          Code ]
                                                                                        [ OrderedList
                                                                                          UnorderedList
                                                                                          DecreaseIndent
                                                                                          IncreaseIndent
                                                                                          CodeBlock ] ]
                                                                       editor.defaultValue s
                                                                       editor.onTextChanged (fun x -> editFunction x) ]
                                                        Bulma.field.div [ Bulma.field.hasAddons
                                                                          prop.children [ Bulma.control.p [ Bulma.button.button [ Bulma.color.isSuccess
                                                                                                                                  prop.onClick
                                                                                                                                      (fun x ->
                                                                                                                                          saveChanges
                                                                                                                                              ())
                                                                                                                                  prop.children [ Bulma.icon [ Html.i [ prop.className
                                                                                                                                                                            "fas fa-save" ] ]
                                                                                                                                                  Html.span [ prop.text
                                                                                                                                                                  "Save" ] ] ] ]
                                                                                          Bulma.control.p [ Bulma.button.button [ Bulma.color.isWarning
                                                                                                                                  prop.onClick
                                                                                                                                      (fun x ->
                                                                                                                                          cancelChanges
                                                                                                                                              ())
                                                                                                                                  prop.children [ Bulma.icon [ Html.i [ prop.className
                                                                                                                                                                            "fas fa-trash" ] ]
                                                                                                                                                  Html.span [ prop.text
                                                                                                                                                                  "Cancel" ] ] ] ] ] ] ] ]
                }

    let errorMessage (msg : string option) =
        match msg with
        | None -> Seq.empty
        | Some s -> seq {
            Bulma.notification [
                Bulma.color.isDanger
                prop.text s
                ]
            }

    let pageContent
        (model: LocationDetailModel)
        (editState: UpdateLocationDetailState)
        (dispatch: LocationDetailUpdate -> unit)
        =
        [ Bulma.title [ title.is1
                        prop.className "mb-5"
                        prop.text model.Name ]
          Bulma.title [ title.is2
                        prop.className "has-text-grey "
                        prop.text model.SubTitle ]
          yield! errorMessage editState.ErrorMessage
          yield!
              editablePageSection
                  "Summary"
                  model.Summary
                  editState.EditingSummary
                  (fun x -> x |> SummaryTextChanged |> dispatch)
                  (fun x -> SummaryTextSaved |> dispatch)
                  (fun x -> SummaryTextChangeCanceled |> dispatch)
                  (fun x -> SummaryStartEdit |> dispatch)
          yield!
              editablePageSection
                  "Description"
                  model.Description
                  editState.EditingDescription
                  (fun x -> x |> DescriptionTextChanged |> dispatch)
                  (fun x -> DescriptionTextSaved |> dispatch)
                  (fun x -> DescriptionTextChangeCanceled |> dispatch)
                  (fun x -> DescriptionStartEdit |> dispatch)
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
                                              prop.text "Upload Images" ] ] ] ]

    halfPageImagePage
        model.Images
        (pageContent model editState (fun x -> x |> LocationDetailUpdated |> dispatch))
        dispatch
