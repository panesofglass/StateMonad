using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// We demonstrate three ways of labeling a binary tree with unique
// integer node numbers: (1) by hand, (2) non-monadically, but
// functionally, by threading an updating counter state variable
// through function arguments, and (3) monadically, by using a
// partially generalized state-monad implementation to handle the
// threading via composition. To understand this program, it may be
// best to start with Main, seeing how the various facilities are
// used, then backtrack through the code learning first the
// non-monadic tree labeler, starting with the function Label, then
// finally the monadic tree labeler, starting with the function
// MLabel. At the very end of this file are several exercises for
// generalizing the constructions further.

namespace StateMonad
{
    public static class Extensions
    {
        // Define an extention method and a default implementation
        // here so as to treat built-in types similarly to
        // user-defined types as regards the "Show" method. Anything
        // can be "Shown"

        public static void Show<a>(this a thing, int level)
        {
            Console.Write("{0}", thing.ToString());
        }
    }

    // Never explicitly use the "set" property accessors of any of our
    // types. This means our classes are immutable by convention.  Put
    // values in properties at construction time, and then never
    // change them.

    public class Program
    {
        // Example "binary-tree" data structure. In exercises at the
        // bottom of this file, generalize this in two ways: to an
        // n-ary tree, and to something with a rich state, that does a
        // more substantial calculation, such as tracking scaling
        // parameters or 4x4 transformation matrices for geometric
        // objects.

        // The following "region" is a C# translation of the following
        // Haskell:
        //
        // A tree containing data of type "a" is either a Leaf
        // containing an instance of type a, "Lf a", or a Branch
        // containing two trees recursively containing data of type a:
        //
        // > data Tr a = Lf a | Br (Tr a) (Tr a)
        // >  deriving Show

        #region "generic tree"

        public const int indentation = 2; // for pretty-printing.

        public abstract class Tr<T>
        {
            public abstract void Show(int level);
        }

        // A Tr<a> is either a Lf<a> or a Br<a>.

        public class Lf<a> : Tr<a>
        {
            public a contents { get; set; }

            public override void Show(int level)
            {
                Console.Write(new String(' ', level * indentation));
                Console.Write("Leaf: ");
                contents.Show(level);
                Console.WriteLine();
            }
        }

        public class Br<a> : Tr<a>
        {
            public Tr<a> left { get; set; }
            public Tr<a> right { get; set; }

            public override void Show(int level)
            {
                Console.Write(new String(' ', level * indentation));
                Console.WriteLine("Branch:");
                left.Show(level + 1);
                right.Show(level + 1);
            }
        }
        #endregion // "generic tree"

        // Our C# translation of the labeled tree, this bit of Haskell
        //
        // "Lt" stands for "Labeled tree," and it's just an ordinary
        // tree, as defined above, but containing a pair of a variable
        // of type S for state and a variable of type a, where S, the
        // type of state, is a just an Int.
        // 
        // > type Lt a = (Tr (S, a))
        // > type S = Int  -- State, or "S", is just an Int
        //
        // The Label is the stateful bit we thread through the
        // labeling machinery.
        //
        // In another exercise, make this completely general,
        // generalizing over the type of the state variable with
        // another generic type parameter. For now, a state is an
        // integer, pure and simple, and a label is a state.

        #region "non-monadically labeled tree"

        // The plan is to convert trees containing "content" data into
        // trees containing pairs of contents and labels.

        // In Haskell, just construct the type "pair of state and
        // contents" on-the-fly as a pair-tuple of type (S, a). In
        // C#, create a class such pairs since tuples are not
        // primitive as they are in Haskell.

        // The first thing we need is a class or type for
        // state-content pairs, call it Scp.  Since the type of the
        // state is hard-coded as "Int," Scp<a> has only one type
        // parameter, the type a of its contents.

        public class Scp<a> // State-Content Pair
        {
            public int label { get; set; }
            public a lcpContents { get; set; } // New name; don't confuse
                                               // with the old "contents"
            public override string ToString()
            {
                return String.Format("Label: {0}, Contents: {1}", label, lcpContents);
            }
        }

