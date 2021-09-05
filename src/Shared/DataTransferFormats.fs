module Shared.DataTransferFormats

open System

type AddressDetailModel =
    { Address1: Option<string>
      Address2: Option<string>
      Address3: Option<string>
      City: Option<string>
      State: Option<string>
      Zipcode: Option<string>

      Longitude: decimal
      Latitude: decimal }

type DetailImageModel =
    { Id: string
      Name: string

      Order: int

      IsCurrentlySelected: bool

      FullSizeUrl: string
      ThumbnailUrl: string

      FullSizeHeight: int
      FullSizeWidth: int
      ThumbnailHeight: int
      ThumbnailWidth: int

      Author: Option<string>
      Description: Option<string>
      Copyright: Option<string>
      MoreInforamtionLink: Option<string>


      IsOwnedByCurrentUser: bool
      SubmissionDate: DateTimeOffset
      Submitter: string
      SubmitterId: string }

type LocationDetailModel =
    { Id: string
      Name: string
      SubTitle: string

      NeighborhoodId: string
      Neighborhood: string

      Summary: Option<string>
      Description: Option<string>
      DescriptionCitation: Option<string>

      Categories: string list option
      Tags: string list option

      Address: Option<AddressDetailModel>

      Images: DetailImageModel list option }

type LocationDistanceFilter =
    { MaxDistance: decimal
      MilesFromZipCode: decimal
      OriginZipCode: string }

type LocationSearchRequest =
    { CurrentPage: int
      ItemsPerPage: int
      Query: string

      FilterToFree: bool
      FilterToOpenAir: bool
      FilterToPrivate: bool

      NeighborhoodFilter: string list option
      TagFilterFilter: string list option
      CategoryFilter: string list option
      DistanceFilter: LocationDistanceFilter option }

let emptySearchRequest =
    { CurrentPage = 1
      ItemsPerPage = 50
      Query = String.Empty

      FilterToFree = false
      FilterToOpenAir = false
      FilterToPrivate = false

      NeighborhoodFilter = None
      TagFilterFilter = None
      CategoryFilter = None
      DistanceFilter = None }

type LocationSummaryViewModel =
    { Id: string
      NeighborhoodId: string
      Neighborhood: string
      Name: string
      SubTitle: string

      Summary: Option<string>

      Address: Option<AddressDetailModel>

      ThumbnailUrl: string
      ThumbnailHeight: int
      ThumbnailWidth: int }

type LocationSearchResult =
    { SearchRequest: LocationSearchRequest

      TotalResults: int
      TotalPages: int
      CurrentPage: int

      Results: LocationSummaryViewModel list option }

type LocationDetailUpdateRequest =
    { Id: string
      Summary: string
      Description: string }

type LocationDetailUpdateResult = { ErrorMessage: string option }
