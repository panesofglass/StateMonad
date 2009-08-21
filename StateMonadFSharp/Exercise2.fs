module Exercise2
  open System
  open Base
  open Exercise1_2
  
  (* In this exercise, I plan to use the builder form of the state monad.
     It's simpler to read and takes a simpler form of the incrementer.
     Besides, if I don't I'm basically just rewriting the C# version! *)  
  
  type Rect =
    { Height: int;
      Width: int;
      Top: int;
      Left: int }
    with override r.ToString() = String.Format("Height: {0}, Width: {1}, Top: {2}, Left: {3}", r.Height, r.Width, r.Top, r.Left)
  
  (* StateBuilder Bounder *)
  let boundTree tree seed leftBounder rightBounder =
    let rec labelTree t bounder =
      match t with
      | Leaf(c)      -> state { let! s = getState
                                do! setState (bounder s)
                                return Leaf(s, c) }

      | Branch(l, r) -> state { let! l = labelTree l leftBounder
                                let! r = labelTree r rightBounder
                                return Branch(l, r) }

    exec (labelTree tree leftBounder) seed