        // Here's the Haskell for labeling a tree by manually
        // threading state through function arguments. Later, we
        // derive a generic state monad by abstracting parts of this
        // definition.
        //
        // Reminder: a labeled tree, Lt<a>, is a tree with an Scp<a>
        // as its contents. The following function, Label, takes a 
        // Tr a and returns a Lt a = Tr (S, a), by calling a helper
        // function, Lab, keeping only the second element of the
        // tuple that Lab returns:
        //
        // > label :: Tr a -> Lt a
        // > label tr = snd (lab tr 0)
        // >  where ...

        // Here is our C# manual-labeling function, Label, which takes
        // a Tr<a> as input and returns a Tr<Scp<a>> (for which we
        // have a new type in the Haskell version.) All this does is
        // call the helper function with a starting value for the
        // labels, namely 0, and then keep only the labeled-tree part
        // of the return value, which has both a label and a labeled
        // tree. Internally, Lab threads the label part of its return
        // value to recursive calls of Lab, but Label does not need
        // this value, even though it happens to be the value of the
        // next label that would be applied to a tree node..

        public static Tr<Scp<a>> Label<a>(Tr<a> t)
        {
            var r = Lab<a>(t, 0); // helper function
            return r.lltpTree; // keep only the tree part when done.
        }

        // Label's helper function threads the label (i.e., the state)
        // around.  It's easiest to create a new data structure to
        // hold a pair of a current Label and a partially labeled
        // Tr<Scp<a>> = Lt a because we build up the fully labeled tree
        // recursively.

        // LLtP<a> = Label-LabeledTree Pair; the return type of the
        // helper function, Lab. 

        private class LLtP<a>
        {
            public int lltpLabel { get; set; } 
            public Tr<Scp<a>> lltpTree { get; set; } //  "label" in Scp<T>
        }

        // "Lab" takes an old tree and a state value, and returns a
        // pair of state and new tree, which is, itself, a tree of
        // pairs:

        // >   lab :: Tr a -> S -> (S, Lt a)
        // >   lab (Lf contents) n = ((n+1), (Lf (n, contents))) -- returned pair
        // >   lab (Br trs) n0     = let (n1, l') = lab l n0  -- pat match in
        // >                             (n2, r') = lab r n1  -- recurive calls
        // >                         in  (n2, Br l' r')       -- returned pair

        // Direct transcription into C#:

        private static LLtP<a> Lab<a>(Tr<a> t, int lbl)
        {
            if (t is Lf<a>)
            {
                var lf = (t as Lf<a>);
                return new LLtP<a>
                {
                    lltpLabel = lbl + 1, // bump the label for recursion
                    lltpTree = new Lf<Scp<a>>
                    {
                        contents =
                        new Scp<a>
                        {
                            label = lbl, // label this Leaf node
                            lcpContents = lf.contents // copy the contents
                        }
                    }
                };
            }
            else if (t is Br<a>)
            {
                var br = (t as Br<a>);
                var l = Lab<a>(br.left, lbl); // recursive call
                var r = Lab<a>(br.right, l.lltpLabel); // threading
                return new LLtP<a>
                {
                    lltpLabel = r.lltpLabel,
                    lltpTree = new Br<Scp<a>>
                    {
                        left = l.lltpTree,
                        right = r.lltpTree
                    }
                };
            }
            else
            {
                throw new Exception("Lab/Label: impossible tree subtype");
            }
        }

        #endregion // non-monadically labeled tree

        #region "monadically labeled tree"

        // A "S2Scp" is a function from state to state-contents pair
        // (or label-contents pair). An instance of the state monad
        // will have one member: a function of this type.  Where is
        // such a function to get the contents part?  Obviously not
        // from its argument list, therefore from the environment --
        // the closure about the function.

        // This is the generalization of the state monad type: it
        // doesn't care what type the contents are. The state monad
        // just says "if you give me a function from a state to a
        // state-contents pair, I'll thread the state around for you."

