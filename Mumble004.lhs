> data Tr a = Lf a | Br [Tr a]
>  deriving Show

> tr0 = Br [Lf 'a', Br [Lf 'b', Lf 'c']]
> tr1 = Br [Lf 'a', Br [Br [Lf 'b', Lf 'c'], Lf 'd']]

> type LbTr a = (Tr (St, a))
> type St = Int  

> label :: Tr a -> LbTr a   
> label tr = snd (lab tr 0) where
>   lab :: Tr a -> St -> (St, LbTr a)
>   lab (Lf contents) n = ((n+1), (Lf (n, contents)))
>   lab (Br trs) n0     = let l = thd trs n0 []
>                         in (fst (last l), Br (map snd l))
>    where                 
>     thd :: [Tr a] -> St -> [(St, LbTr a)] -> [(St, LbTr a)]
>     thd [] _ acc = reverse acc
>     thd (t:trs) n0 acc = let (n1, nt1) = lab t n0
>                          in thd trs n1 ((n1, nt1):acc)

> newtype Labeled a = Labeled (St -> (St, a))

> instance Monad Labeled where

>  return contents = Labeled (\st -> (st, contents))

>  Labeled fst0 >>= fany1 = 
>   Labeled $ \st0 ->  
>    let (st1, any1)  = fst0  st0  
>        Labeled fst1 = fany1 any1
>    in fst1 st1  

> mlabel :: Tr a -> LbTr a
> mlabel tr = let Labeled mt = insert tr
>             in snd (mt 0) 

> insert :: Tr a -> Labeled (LbTr a)
> insert (Lf x)
>   = do n <- updateState
>        return $ Lf (n,x)
> insert (Br trs)
>   = do mtrs <- mapM insert trs
>        return $ Br mtrs

> updateState :: Labeled St
> updateState =  Labeled (\n -> ((n+1),n))

> t1 = 5
> f x = t1 + x

> main = print $ mlabel tr1

