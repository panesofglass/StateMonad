module Base
  let indentation = 2

  type Tree<'a> =
    | Leaf of 'a
    | Branch of Tree<'a> * Tree<'a>

  // The show method already takes into account the tuples,
  // unlike the show method that is overloaded in the C# version.
  let show tree =
    let rec printTree tree level =
      let spacing = new string(' ', level * indentation)
      printf "%A" spacing
      match tree with
      | Leaf((label, contents)) ->
          printfn "Leaf: %A, Contents: %A" label contents
      | Branch(left, right) ->
          printfn "Branch: "
          printTree left (level + 1)
          printTree right (level + 1)
    printTree tree 0