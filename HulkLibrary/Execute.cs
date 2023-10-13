namespace HUlK;

public class Execute
{
    private static  Context? globalContext;
    public static  Context? functionContext;
    public static  List<Function> functionList= new List<Function>(); 
    public static int StackOverFlow=0;

    public static Context? GlobalContext { get => globalContext; set => globalContext = value; }
     
    public static Result Evaluator(Expression tree)
    {
        if(tree is NumberExpression number)
        {
            return new Result(number.Number.dato,TokenType.NumberExpression);
        }

        if(tree is ParenthesesExpression p)
        {
            return Evaluator(p.Corpus);


        }

        if(tree is BinaryExpression binary)
        {
            double result =BinaryOperatorEvaluator(binary);
            
            return new Result(result.ToString(), TokenType.NumberExpression);
        }
        
        if(tree is UnaryExpression unary)
        {
            var right= Evaluator(unary.Right);
            if(unary.Nodo.type==TokenType.PlusToken || unary.Nodo.type==TokenType.MinusToken)
            {
                if(right.Type!= TokenType.NumberExpression)
                { 
                    Error error= new Error($"! SEMANTIC ERROR: Unary Operator '{unary.Nodo.dato}' cannot be applied to operands of type  '{right.Type}'.",'('+unary.Nodo.position.ToString()+')');
                    Lexer.error_list.Add(error);
                    return new Result("error ", TokenType.Error);
                }
                if(unary.Nodo.type==TokenType.PlusToken)
                {
                    return right;
                }
                else
                {
                    double aux= double.Parse(right.Resultado);
                    aux= -aux;
                    return new  Result(aux.ToString(),TokenType.NumberExpression);
                }
            }
            else if(unary.Nodo.type==TokenType.NotOperatorToken)
            {
                if(right.Type!= TokenType.BoolExpression)
                {
                    Error error= new Error($"! SEMANTIC ERROR: Unary Operator '{unary.Nodo.dato}' cannot be applied to operands of type  '{right.Type}'.",'('+unary.Nodo.position.ToString()+')');
                    Lexer.error_list.Add(error);
                    return new Result("error ", TokenType.Error);
                }
                bool aux = bool.Parse(right.Resultado);
                aux = !aux;
                return new  Result(aux.ToString(),TokenType.BoolExpression);

            }
        }
        
        if( tree is BoolExpression bol)
        {
            bool result= BoolEvaluator(bol);
            return new Result(result.ToString(),TokenType.BoolExpression);
        }
       
        if(tree is ArrobaExpression arroba)
        {
            var left= Evaluator(arroba.Left);
            var right= Evaluator(arroba.Right);
            if(left.Type==TokenType.BoolExpression || left.Type==TokenType.BoolExpression)
            {
                Error error= new Error($"! SEMANTIC ERROR: Operator '{arroba.Nodo.dato}' cannot be applied to operands of type  '{left.Type}' and '{right.Type}'.",'('+arroba.Nodo.position.ToString()+')');
                Lexer.error_list.Add(error);
                return new Result("error ", TokenType.Error);   
            }
            string result = left.Resultado+ right.Resultado;
            return new Result(result.ToString(), TokenType.StringExpression);
        }

        if( tree is StringExpression str)
        {
            return new Result(str.Nodo.dato,TokenType.StringExpression);
        }
        
        if( tree is FunctionExpression fun)
        {
            return FuntionEvaluator(fun);
        }

        if(tree is IdentifierExpression id )                                                                  
        {    
            if(globalContext!=null)
            {
                var value = globalContext.GetValue(id.Identifier);
                if(value==null)   return new Result("error en identificador",TokenType.Error);
                var x= Evaluator(value);
               
                return x;
            }
            if(id.FunctionName!=null && functionContext!=null)
            {
                var value= functionContext.GetValue(id.Identifier,id.FunctionName);
                if(value==null)   return new Result("error en identificador",TokenType.Error);
                return Evaluator(value);
            }
            if(globalContext==null)
            {
                Error Error= new Error($"! SEMANTIC ERROR: The name '{id.Identifier.dato}' does not exist in the current context.",'('+id.Identifier.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return  new Result("error en identificador",TokenType.Error);
            }
            
        }
        
        if(tree is LetExpression let)
        {
            if(GlobalContext==null)
            {
                GlobalContext= new Context(let.VariableList,null,null);
            }
            else
            {
                GlobalContext= new Context(let.VariableList,GlobalContext,null);
            }
            
            var res =Evaluator(let.Scope);
            if(globalContext!=null)
            {
                if(globalContext.Father!=null)  globalContext=globalContext.Father;
                else globalContext=null;
            }
            return res;

        }

        if(tree is FunctionCall v)
        {
            StackOverFlow++;
            if(StackOverFlow> 150)
            {
                Error Error = new Error($"! SEMANTIC ERROR: StackOverflow error","");
                Lexer.error_list.Add(Error);
                return new Result("error en funcion",TokenType.Error);
            }

            if(!Lexer.function.Contains(v.Function.dato))
            {
                Error Error = new Error($"! SEMANTIC ERROR: Function '{v.Function.dato}' is not defined.",'('+v.Function.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return new Result("error en funcion",TokenType.Error);        
            }

            Dictionary<IdentifierExpression,Expression> dic = new Dictionary<IdentifierExpression, Expression>();
            int positionFunction=0;
            for (int i = 0; i < functionList.Count; i++)                    
            {
                if(functionList[i].Name.dato==v.Function.dato)
                {
                    positionFunction=i;
                    break;
                }
            }
            Function actual=functionList[positionFunction];
            
            if(v.Arg.Count!=actual.Arguments.Count)                       // chequeo si recibe la cantidad correcta de parametros 
            {
                Error Error = new Error($"! SEMANTIC ERROR: Function '{actual.Name}' receives {actual.Arguments.Count} argument(s), but {v.Arg.Count} were given.",'('+v.Function.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return new Result("error en funcion",TokenType.Error);
            }   
           
            for (int i = 0; i < v.Arg.Count; i++)           
            {    
                var  aux = Evaluator(v.Arg[i]);
                Expression e= v.Arg[i];
                if(aux.Type==TokenType.NumberExpression)
                {
                    e= new NumberExpression(new Token(aux.Resultado,0,TokenType.NumberToken));
                    dic.Add(actual.Arguments[i],e);
                }
                else if(aux.Type==TokenType.BoolExpression)
                {
                e= new BoolExpression(null,new Token(aux.Resultado,0,TokenType.BoolExpression),null);
                    dic.Add(actual.Arguments[i],e);
                }
                else if(aux.Type==TokenType.StringExpression)
                {
                    e= new StringExpression(new Token(aux.Resultado,0,TokenType.StringToken));
                    dic.Add(actual.Arguments[i],e);
                }
                else
                {
                    return new Result("error en funcion",TokenType.Error);
                }
            }
            if(functionContext==null)
            {
                functionContext= new Context(dic,null,actual.Name.dato);
            }
            else
            {
                functionContext= new Context(dic,functionContext,actual.Name.dato);
            }
            var a=Evaluator(actual.Corpus);
            if(functionContext.Father!=null)  functionContext=functionContext.Father;
            StackOverFlow --;
            
            return a;
             
        }
        
        if(tree is Function f)
        {
            Error e= new Error("! SEMANTIC ERROR : Unexpected function declaration.",$"({f.Name.position}) ");
            Lexer.error_list.Add(e);
            Lexer.function.Remove(f.Name.dato);
            functionList.RemoveAt(functionList.Count-1);
            return new Result("error ", TokenType.Error);
        }
        
        Error er= new Error("! SEMANTIC ERROR.","");
        Lexer.error_list.Add(er);
        return new Result("error ", TokenType.Error);
    }

    
    private static bool BoolEvaluator(BoolExpression tree)                                                  
    {
        if(tree.Left==null || tree.Right==null)
        {
            if(tree.Nodo.dato=="true")  return true;
            else if(tree.Nodo.dato=="false")  return false;
        }
        else if(tree.Nodo.dato=="&" || tree.Nodo.dato=="|" )
        {
            
            var left=Evaluator(tree.Left);
            var right=Evaluator(tree.Right);
            if(left.Type!=TokenType.BoolExpression || right.Type!=TokenType.BoolExpression)
            {
                string error_type= $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}.'";
                Error Error= new Error(error_type,'('+tree.Nodo.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return false;
            }
    
            if(tree.Nodo.dato=="&")
            {
                return bool.Parse(left.Resultado) && bool.Parse(right.Resultado);   
            }
            else return bool.Parse(left.Resultado) || bool.Parse(right.Resultado);
        }
        else if (tree.Nodo.dato==">"|| tree.Nodo.dato==">=" || tree.Nodo.dato=="<" || tree.Nodo.dato=="<=")
        {
            var left=Evaluator(tree.Left);
            var right=Evaluator(tree.Right);
            if(left.Type!=TokenType.NumberExpression || right.Type!=TokenType.NumberExpression)
            {
                string error_type= $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}'.";
                Error Error= new Error(error_type,'('+tree.Nodo.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return false;
            }
            double l = double.Parse(left.Resultado);
            double r = double.Parse(right.Resultado);
            if(tree.Nodo.dato==">")
            {
                if(l > r)    return true;
                else return false;
            }
            if(tree.Nodo.dato==">=")
            {
                if(l >= r)    return true;
                else return false;
            }
            if(tree.Nodo.dato=="<")
            {
                if(l < r)    return true;
                else return false;
            }
            if(tree.Nodo.dato=="<=")
            {
                if(l <= r)    return true;
                else return false;
            }

        }

        else if( tree.Nodo.dato== "==" || tree.Nodo.dato=="!=")
        {
            var left= Evaluator(tree.Left);
            var right= Evaluator(tree.Right);
            
            if(left.Type!= right.Type)
            {
                string error_type= $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}'.";
                Error Error= new Error(error_type,'('+tree.Nodo.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return false;
            }
            if(left.Type==TokenType.NumberExpression)
            {   
                double  l= double.Parse(Evaluator(tree.Left).Resultado);
                double  r= double.Parse(Evaluator(tree.Right).Resultado);
                if(tree.Nodo.dato=="==")
                {
                    return (l==r);
                    
                }
                else if(tree.Nodo.dato=="!=")
                {
                    return l!=r;
                }

            }
            if(left.Type==TokenType.BoolExpression || left.Type==TokenType.StringExpression)
            {
                string l= Evaluator(tree.Left).Resultado;
                string r= Evaluator(tree.Right).Resultado;
                if(tree.Nodo.dato=="==")
                {
                    return (l==r);
                    
                }
    
                else if(tree.Nodo.dato=="!=")
                {
                    return l!=r;
                }

            }
            
        }
        
        return false;

    }
    public static double BinaryOperatorEvaluator(BinaryExpression tree)
    {
        var left= Evaluator(tree.Left);
        var right= Evaluator(tree.Right);
        if(left.Type!=TokenType.NumberExpression || right.Type!=TokenType.NumberExpression)
        {
            string error_type= $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}'.";
            Error Error= new Error(error_type,'('+tree.Nodo.position.ToString()+')');
            Lexer.error_list.Add(Error);
            return double.NaN;
        }
        
        else if(tree.Nodo.type==TokenType.PlusToken)
        {
            return double.Parse(left.Resultado) +double.Parse(right.Resultado) ;
        }
        else if(tree.Nodo.type==TokenType.MinusToken)
        {
            return double.Parse(left.Resultado) - double.Parse(right.Resultado) ;
        }
        else if(tree.Nodo.type==TokenType.MultiplicationToken)
        {
            return double.Parse(left.Resultado)  * double.Parse(right.Resultado) ;
        }
        else if(tree.Nodo.type==TokenType.DivisionToken)
        {
            
            if(double.Parse(right.Resultado)==0)
            {
                string error_type= $" ! MATH ERROR: You can not divide by 0.";
                Error Error= new Error(error_type,'('+tree.Nodo.position.ToString()+')');
                Lexer.error_list.Add(Error);
                return 1;

            }
            return double.Parse(left.Resultado) / double.Parse(right.Resultado);
        }
        else if(tree.Nodo.type==TokenType.ExponentToken)
        {
           
            return Math.Pow(double.Parse(left.Resultado),double.Parse(right.Resultado));
        }
        else
        {
            return double.Parse(left.Resultado)  % double.Parse(right.Resultado) ;
        }
        
        
    
    }
    public static Result FuntionEvaluator(FunctionExpression tree)
    {
        switch (tree.Function.dato)
        {
            case "print":
            {
                tree.Type=TokenType.PrintExpression;
                return  Evaluator(tree.Child[0]);
            }
            case"cos":
            case"sin":
            case"sqrt":
            {
                if(tree.Child.Count!=1)
                {

                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 1 argument, but {tree.Child.Count} were given.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);
                }
                var arg= Evaluator(tree.Child[0]);
                if(arg.Type!=TokenType.NumberExpression)
                {
                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'NumberExpression' not '{arg.Type}'.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);            
                }
                double res=0;
                double num= double.Parse (arg.Resultado);
                if (tree.Function.dato=="cos")   res=Math.Cos(num);
                else if(tree.Function.dato=="sin")  res=Math.Sin( num);
                else
                {
                    if(num<0)
                    {
                        Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' must receive a positive argument.",'('+tree.Function.position.ToString()+')');
                        Lexer.error_list.Add(Error);
                        return new Result("error en funcion",TokenType.Error);
                    }
                    res=Math.Sqrt(num);
                }
                if(res>0 && res<0.0000001 || (res<0 && res>0.0000001)  ) res=0;
                else if(res - Math.Truncate(res) > 0.99999999  )    res= Math.Truncate(res)+1;
                return new Result(res.ToString(),TokenType.NumberExpression);

            }
            
            case"pow":
            case"log":
            {

                if(tree.Child.Count!=2)
                {
                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 2 arguments, but {tree.Child.Count} were given.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);
                }
                var arg1=Evaluator(tree.Child[0]);
                var arg2= Evaluator(tree.Child[1]);
                if( arg1.Type!=TokenType.NumberExpression )
                {
                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'NumberExpression' not '{arg1.Type}'.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);            
                }
                if( arg2.Type!=TokenType.NumberExpression )
                {
                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'NumberExpression' not '{arg2.Type}'.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);            
                }
                double aux=double.Parse(arg1.Resultado);
                double aux2=double.Parse(arg2.Resultado);
                double res=0; 
                if(tree.Function.dato=="pow")
                {
                    res=Math.Pow(aux,aux2);
                    return new Result(res.ToString(),TokenType.NumberExpression);    
                }
                else
                {
                    if(aux<=0 || aux2<=0 )
                    {
                        Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' must receives positive arguments.",'('+tree.Function.position.ToString()+')');
                        Lexer.error_list.Add(Error);
                        return new Result("error en funcion",TokenType.Error);   
                    }
                    if(aux2==1)
                    {
                        Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' cannot receive a base equal 1.",'('+tree.Function.position.ToString()+')');
                        Lexer.error_list.Add(Error);
                        return new Result("error en funcion",TokenType.Error); 
                    }

                    res=Math.Pow(aux,aux2);
                    return new Result(res.ToString(),TokenType.NumberExpression);    
                }
            }
            
            case"rand":
            {
                if(tree.Child.Count!=0)
                {
                    Error Error = new Error($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 0 argument, but {tree.Child.Count} were given.",'('+tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);
                    return new Result("error en funcion",TokenType.Error);
                }
                Random r = new Random();
                
                return new Result(r.NextDouble().ToString(),TokenType.NumberExpression);
            }
            case "if":
            {
                string conditon="";
                bool cond;
                conditon = Evaluator(tree.Child[0]).Resultado;
                bool isOK=bool.TryParse(conditon, out cond);

                if(bool.TryParse(conditon, out cond))
                {
                    if(cond)    return Evaluator(tree.Child[1]);
                    else    return Evaluator(tree.Child[2]);

                }
                else
                {
                    Error Error = new Error($"! SEMANTIC ERROR: The expression if-else receive  'BoolExpression' not {Evaluator(tree.Child[0]).Type}.",tree.Function.position.ToString()+')');
                    Lexer.error_list.Add(Error);

                    return new Result("error en if",TokenType.Error);
        
                }
                
            
            }
            default:
                return  new Result("error en if",TokenType.Error);
        }
    }
   
}

public class Result
{
    private string resultado;
    private TokenType type;

    public Result(string resultado,TokenType type)
    {
        this.resultado = resultado;
        this.type = type;
    }

    public string Resultado { get => resultado; set => resultado = value; }
    public TokenType Type { get => type; set => type = value; }
}