        public delegate Scp<a> S2Scp<a>(int state);

        // Here is a type for functions that takes an input of type a
        // and puts it in an instance of the state monad containing an
        // instance of type b. In other words, it both transforms an a
        // to a b and lifts the b into the state monad.

        public delegate SM<b> Maker<a, b>(a input);

        // The following is actually general, aside from the hard-coding
        // of the type of label as int. This is the type of a State Monad
        // with contents of any type A.

        public class SM<a>
        {
            // Here is the meat: the only data member of this monad:

            public S2Scp<a> s2scp { get; set; }

            // Any monad -- the state monad, the continuation monad,
            // the list monad, the maybe monad, etc. must implement
            // the two operators @return and @bind, which we represent
            // here as instance methods.
            //
            //     (footnote: At-sign lets me use the "return" keyword
            //     as an identifier, and it's benign to use it on
            //     "bind" for stylistic and syntactic
            //     parallelism. These two operators are required and
            //     must satisfy the monad laws. Alternative: misspell
            //     "return" as in "retern" or what-not.)
            //
            // Exercise 3 asks you to create an abstract class with
            // these operators and to derive SM from that
            // class. Exercise 8 asks you to promote them into an
            // interface.

            // @return takes some contents as an argument and returns
            // an instance of the monad. For the state monad, this
            // instance contains, as required, a closure over a
            // function from state to state-contents pair.

            // @bind takes two arguments: an instance of monad M<a>,
            // something of type a already in the monad; and a Maker
            // function of type "from a to instance of M<b>". @bind
            // returns an instance of M<b>.  Imagine wrapping a call
            // of @bind(M<a>, a->M<b>) in a function that takes an
            // instance of type c, and see that @bind effects a
            // composition of a c->M<a> and an a->M<B> to create a
            // c->M<b>. Look up "Kleisli composition."

            // @return and @bind must satisfy the monad laws:
            //
            // Left-identity:
            //
            //     @bind(@return(anything), k)  ==  k(anything)
            //
            // Right-identity:
            //
            //     @bind(m, @return)  ==  m
            //
            // Associativity:
            //
            //     @bind(m, (x => @bind(k(x), h)))  ==
            //
            //     @bind(@bind(m, k), h)
            //
            // In exercise 7, verify the monad laws for this
            // implementation.

            // Here are the particular implementations of @return
            // and @bind for the state monad:

            // >  return contents = Labeled (\st -> (st, contents))

            // No wiggle room, here: put the contents in the contents
            // slot and put the state in the state slot. The new
            // state-monad instance contains a new function closed
            // over the contents, which are supplied in the argument
            // list of @return. The function is implemented as a C#
            // lambda expression:

            public static SM<a> @return(a contents)
            {
                return new SM<a>
                {
                    s2scp = (st => new Scp<a>
                    {
                        label = st,
                        lcpContents = contents
                    })
                };
            }

            // ">>=" is the infix notation for "@bind" from
            // Haskell. Here, take an instance of monad x and a
            // function (x -> monad y) and returns an instance of
            // monad y. No wiggle room here, either: much easier
            // to show in pictures:
            //
            //                                +---------+
            //                            .-->|  fany1  |
            //                            |   +---------+
            //                            |        |
            //                            |        |
            //                            |        v
            //                            |       ===
            //         +--------+  any1   |   +---------+
            //         |        |---------'   |         |---------->
            //         |  fst0  |             |  fst1   |
            //   st0   |        |   st1       |         |
            // ------->|        |------------>|         |---------->
            //         +--------+             +---------+
            //
            // fst0 produces a state-contents pair. Feed the contents
            // produced by fst0 into fany1, which produces a function
            // from state to state-contents pair. Feed the state
            // produced by fst0 into the function produced by
            // fany1. Get a new state-contents pair. Make the whole
            // thing just a function from st0, and the final result is
            // a function from state to state-contents pair.
            // Recognize this as the signature of the final, resulting
            // monad instance. The pattern is also obvious chainable.
            //
            // >  M fst0 >>= fany1 = -- fst0 :: st->(st, any)
            // >   M $ \st0 ->   -- return new monad instance: a func of st0
            // >    let (st1, any1) = fst0 st0  -- pat match new st1 and contents
            // >        M fst1 = fany1 any1 -- shove contents into fany1,
            // >                            --  getting new monad inst fst1.
            // >    in fst1 st1  -- feed st1 into new monad inst, return (st, any)
            // >                 -- and that's what we needed, a function from
            // >                 -- st0 to (st->any) implemented through the new
            // >                 -- monad instance returned by fany1.

