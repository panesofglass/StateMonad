// State Monad

let indentation = 2

type Tree<'a> =
  | Leaf of 'a
  | Branch of Tree<'a> * Tree<'a>

// The show method already takes into account the tuples,
// unlike the show method that is overloaded in the C# version.
let show tree =
  let rec printTree tree label =
    let spacing = new string(' ', label * indentation)
    printf "%A" spacing
    match tree with
    | Leaf((label, contents)) ->
        printf "Leaf: %A, " label
        printfn "Contents: %A" <| contents.ToString()
    | Branch(left, right) ->
        printfn "Branch: "
        printTree left (label + 1)
        printTree right (label + 1)
  printTree tree 0

// Non-monadic labeller
let labelTreeWithoutMonad tree label incrementer =
  let rec labelTree tree label incrementer =
    match tree with
    | Leaf(contents) ->
        (incrementer label, Leaf((label, contents)))
    | Branch(left, right) ->
        let l = labelTree left label incrementer
        let r = labelTree right (fst l) incrementer
        (fst r, Branch((snd l), (snd r)))
  let (newState, labeledTree) =
    labelTree tree label incrementer
  labeledTree

// Monadic labeller
type State<'S, 'a> = State of ('S -> 'S * 'a)

type StateMonad() =
  static member Return(a) = State (fun s -> s, a)
  static member Bind sm f =
    State (fun s0 ->
             let (s1, a1) = match sm with | State h -> h s0
             let (s2, a2) = match f a1 with | State h1 -> h1 s1
             (s2, a2))

let (>>=) sm f = StateMonad.Bind sm f
let Return = StateMonad.Return

let labelTreeWithStateMonad tree initialState incrementer =
  let rec makeMonad t incrementer =
    match t with
    | Leaf(contents) ->
        incrementer () >>= fun n -> Return (Leaf((n, contents)))
    | Branch(oldLeft, oldRight) ->
        makeMonad oldLeft incrementer >>=
          fun newLeft -> makeMonad oldRight incrementer >>=
                           fun newRight -> Return (Branch(newLeft, newRight))
  let (newState, labeledTree) =
    match makeMonad tree incrementer with
    | State f -> f initialState
  labeledTree

// Test it out
let demoTree = Branch(Leaf("A"), Branch(Leaf("B"),Branch(Leaf("C"),Leaf("D"))))
let initialState = 0

printfn "Non-monadically labeled tree"
let incrementer = fun n -> n + 1
let labeledTree = labelTreeWithoutMonad demoTree initialState incrementer
show labeledTree

printfn ""
printfn "Monadically labeled tree"
let monadicIncrementer = fun () -> State(fun n -> n + 1, n)
let mTree = labelTreeWithStateMonad demoTree initialState monadicIncrementer
show mTree