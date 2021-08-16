module SharedComponents

open Contracts
open Feliz
open Feliz.Bulma
open Fable.React
open Fable.React.Props
open Shared
open Fulma
open Fulma.Extensions.Wikiki


let loadingScreen =
    PageLoader.pageLoader [ PageLoader.Color IsLight
                            PageLoader.IsActive true ] [ span [ Class "title has-text-centered" ]
                                                                                [ str "Loading..."
                                                                                  br []
                                                                                   ] ]

let navBar (dispatch: Msg -> unit) =
    div [ Class "is-relative" ]
      [ nav [ Class "navbar py-4"
              Style [ BackgroundColor "transparent" ] ]
          [ div [ Class "navbar-brand" ]
              [ a [ Class "navbar-item is-hidden-touch"
                    Href "#" ]
                  [ h1 [ Class "title is-1 text has-text-primary"
                         Style [ BackgroundColor "rgba(255, 255, 255, 0.8)"
                                 Padding "10px"
                                 BorderRadius "25px" ] ]
                      [ str "Wilkens Avenue" ] ]
                a [ Class "navbar-item is-hidden-desktop"
                    Href "#" ]
                  [ h1 [ Class "title is-1 text has-text-primary"
                         Style [ BackgroundColor "rgba(255, 255, 255, 0.8)"
                                 Padding "10px"
                                 BorderRadius "25px" ] ]
                      [ str "Wilkens Avenue" ] ]
                a [ Class "navbar-burger"
                    Role "button"
                    HTMLAttr.Custom ("aria-label", "menu")
                    AriaExpanded false ]
                  [ span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                      [ ]
                    span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                      [ ]
                    span [ HTMLAttr.Custom ("aria-hidden", "true") ]
                      [ ] ] ]
            div [ Class "navbar-menu" ]
              [ div [ Class "navbar-end" ]
                  [ a [ Class "navbar-item"
                        Href "#/" ]
                      [ str "Home" ]
                    a [ Class "navbar-item"
                        Href "#/viewlocation/12312" ]
                      [ str "Find" ]
                    a [ Class "navbar-item"
                        Href "#/viewlocation/12312" ]
                      [ str "Browse" ]
                    a [ Class "navbar-item"
                        Href "#" ]
                      [ str "Login" ]
                    a [ Class "navbar-item"
                        Href "#" ]
                      [ str "Register" ] ] ] ] ]

                      
let sectionOrYieldNothing cls title optionalText =
    match optionalText with
    | None -> Seq.empty
    | Some x -> seq { yield section [ Class "section" ]
            [
                h1 [ Class "title" ] [ str title ]
                p [Class cls] [ str x ]
                ] }


let modifyTextInParagraphOrYieldNothing cls f optionalText =
    match optionalText with
    | None -> Seq.empty
    | Some x -> seq { yield p [ Class cls ] [ str (f x) ] }

let paragraphOrYieldNothing cls optionalText =
    modifyTextInParagraphOrYieldNothing cls id optionalText

let imageThumbnail image =
  article [ Class "media" ]
      [ figure [ Class "media-left" ]
          [ p [ Class "image is-128x128" ]
              [ img [ Src image.ThumbnailUrl ] ] ]
        div [ Class "media-content" ]
          [ div [ Class "content" ]
              [ p [ ]
                  [ strong [ ] [ str image.Submitter ]
                    br [ ]
                    yield! paragraphOrYieldNothing "" image.Description
                    br [ ]
                    small [ ] [ str (image.SubmissionDate.ToString("d")) ] ] ] ] ]


let buildListOfThumbnails images = images |> List.map imageThumbnail |> Seq.ofList

let leftHalfPageImageRotation (imageList :  DetailImageModel list option) =

  let mainImage src alt = img [ Class "is-hidden-touch image is-fullwidth"
                                Style [
                                    Position PositionOptions.Absolute
                                    Top "0"
                                    Bottom "0"
                                    Left "0"
                                    ObjectFit "cover"
                                    Height "100%"
                                    Width "50%" ]
                                Src src
                                Alt alt ]
    
  let childImageContainer images =
    footer  [ Class "is-footer" ]
        [ div [ Class "pt-5 columns is-multiline" ]
             [ yield! buildListOfThumbnails images ] ]

  seq {
    match imageList with
    | None | Some []  ->
        yield (mainImage "https://via.placeholder.com/3708x2950?text=Image+Not+Available" "")
    | Some [ x ] ->
          yield mainImage x.FullSizeUrl x.Name
    | Some (x :: xs)  ->
          yield mainImage x.FullSizeUrl x.Name
   }