            public static SM<b> @bind<b>(SM<a> inputMonad, Maker<a, b> inputMaker)
            {
                return new SM<b>
                {
                    // The new instance of the state monad is a
                    // function from state to state-contents pair,
                    // here realized as a C# lambda expression:

                    s2scp = (st0 =>
                    {
                        // Deconstruct the result of calling the input
                        // monad on the state parameter (done by
                        // pattern-matching in Haskell, by hand here):

                        var lcp1 = inputMonad.s2scp(st0);
                        var state1 = lcp1.label;
                        var contents1 = lcp1.lcpContents;

                        // Call the input maker on the contents from
                        // above and apply the resulting monad
                        // instance on the state from above:

                        return inputMaker(contents1).s2scp(state1);
                    })
                };
            }
        }

        // Here's a particular state monad instance we need to update
        // state. We're going to @bind -- that is, compose -- an
        // instance of this with leaves of the labeled tree:

        // > updateState :: Labeled S
        // > updateState =  Labeled (\n -> ((n+1),n))

        private static SM<int> UpdateState()
        {
            return new SM<int>
            {
                s2scp = (n => new Scp<int>
                {
                    label = n + 1,
                    lcpContents = n
                })
            };
        }

        // Here's a helper that composes UpdateState with Leaf and
        // Branch in the original unlabeled tree. This looks very
        // hairy in C#, but in Haskell it's quite short. Here's what
        // we do with leaves:
        //
        // > mkm :: Tr anytype -> Labeled (Lt anytype)
        // > mkm (Lf x)
        // >   = do n <- updateState  -- call updateState; "n" is of type "S"
        // >        return (Lf (n,x)) -- "return" does the heavy lifting
        // >                          --  of creating the Monad from a value.
        //
        // The "do" notation is just Haskell syntactic sugar for
        // precisely the following call of @bind:
        //
        // updateState >>= \n -> return (Lf (n, x))
        //
        // which says "call update state, which returns an instance of
        // the state monad, then @bind it to the variable n in a
        // function that returns a leaf node labeled by the given
        // state value." We translate this directly into C# below.
        // 
        // The Branch case is a trivial recursion. Notice that
        // updateState only gets called on leaves.
        //
        // > mkm (Br l r)
        // >   = do l' <- mkm l
        // >        r' <- mkm r
        // >        return (Br l' r')
        //
        // Notice this is private:

        private static SM<Tr<Scp<a>>> MkM<a>(Tr<a> t)
        {
            if (t is Lf<a>)
            {
                // Call UpdateState to get an instance of
                // SM<int>. Shove it (@bind it) through a lambda
                // expression that converts ints to SM<Tr<Scp<a>>
                // using the "closed-over" contents from the input
                // Leaf node:

                var lf = (t as Lf<a>);

                return SM<int>.@bind
                (   UpdateState(),
                    (n => SM<Tr<Scp<a>>>.@return
                        (new Lf<Scp<a>>
                            {   contents = new Scp<a>
                                {   label = n,
                                    lcpContents = lf.contents
                                }
                            }
                        )
                    )
                );
            }
            else if (t is Br<a>)
            {
                var br = (t as Br<a>);
                var oldleft = br.left;
                var oldright = br.right;
                
                return SM<Tr<Scp<a>>>.@bind
                (   MkM<a>(oldleft),
                    (newleft => SM<Tr<Scp<a>>>.@bind
                        (   MkM<a>(oldright),
                            (newright => SM<Tr<Scp<a>>>.@return
                                (new Br<Scp<a>>
                                    {
                                        left = newleft,
                                        right = newright
                                    }
                                )
                            )
                        )
                    )
                );
            }
            else
            {
                throw new Exception("MakeMonad/MLabel: impossible tree subtype");
            }
        }

