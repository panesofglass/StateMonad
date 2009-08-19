open Base
open Exercise1

// Non-monadic labeller
let labelTreeWithoutMonad tree label incrementer =
  let rec labelTree tree label incrementer =
    match tree with
    | Leaf(contents)      -> (incrementer label, Leaf((label, contents)))
    | Branch(left, right) -> let l = labelTree left label incrementer
                             let r = labelTree right (fst l) incrementer
                             (fst r, Branch((snd l), (snd r)))
  labelTree tree label incrementer |> snd

// Test it out
let demoTree = Branch(Leaf("A"), Branch(Leaf("B"),Branch(Leaf("C"),Leaf("D"))))
let initialState = 0

printfn "Non-monadically labeled tree"
let incrementer = fun n -> n + 1
let labeledTree = labelTreeWithoutMonad demoTree initialState incrementer
show labeledTree

printfn "Exercise 1: Monadically labeled tree using static monad implementation"
let monadicIncrementer = fun () -> State(fun n -> n + 1, n)
let smTree = labelTreeWithStaticStateMonad demoTree initialState monadicIncrementer
show smTree

printfn "Exercise 1: Monadically labeled tree using monad builder implementation"
let mTree = labelTreeWithStateBuilder demoTree initialState incrementer // Note the use of the normal incrementer.
show mTree