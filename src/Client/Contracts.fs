module Contracts

open Shared
open System 

type PageModel = 
    HomePageModel
  | FindPageModel of string * string * int
  | BrowsePageModel of string
  | AddLocationPageModel 
  | YourLocationsPageModel
  | LoginPageModel
  | RegisterPageModel
  | LogoutPageModel
  | YourAccountPageModel
  | EditLocationPageModel of string
  | ViewLocationPageModel of LocationDetailModel
  | AboutPageModel
  | NotFound
  | Unauthorized

type Model =
    {
       CurrentRoute: Option<string>
       PageModel: PageModel
       CurrentUser: Option<string>
    }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo
    | RecievedLocationDetail of LocationDetailModel

type ClientRoute =
   | Home
   | Find of string * string * int
   | Browse of string
   | AddLocation 
   | YourLocations
   | Login
   | Register
   | Logout
   | YourAccount
   | EditLocation of string
   | ViewLocation of string
   | About