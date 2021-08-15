module Client.Tests

open Fable.Mocha

open Index
open Shared
open Elmish.UrlParser
open Contracts

let runTestForRouteParse urlSlug (expectedResponse : Option<ClientRoute>) =
    let actual = parse clientRouter urlSlug Map.empty
    Expect.equal actual expectedResponse (sprintf "url slug parser failed slug:%s expected:%O actual:%O" urlSlug expectedResponse actual)

let client = testList "Client" [
    testCase "All routes working" <| fun _ ->
        runTestForRouteParse "about" (Some ClientRoute.About)
        runTestForRouteParse "find/str1/str2/2" (("str1", "str2", 2) |> ClientRoute.Find |> Some)
        runTestForRouteParse "browse/str1" ("str1" |> Browse |> Some)
        runTestForRouteParse "addlocation" (Some ClientRoute.AddLocation)
        runTestForRouteParse "yourlocations" (Some ClientRoute.YourLocations)
        runTestForRouteParse "login" (Some ClientRoute.Login)
        runTestForRouteParse "register" (Some ClientRoute.Register)
        runTestForRouteParse "logout" (Some ClientRoute.Logout)
        runTestForRouteParse "youraccount" (Some ClientRoute.YourAccount)
        runTestForRouteParse "editlocation/str1" ("str1" |> EditLocation |> Some)
        runTestForRouteParse "viewlocation/str1" ("str1" |> ViewLocation |> Some)
]

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all