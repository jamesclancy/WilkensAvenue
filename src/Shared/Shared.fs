namespace Shared

open System

type Todo = { Id: Guid; Description: string }

type AddressDetailModel =
    {
         Address1: Option<string>
         Address2: Option<string>
         Address3: Option<string>
         City: Option<string>
         State: Option<string>
         Zipcode: Option<string>

         Longitude: decimal
         Latitude: decimal  
    }

type DetailImageModel =
    {
        Id: string
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
        SubmitterId: string
    }

type LocationDetailModel =
    {
        Id: string
        Name: string
        SubTitle: string

        Summary: Option<string>
        Description: Option<string>
        DescriptionCitation: Option<string>

        Address: Option<AddressDetailModel>

        Images: DetailImageModel list option
    }  



module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) =
        { Id = Guid.NewGuid()
          Description = description }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let builderWithoutApiPrefix typeName methodName =
        sprintf "/%s/%s" typeName methodName

type ITodosApi =
    { getTodos: unit -> Async<Todo list>
      addTodo: Todo -> Async<Todo>
      getLocation: string -> Async<LocationDetailModel>}
