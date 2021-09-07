module SharedComponents

open Contracts
open Feliz
open Feliz.Bulma
open Shared.DataTransferFormats


let loadingScreen =
    PageLoader.pageLoader [ pageLoader.isSuccess
                            pageLoader.isActive
                            prop.children [ PageLoader.title "Loading..." ] ]

let navBar (model: Model) (dispatch: Msg -> unit) =

    let burgerActiveClass =
        seq {
            if model.MenuBurgerExpanded then
                "is-active"
            else
                ""
        }

    Html.div [ prop.children [ Bulma.navbar [ color.isLight
                                              navbar.isTransparent
                                              navbar.hasShadow
                                              navbar.isTransparent
                                              prop.style []

                                              prop.children [ Bulma.navbarBrand.div [ Bulma.navbarItem.a [ prop.href
                                                                                                               "#/"
                                                                                                           prop.children [ Html.h1 [ prop.text
                                                                                                                                         "Wilkens Avenue"
                                                                                                                                     prop.className
                                                                                                                                         "title navBarHeader" ] ] ]

                                                                                      Bulma.navbarBurger [ prop.custom (
                                                                                                               "data-target",
                                                                                                               "nav-menu"
                                                                                                           )
                                                                                                           prop.classes
                                                                                                               burgerActiveClass
                                                                                                           prop.onClick
                                                                                                               (fun _ ->
                                                                                                                   ToggleBurger
                                                                                                                   |> dispatch)
                                                                                                           prop.children [ Html.span [  ]
                                                                                                                           Html.span [  ]
                                                                                                                           Html.span [  ] ] ] ]

                                                              Bulma.navbarMenu [ prop.id "nav-menu"
                                                                                 prop.classes burgerActiveClass
                                                                                 prop.children [ Bulma.navbarStart.div [ Bulma.navbarItem.a [ prop.text
                                                                                                                                                  "Home"
                                                                                                                                              prop.href
                                                                                                                                                  "#/" ]
                                                                                                                         Bulma.navbarItem.a [ prop.text
                                                                                                                                                  "Find"
                                                                                                                                              prop.href
                                                                                                                                                  "#/browse/12312" ]
                                                                                                                         Bulma.navbarItem.a [ prop.text
                                                                                                                                                  "Browse"
                                                                                                                                              prop.href
                                                                                                                                                  "#/browse/12312" ] ]
                                                                                                 Bulma.navbarEnd.div [ Bulma.navbarItem.div [ Bulma.buttons [ Bulma.button.a [ Html.strong
                                                                                                                                                                                   "Login" ]
                                                                                                                                                              Bulma.button.a [ prop.text
                                                                                                                                                                                   "Register" ] ] ] ] ] ] ] ] ] ]



let sectionOrYieldNothing (cls: string) (title: string) (optionalText: string option) =
    match optionalText with
    | None -> Seq.empty
    | Some x ->
        seq {
            yield
                Bulma.section [ prop.children [ Bulma.title [ prop.text title ]
                                                Html.p [ prop.className cls
                                                         prop.text x ] ] ]
        }


let modifyTextInParagraphOrYieldNothing (cls: string) (f: string -> string) (optionalText: string option) =
    match optionalText with
    | None -> Seq.empty
    | Some x ->
        seq {
            yield
                Html.p [ prop.className cls
                         prop.text (f x) ]
        }

let paragraphOrYieldNothing cls optionalText =
    modifyTextInParagraphOrYieldNothing cls id optionalText

let imageThumbnail (image: DetailImageModel) =
    Bulma.media [ Bulma.mediaLeft [ Bulma.image [ Bulma.image.is64x64
                                                  prop.children [ Html.img [ prop.src image.ThumbnailUrl ] ] ] ]
                  Bulma.mediaContent [ Bulma.content [ Html.p [ Html.strong image.Submitter
                                                                Html.br []
                                                                yield! paragraphOrYieldNothing "" image.Description
                                                                Html.br []
                                                                Html.small (image.SubmissionDate.ToString("d")) ] ] ] ]



let buildListOfThumbnails images =
    images |> List.map imageThumbnail |> Seq.ofList

let leftHalfPageImageRotation (imageList: DetailImageModel list option) =
    let mainImage src alt =
        Html.img [ prop.className "is-hidden-touch image halfScreenImage"
                   prop.src src
                   prop.alt alt ]

    seq {
        match imageList with
        | None
        | Some [] -> yield (mainImage "https://via.placeholder.com/3708x2950?text=Image+Not+Available" "")
        | Some [ x ] -> yield mainImage x.FullSizeUrl x.Name
        | Some (x :: xs) -> yield mainImage x.FullSizeUrl x.Name
    }

let leftHalfPageImageRotationRightContent innerContent =
    Bulma.container [ container.isFluid
                      prop.children [ Bulma.columns [ prop.className "pt-5"
                                                      columns.isMultiline
                                                      columns.isGapless
                                                      prop.children [ Bulma.column [ column.is12
                                                                                     column.is5Desktop
                                                                                     prop.className "ml-auto"
                                                                                     prop.children [ yield! innerContent ] ] ] ] ] ]

let halfPageImagePage
    (model: Model)
    (imageList: DetailImageModel list option)
    formatedRightHandContent
    (dispatch: Msg -> unit)
    =
    Bulma.section [ prop.classes [ "pt-0"; "is-relative" ]
                    prop.children [ yield! (leftHalfPageImageRotation imageList)
                                    (navBar model dispatch)
                                    leftHalfPageImageRotationRightContent formatedRightHandContent ] ]
