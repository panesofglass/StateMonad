// State Monad
module Exercise1
  open Base
  
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

  let labelTreeWithStateMonad tree initialState inputMaker =
    let rec makeMonad t incrementer =
      match t with
      | Leaf(contents) ->
          inputMaker () >>= fun n -> Return (Leaf((n, contents)))
      | Branch(oldLeft, oldRight) ->
          makeMonad oldLeft incrementer >>=
            fun newLeft -> makeMonad oldRight incrementer >>=
                             fun newRight -> Return (Branch(newLeft, newRight))
    let (newState, labeledTree) =
      match makeMonad tree inputMaker with
      | State f -> f initialState
    labeledTree