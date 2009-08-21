open Base
open Exercise1_1
open Exercise1_2
open Exercise2

// Non-monadic labeller
let labelTreeWithoutMonad tree label incrementer =
  let rec labelTree tree label incrementer =
    match tree with
    | Leaf(contents)      -> incrementer label, Leaf(label, contents)
    | Branch(left, right) -> let l = labelTree left label incrementer
                             let r = labelTree right (fst l) incrementer
                             (fst r, Branch((snd l), (snd r)))
  labelTree tree label incrementer |> snd

// Test it out
let demoTree = Branch(
                 Leaf("A"), 
                 Branch(
                   Branch(
                     Leaf("B"),
                     Leaf("C")),
                   Leaf("D")))
let seed = 0

printfn "Non-monadically labeled tree"
let incrementer = fun n -> n + 1
let labeledTree = labelTreeWithoutMonad demoTree seed incrementer
show labeledTree

printfn "Exercise 1: Monadically labeled tree using static monad implementation"
let monadicIncrementer = fun () -> State(fun n -> n + 1, n)
let smTree = labelTreeWithStaticStateMonad demoTree seed monadicIncrementer
show smTree

printfn "Exercise 1: Monadically labeled tree using monad builder implementation"
let sbTree = labelTreeWithStateBuilder demoTree seed incrementer // Note the use of the normal incrementer.
show sbTree

printfn "Exercise 2: Bound tree with rects"
let leftBounder = fun rect -> { Height = rect.Height
                                Width = rect.Width / 2
                                Top = rect.Top
                                Left = rect.Left + (rect.Width / 2) }

let rightBounder = fun rect -> { Height = rect.Height
                                 Width = rect.Width * 2
                                 Top = rect.Top
                                 Left = rect.Left + rect.Width }
                                 
let rectSeed =
  { Height = 100
    Width = 100
    Top = 1
    Left = 1 }
                                
let bTree = boundTree demoTree rectSeed leftBounder rightBounder
show bTree