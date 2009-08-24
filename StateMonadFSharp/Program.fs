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

printfn "\nExercise 1:"
printfn "Monadically labeled tree using static monad implementation"
let monadicIncrementer = fun () -> State(fun n -> n + 1, n)
let smTree = labelTreeWithStaticStateMonad demoTree seed monadicIncrementer
show smTree

printfn "Monadically labeled tree using monad builder implementation"
let sbTree = labelTreeWithStateBuilder demoTree seed incrementer // Note the use of the normal incrementer.
show sbTree

printfn "\nExercise 2:"
let leftBounder =
  fun (depth, rect) -> let newDepth = depth + 1.0
                       let multiplier = 2.0 * newDepth
                       ( (newDepth,
                          { Height = rect.Height
                            Width = rect.Width / multiplier
                            Top = rect.Top
                            Left = rect.Left + rect.Width / multiplier }),
                         (newDepth,
                          { Height = rect.Height
                            Width = rect.Width / multiplier
                            Top = rect.Top
                            Left = rect.Left }) )

let rightBounder =
  fun (depth, rect) -> let newDepth = depth - 1.0
                       ( (newDepth,
                          { Height = rect.Height
                            Width = rect.Width * 2.0
                            Top = rect.Top
                            Left = rect.Left + rect.Width }),
                         (depth, rect) )

let initialDepth = 0.0
let initialRect = { Height = 100.0
                    Width = 100.0
                    Top = 0.0
                    Left = 0.0 }              
let ex2Seed = (initialDepth, initialRect)

printfn "Bound tree to rects"
let bTree = boundTree demoTree ex2Seed leftBounder rightBounder
show bTree