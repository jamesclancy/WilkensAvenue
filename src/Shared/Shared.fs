namespace Shared

open System
open Shared.DataTransferFormats

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let builderWithoutApiPrefix typeName methodName = sprintf "/%s/%s" typeName methodName

type ILocationInformationApi =
    { getLocation: string -> Async<LocationDetailModel>
      searchLocations: LocationSearchRequest -> Async<LocationSearchResult> }
