open Base
open Exercise1

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

// Test it out
let demoTree = Branch(Leaf("A"), Branch(Leaf("B"),Branch(Leaf("C"),Leaf("D"))))
let initialState = 0

printfn "Non-monadically labeled tree"
let incrementer = fun n -> n + 1
let labeledTree = labelTreeWithoutMonad demoTree initialState incrementer
show labeledTree

printfn "Exercise 1: Monadically labeled tree"
let inputMaker = fun () -> State(fun n -> n + 1, n)
let mTree = labelTreeWithStateMonad demoTree initialState inputMaker
show mTree