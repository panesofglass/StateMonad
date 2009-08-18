// State Monad
module Exercise1
  open Base
  
  (* StateMonad *)
  type State<'S, 'a> = State of ('S -> 'S * 'a)

  type StateBuilder() =
    member m.Bind sm f = State (fun s0 -> let (s1, a1) = match sm with | State g -> g s0
                                          let (s2, a2) = match f a1 with | State h -> h s1
                                          (s2, a2))
    member m.Return(a) = State (fun s -> s, a)

  let state = StateBuilder()

  let GetState = State (fun s -> s, s)
  let SetState s = State (fun _ -> s, ())

  let Eval sm s =
    match sm with
    | State f -> f s |> fst

  let Exec sm s =
    match sm with
    | State f -> f s |> snd

  (* Labeller *)
  let labelTreeWithStateMonad tree initialState incrementer =
    let rec makeMonad t incrementer =
      match t with
      | Leaf(c)      -> state { let! x = GetState
                                do! SetState (incrementer s)
                                return Leaf((s, c)) }

      | Branch(l, r) -> state { let! l = makeMonad l incrementer
                                let! r = makeMonad r incrementer
                                return Branch(l, r) }

    Exec (makeMonad tree incrementer)