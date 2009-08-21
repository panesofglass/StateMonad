module Exercise1_1
  open Base
  
  type StateMonad() =
    static member Bind sm f = State (fun s0 -> let (s1, a1) = match sm with | State h -> h s0
                                               let (s2, a2) = match f a1 with | State h1 -> h1 s1
                                               (s2, a2))
    static member Return(a) = State (fun s -> s, a)
 
  let (>>=) sm f = StateMonad.Bind sm f
  let Return = StateMonad.Return
 
  (* Static StateMonad Labeller *)
  let labelTreeWithStaticStateMonad tree seed incrementer =
    let rec labelTree t incrementer =
      match t with
      | Leaf(c)      -> incrementer () >>= fun s -> Return (Leaf((s, c)))
      | Branch(l, r) -> labelTree l incrementer >>=
                          fun newLeft -> labelTree r incrementer >>=
                                           fun newRight -> Return (Branch(newLeft, newRight))
                             
    exec (labelTree tree incrementer) seed