        // Here's our final function: takes an unlabled tree and
        // returns a monadically labeled tree.

        // Same signature as non-monadic "Label" above

        public static Tr<Scp<a>> MLabel<a>(Tr<a> t) 
        {
            // throw away the label, we're done with it.

            return MkM(t).s2scp(0).lcpContents; 
        }

        #endregion // "monadically labeled tree"

        static void Main(string[] args)
        {
            Console.WriteLine("Unlabeled Tree:");
            var t = new Br<string>
            {
                left = new Lf<string> { contents = "a" },
                right = new Br<string>
                {
                    left = new Br<string>
                    {
                        left = new Lf<string> { contents = "b" },
                        right = new Lf<string> { contents = "c" }
                    },
                    right = new Lf<string> { contents = "d" }
                },
            };
            t.Show(2);

            Console.WriteLine();
            Console.WriteLine("Hand-Labeled Tree:");
            var t1 = new Br<Scp<string>>
            {
                left = new Lf<Scp<string>>
                {
                    contents = new Scp<string>
                    {
                        label = 0,
                        lcpContents = "a"
                    }
                },
                right = new Br<Scp<string>>
                {
                    left = new Br<Scp<string>>
                    {
                        left = new Lf<Scp<string>>
                        {
                            contents = new Scp<string>
                            {
                                label = 1,
                                lcpContents = "b"
                            }
                        },
                        right = new Lf<Scp<string>>
                        {
                            contents = new Scp<string>
                            {
                                label = 2,
                                lcpContents = "c"
                            }
                        }
                    },
                    right = new Lf<Scp<string>>
                    {
                        contents = new Scp<string>
                        {
                            label = 3,
                            lcpContents = "d"
                        }
                    }
                },
            };
            t1.Show(2);

            Console.WriteLine();
            Console.WriteLine("Non-monadically Labeled Tree:");
            var t2 = Label<string>(t);
            t2.Show(2);

            Console.WriteLine();
            Console.WriteLine("Monadically Labeled Tree:");
            var t3 = MLabel<string>(t);
            t3.Show(2);

            Console.WriteLine();
        }

        // Exercise 1: generalize over the type of the state, from int
        // to <S>, say, so that the SM type can handle any kind of
        // state object. Start with Scp<T> --> Scp<S, T>, from
        // "label-content pair" to "state-content pair".

        // Exercise 2: go from labeling a tree to doing a constrained
        // container computation, as in WPF. Give everything a
        // bounding box, and size subtrees to fit inside their
        // parents, recursively.

        // Exercise 3: promote @return and @bind into an abstract
        // class "M" and make "SM" a subclass of that.

        // Exercise 4 (HARD): go from binary tree to n-ary tree.

        // Exercise 5: Abstract from n-ary tree to IEnumerable; do
        // everything in LINQ! (Hint: SelectMany).

        // Exercise 6: Go look up monadic parser combinators and
        // implement an elegant parser library on top of your new
        // state monad in LINQ.

        // Exercise 7: Verify the Monad laws, either abstractly
        // (pencil and paper), or mechnically, via a program, for the
        // state monad.

        // Exercise 8: Design an interface for the operators @return
        // and @bind and rewrite the state monad so that it implements
        // this interface. See if you can enforce the monad laws
        // (associativity of @bind, left identity of @return, right
        // identity of @return) in the interface implementation.

        // Exercise 9: Look up the List Monad and implement it so that
        // it implements the same interface.

        // Exercise 10: deconstruct this entire example by using
        // destructive updates (assignment) in a discipline way that
        // treats the entire CLR and heap memory as an "ambient
        // monad." Identify the @return and @bind operators in this
        // monad, implement them explicitly both as virtual methods
        // and as interface methods.
    }
}
