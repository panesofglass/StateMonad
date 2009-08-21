module Exercise1_2
  open Base
  
  (* StateBuilder *)
  type StateBuilder() =
    member m.Delay (f)    = State (fun s -> match (f()) with | State h -> h s)
    member m.Bind (sm, f) = State (fun s0 -> let (s1, a1) = match sm with | State h -> h s0
                                             let (s2, a2) = match f a1 with | State h1 -> h1 s1
                                             (s2, a2))
    member m.Return (a)   = State (fun s -> s, a)

  let state = StateBuilder()

  let getState   = State (fun s -> s, s)
  let setState s = State (fun _ -> s, ())

  (* StateBuilder Labeller *)
  let labelTreeWithStateBuilder tree seed incrementer =
    let rec labelTree t incrementer =
      match t with
      | Leaf(c)      -> state { let! s = getState
                                do! setState (incrementer s)
                                return Leaf(s, c) }

      | Branch(l, r) -> state { let! l = labelTree l incrementer
                                let! r = labelTree r incrementer
                                return Branch(l, r) }

    exec (labelTree tree incrementer) seed