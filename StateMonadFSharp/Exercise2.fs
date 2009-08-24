module Exercise2
  open System
  open Base
  open Exercise1_2
  
  (* In this exercise, I plan to use the builder form of the state monad.
     It's simpler to read and takes a simpler form of the incrementer.
     Besides, if I don't I'm basically just rewriting the C# version! *)  
  
  type Rect =
    { Height: float;
      Width: float;
      Top: float;
      Left: float }
    with override r.ToString() = String.Format("Height: {0}, Width: {1}, Top: {2}, Left: {3}", r.Height, r.Width, r.Top, r.Left)
  
  (* StateBuilder Bounder *)
  let boundTree tree seed leftUpdater rightUpdater =
    let rec labelTree t updater =
      match t with
      | Leaf(c)      -> state { let! s = getState
                                let (next, curr) = updater s
                                do! setState next
                                return Leaf(curr, c) }

      | Branch(l, r) -> state { let! l = labelTree l leftUpdater
                                let! r = labelTree r rightUpdater
                                return Branch(l, r) }

    exec (labelTree tree leftUpdater) seed