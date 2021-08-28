namespace Shared

open System
open Shared.DataTransferFormats

type Todo = { Id: Guid; Description: string }

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) =
        { Id = Guid.NewGuid()
          Description = description }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let builderWithoutApiPrefix typeName methodName = sprintf "/%s/%s" typeName methodName

type ITodosApi =
    { getLocation: string -> Async<LocationDetailModel>
      searchLocations: LocationSearchRequest -> Async<LocationSearchResult> }
