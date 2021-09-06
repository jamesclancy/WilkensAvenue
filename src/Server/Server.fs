module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open Shared.DataTransferFormats
open System
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http

open Authentication
open Configuration

let exampleLocation =
    { Id = "1"
      Name = "Carrollton Viaduct"
      SubTitle = "America's oldest railroad bridge"

      Summary =
          Some
              "The Carrollton Viaduct, located over the Gwynns Falls stream near Carroll Park in southwest Baltimore, Maryland, is the first stone masonry bridge built for railroad use in the United States for the Baltimore and Ohio Railroad, founded 1827, with construction beginning the following year and completed 1829. The bridge is named in honor of Charles Carroll of Carrollton (1737-1832), of Maryland, known for being the last surviving signer of the Declaration of Independence, the only Roman Catholic in the Second Continental Congress (1775-1781), and wealthiest man in the Thirteen Colonies of the time of the American Revolutionary War (1775-1783)."
      Description =
          Some
              """The bridge is currently one of the world's oldest railroad bridges still in use for rail traffic, carrying loads far greater than originally envisioned.[3][4] It was named after Charles Carroll of Carrollton (1737-1832), the last living signer of the Declaration of Independence and a director of the Baltimore and Ohio Railroad, who laid the cornerstone on July 4, 1828.[5] As he laid the first stone he said, "I consider this among the most important acts of my life, second only to my signing the Declaration of Independence." Builder Caspar Wever and designer James Lloyd completed the structure for the railroad in November 1829, at an officially listed cost of $58,106.73. The actual cost of the construction may have been as high as $100,000.[6]

        The bridge, 312 feet (95 m) in length, rises from its foundations about 65 feet (20 m). It is 51 feet 9 inches (15.8 m) above Gwynns Falls. It consists of a full-centered arch with a clear span length of 80 feet (24 m) over the stream, and a space for two railroad tracks on its deck. To provide an underpass for a wagon road, an arched passageway, 16 feet (5 m) in width, was built through one of the masonry-walled approaches. Originally planned as one arch of 40 feet (12 m) chord, the dimensions were enlarged to quiet the concern of the proprietor of the mills located immediately above the bridge site, who feared that 40 feet would be insufficient if the stream was flooded. The heavy granite blocks which form the arches and exterior walls were procured from Ellicott's Mills and Port Deposit.[7] A temporary wooden framework supporting the central span held 1,500 tons (1,360 tonnes) of this stone during construction. A white cornerstone at one end of the bridge bears the inscription "James Lloyd of Maryland, Builder A.D. 1829."

        Andrew Jackson, the first President of the United States to ride on a railroad train, crossed the bridge on a trip between Ellicott's Mills and Baltimore on June 6, 1833. The Carrollton Viaduct has provided continual service to the Baltimore and Ohio Railroad and its modern corporate successor, CSX Transportation.

        The viaduct was designated a National Historic Landmark on November 11, 1971 and was automatically listed on the National Register of Historic Places the same day.[2][8]

        In 1982 the viaduct was designated a Historic Civil Engineering Landmark by the American Society of Civil Engineers."""
      DescriptionCitation = Some "https://en.wikipedia.org/wiki/Carrollton_Viaduct"

      NeighborhoodId = "1"
      Neighborhood = "Pigtown"

      Categories = None
      Tags = None

      Address =
          Some
              { Address1 = Some "123 Wilkens Ave"
                Address2 = None
                Address3 = None
                City = Some "Baltimore"
                State = Some "MD"
                Zipcode = Some "21223"
                Longitude = -76.655M
                Latitude = 39.275277777777774M }


      Images =
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
                   SubmitterId = "1" }
                 { Id = "2"
                   Name = "From wikipedia"
                   Order = 2
                   IsCurrentlySelected = false

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
                   SubmitterId = "1" } ] }


let generateABunchOfItems =
    [ 1 .. 16 ]
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
                        Longitude = -76.655M
                        Latitude = 39.275277777777774M } })


let locationInformationApi =
    { getLocation = fun id -> async { return exampleLocation }
      searchLocations =
          fun req ->
              async {
                  return
                      { SearchRequest = req
                        TotalResults = 90
                        TotalPages = Convert.ToInt32(Math.Ceiling(90m / 16m))
                        CurrentPage = req.CurrentPage
                        Results = Some(List.ofSeq generateABunchOfItems) }
              }
      updateLocationDetails = fun req -> async { return { ErrorMessage = None } } }

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builderWithoutApiPrefix
    |> Remoting.fromValue locationInformationApi
    |> Remoting.buildHttpHandler

let buildApi next ctx =
    task {
        let handler =
            Remoting.createApi ()
            |> Remoting.withRouteBuilder Route.builderWithoutApiPrefix
            |> Remoting.fromValue locationInformationApi
            |> Remoting.buildHttpHandler

        return! handler next ctx
    }

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/loggedinhomepage"
             >=> (authChallenge
                  >=> htmlString "You are logged in now.")
             subRoute "/api" ( (*authChallenge >=> *) buildApi) ]

let getPort =
    let envPort =
        Environment.GetEnvironmentVariable("PORT")

    if String.IsNullOrWhiteSpace(envPort) then
        "8085"
    else
        envPort

let app =
    application {
        url (sprintf "http://+:%s" getPort)
        host_config configureHost
        use_router routes
        memory_cache
        use_static "public"
        use_gzip
        use_open_id_auth_with_config_from_service_collection openIdConfig
        app_config addAuth
    }

run app
