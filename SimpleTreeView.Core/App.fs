module SimpleTreeView.Core

open Serilog
open Microsoft.Extensions.Logging
open Elmish.WPF

type Person = {Id: int; Name: string}
type Tree = {Node: Person; Childrens: Tree list}

type Model = {
    Tree: Tree
    PersonId: int option
}

let init () =
    let tree = 
        {
            Node = {Id = 0; Name = "Root"}
            Childrens = [
                {
                    Node = {Id = 1; Name = "Mark"}
                    Childrens = []
                }
                {
                    Node = {Id = 2; Name = "Simon"}
                    Childrens = [
                        {
                            Node = {Id = 4; Name = "Jane"}
                            Childrens = []
                        }
                        {
                            Node = {Id = 5; Name = "Sean"}
                            Childrens = []
                        }
                    ]
                }
                {
                    Node = {Id = 3; Name = "Barry"}
                    Childrens = []
                }
            ]
        }
    {Tree = tree; PersonId = None}

type Msg =
    | Select of int option

let update msg m =
    match msg with
    | Select id -> {m with PersonId = id}

let bindings () : Binding<Model, Msg> list = [
    "Persons" |> Binding.subModelSeq(
        (fun m -> m.Tree.Childrens),
        (fun e -> e.Node.Id),
        (fun () -> [
            "Name" |> Binding.oneWay (fun (_, e) -> e.Node.Name)
            "Person" |> Binding.subModelSeq(
                (fun m -> (snd m).Childrens),
                (fun e -> e.Node.Id),
                (fun () -> [
                    "Name" |> Binding.oneWay (fun (_, e) -> e.Node.Name)
                ])
            )
        ])
    )

    "SelectPerson" |> Binding.subModelSelectedItem("Persons", (fun m -> m.PersonId), Select)
]

let mainDesignVm = ViewModel.designInstance (init ()) (bindings ())

let main mainWindow =

  let logger =
    LoggerConfiguration()
      .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
      .WriteTo.Console()
      .CreateLogger()

  WpfProgram.mkSimple init update bindings
      |> WpfProgram.withLogger ((new LoggerFactory()).AddSerilog(logger))
      |> WpfProgram.startElmishLoop mainWindow
