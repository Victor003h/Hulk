
using System.Runtime.InteropServices;

namespace HUlK;
 
public class Parser
{  
    public static string? contextOfFunction;
    private   List<Token> tokens= new List<Token>();
    public  static bool correct= true;
    private List<string> variablesContext= new List<string>();
    private  Token currentToken ; 
    private  int position;

    public  Token CurrentToken { get => currentToken; set => currentToken = value; }
    public   List<Token> Tokens { get => tokens; set => tokens = value; }
    public  int Position { get => position; set => position = value; }

    public  Parser(string code)
    {
        this.Tokens=Lexer.Scanner(code);
        tokens.Add(new Token("",tokens.Count,TokenType.EndLine));
        if(!Lexer.error_list.Any())
        {
            this.currentToken=Tokens[0];
            this.Position=0;
        }
    }

    private  Token NextToken()
    {
        if(position==tokens.Count-1)
        {
            return tokens[position];
        }
        
        currentToken=tokens[Position+1];
        Position++;
        return currentToken;                                                                 
    }
    public  Expression Parse()
    {
        Token aux= new Token("sd",0,TokenType.Error);
        Expression r= new NumberExpression(aux);
        if(!Lexer.error_list.Any())
        {
            var result= ParseOperatorOr();
            if( currentToken.type!=TokenType.SemiColonToken)
            {
                if(currentToken.type!=TokenType.EndLine)
                {
                    Error E= new Error($"! SINTAX ERROR: Unexpected Toke '{currentToken.dato}' expected ';'. ",$"({currentToken.position})");
                    Lexer.error_list.Add(E);

                }
                else
                {
                    Error Error= new Error($"! SINTAX ERROR: Expected ';'. ",$"({currentToken.position})");
                    Lexer.error_list.Add(Error);
                }
                return r;
            }
            if(NextToken().type!=TokenType.EndLine)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected 'EndLine'. ",$"({currentToken.position})");
                Lexer.error_list.Add(Error);
            }
            
            r=result;
        
        }
        return r;
    } 
    public  Expression ParseOperatorOr()
    {
        
        Expression left= ParseOperatorAnd();

        while(currentToken.type==TokenType.OrOperatorToken)
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseOperatorAnd();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BoolExpression(left,operatorNodo, right);
        }
        return left;
    }
    public  Expression ParseOperatorAnd()
    {
        
        Expression left= ParseEqual();

        while(currentToken.type==TokenType.AndOperatorToken)
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseEqual();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BoolExpression(left,operatorNodo, right);
        }
        return left;

    }
    public  Expression ParseEqual()
    {
        
        Expression left= ParseComp();

        while(currentToken.type==TokenType.EqualEqualToken || currentToken.type==TokenType.NotEqualToken)
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseComp();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BoolExpression(left,operatorNodo, right);
        }
        return left;

    }
    public  Expression ParseComp()
    {
        Expression left= ParseArroba();

        while(currentToken.type==TokenType.ComparisonToken)
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseArroba();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BoolExpression(left,operatorNodo, right);
        }
        return left;

    } 
    public  Expression ParseArroba()
     {
        Expression left= ParseTerm();
       
        while(currentToken.type==TokenType.ArrobaToken )
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseTerm();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new ArrobaExpression(left,operatorNodo,right);                                                                                                
        }
        
        return left;
     }
    public  Expression ParseTerm()
     {
        Expression left= ParseProduc();
       
        while(currentToken.type==TokenType.PlusToken ||currentToken.type==TokenType.MinusToken )
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseProduc();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BinaryExpression(left,operatorNodo,right);                                                                                                
        }
        
        return left;
     }
     public  Expression ParseProduc()
     {
        Expression left= ParseExponent();
        
        while(currentToken.type==TokenType.MultiplicationToken ||currentToken.type==TokenType.DivisionToken|| currentToken.type==TokenType.RestoToken )
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseExponent();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BinaryExpression(left,operatorNodo, right);
            
        }
        
        return left;
     }
     public  Expression ParseExponent()
     {
        Expression left= ParseUnary();
        
        while(currentToken.type==TokenType.ExponentToken )
        {
            Token operatorNodo= currentToken;
            NextToken();
            Expression right= ParseUnary();
            if(right is ErrorExpression e)
            {
                Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{operatorNodo.dato}'.",$"({e.position}) ");
                Lexer.error_list.Add(Error);
            }
            left= new BinaryExpression(left,operatorNodo, right);
                                                                                                                                                              
        }
        return left;

     }
     public  Expression ParseUnary()
     {
        
        Expression right= Factor();
        if(currentToken.type==TokenType.AssignmentToken)
        {
            Error E = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',.", $"({currentToken.position})");
            Lexer.error_list.Add(E);
        }
        return right;

     }

    private  Expression Factor()
    {
        Expression factor=new NumberExpression(currentToken);

        if(currentToken.type==TokenType.NumberToken)
        {
            NextToken();
            return factor;
        }

        if(currentToken.type==TokenType.StringToken)
        {
            factor = new StringExpression(currentToken);
            NextToken();
            return factor;
        }

        if(currentToken.type==TokenType.IdentifierToken)
        {
           
            if(tokens[position+1].type==TokenType.OpenParenthesesToken)    currentToken.type=TokenType.FunctionCall;

            
            else 
            {
                factor= new IdentifierExpression(currentToken);
                if(contextOfFunction!=null)
                {
                    if(!variablesContext.Contains(currentToken.dato))
                    {
                        Error Error= new Error($"! SEMANTIC ERROR: The name '{currentToken.dato}' does not exist in the current context. ",'('+currentToken.position.ToString()+')');
                        Lexer.error_list.Add(Error);  
                        correct=false;
                    }
                    IdentifierExpression f = new IdentifierExpression(currentToken);
                    f.FunctionName=contextOfFunction;
                    NextToken();
                    return f;
                }
                NextToken();
                return factor;
            }            
        }

        if(currentToken.type==TokenType.MinusToken || currentToken.type==TokenType.PlusToken || currentToken.type==TokenType.NotOperatorToken)
        {
            Token nodo = currentToken;
            NextToken();
            var right= Factor();
            factor= new UnaryExpression(nodo,right);               
            return factor;
        }

        if(currentToken.type==TokenType.OpenParenthesesToken)
        {
            NextToken();
            factor =ParseOperatorOr();
            NextToken();
            return factor;
        }

        if(currentToken.type==TokenType.FunctionCall)
        {  
            if(currentToken.dato!=contextOfFunction && !Lexer.function.Contains(currentToken.dato))
            {
                Error Error = new Error($"! SEMANTIC ERROR: Function '{currentToken.dato}' is not defined.",'('+currentToken.position.ToString()+')');
                Lexer.error_list.Add(Error);
                correct=false;
                return factor;
            }

            List<Expression> argsList= new List<Expression>();
            Token Function=currentToken;
            if(NextToken().type!=TokenType.OpenParenthesesToken)
            {
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '('.", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
                correct=false;
                return factor;
            }
            NextToken();
            while (true)                                                        // busco los argumento
            {                                                                       
                if(currentToken.type==TokenType.CloseParenthesesToken)  break;
                Expression arg= ParseOperatorOr();
                argsList.Add(arg);
                if(currentToken.type!=TokenType.ColonToken)  break;

                NextToken();
            }
            if(currentToken.type!=TokenType.CloseParenthesesToken)
            {
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}'.", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
                correct=false;
                return factor;
            }

            NextToken();
            factor = new FunctionCall(Function,argsList);
            return factor;
        }
        
        if(currentToken.type==TokenType.ReservedWordToken)
        {
            factor= ReserverWord(currentToken);
            return factor;
        }

        if(currentToken.type==TokenType.ReservedFunctionToken)
        {
            factor= ReserverdFunction(currentToken);
            return factor;
        }
        
        if(currentToken.type==TokenType.AssignmentToken )
        {
            Error E = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}'.", $"({currentToken.position})");
            Lexer.error_list.Add(E);
        }
        
        if(currentToken.type==TokenType.AndOperatorToken ||currentToken.type==TokenType.OrOperatorToken ||currentToken.type==TokenType.ComparisonToken||currentToken.type==TokenType.EqualEqualToken||currentToken.type==TokenType.NotEqualToken
           || currentToken.type==TokenType.DivisionToken ||currentToken.type==TokenType.ExponentToken ||currentToken.type==TokenType.MultiplicationToken || currentToken.type==TokenType.RestoToken )
        {  
            Error Error= new Error($"! SINTAX ERROR: Expected expression before '{currentToken.dato}'.",$"({currentToken.position}) ");
            Lexer.error_list.Add(Error);
            return factor;
            
        }
        else
        {
            factor= new ErrorExpression(currentToken.position,currentToken);
        }
        
        return factor;
    }
    private Expression ReserverWord(Token Token)
    {
    
        switch (Token.dato)
        {
            case "true":
            case "false":
            {
                NextToken();
                return new BoolExpression(null,Token,null);
            }
    
            case "PI":
            {
                NextToken();
                Token.dato= Math.PI.ToString();
                Token.type=TokenType.NumberToken;
                return new NumberExpression(Token);
            }
            case "E":
            {
                NextToken();
                Token.dato= Math.E.ToString();
                Token.type=TokenType.NumberToken;
                return new NumberExpression(Token);
            }
            
            case "if":                                                                                                     
                {
                    return IfFunction(Token);
                }
            case "let":
                {
                    return LetFunction();                                             
                }
            case "function":
                {
                    return FunctionMethod();

                }
            
            default:
                NextToken();
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{Token.dato}'.", $"({Token.position})");
                Lexer.error_list.Add(Error);
                return new ErrorExpression(Token.position,Token);
                
        }
    }
    private Expression IfFunction(Token aux)
    {
        List<Expression> childs = new List<Expression>();
        Expression result= new NumberExpression(aux);
        NextToken();
        if (currentToken.type != TokenType.OpenParenthesesToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        Expression boolCondition = Factor();
        childs.Add(boolCondition);
        if (currentToken.dato == "else")
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected a expression.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        Expression child_1 = ParseOperatorOr();

        childs.Add(child_1);
        if (currentToken.dato != "else")
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'else'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        NextToken();
        Expression child_2 = ParseOperatorOr();
        childs.Add(child_2);
        result = new FunctionExpression(aux, childs);
        return result;
    }
    private Expression LetFunction()
    {
        bool ok= false;
        NextToken();
        Token aux= new Token("error en let",0,TokenType.Error);
        Expression result= new NumberExpression(aux);
        Dictionary<IdentifierExpression,Expression> listVariable= new Dictionary<IdentifierExpression, Expression>();
        while (true)
        {
            if (currentToken.type != TokenType.IdentifierToken)
            {
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
            }
            

            Token identifier = currentToken;
            if (NextToken().type != TokenType.AssignmentToken)
            {
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '=' .", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
                return result;
            }

            if( NextToken().dato=="in")
            {
                Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected a expression after varible '{identifier.dato}'.", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
                return result;
            }
            if(currentToken.dato=="function")
            {
                Error Error = new Error($"! SINTAX ERROR:   Can not declare a function in let-in scope.", $"({currentToken.position})");
                Lexer.error_list.Add(Error);
                return result;
            }
            Expression value = ParseOperatorOr();
            
            for (int i = 0; i < listVariable.Count; i++)
            {
                if(listVariable.ElementAt(i).Key.Identifier.dato== identifier.dato)
                {
                    listVariable[listVariable.ElementAt(i).Key]=value;
                    ok=true;
                }
            }
            if(!ok)
            {
                IdentifierExpression id = new IdentifierExpression(identifier);
                listVariable.Add(id,value);
            }
            
            if (currentToken.type != TokenType.ColonToken)
            {
                break;
            }
            NextToken();
        }

        if (currentToken.dato != "in")
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'in'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;

        }
        NextToken();
        Expression scope = ParseOperatorOr();
        result = new LetExpression(scope,listVariable);
        return result;
    }
    private Expression ReserverdFunction(Token function)
    {
        List<Expression> childs= new List<Expression>();
        NextToken();
        if(currentToken.type!=TokenType.OpenParenthesesToken)
        {
            Error Error= new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected '('.",$"({currentToken.position})");
            Lexer.error_list.Add(Error);
        }
        NextToken();
        if(currentToken.type==TokenType.CloseParenthesesToken)
        {
            NextToken();
            return  new FunctionExpression(function,childs);
        }
        while (true)                                                        // busco los argumentos
        {                                                                       
            Expression arg= ParseOperatorOr();
            childs.Add(arg);
            if(currentToken.type!=TokenType.ColonToken)  break;
            NextToken();
        }
        if(currentToken.type!=TokenType.CloseParenthesesToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
        }
        NextToken();
        return  new FunctionExpression(function,childs);

    }
    private Expression FunctionMethod()
    {
        bool aux = false;
        bool arg=true;
        NextToken();
        Token aux2= new Token("error en function",0,TokenType.Error);
        Expression result= new NumberExpression(aux2);
        
        List<IdentifierExpression> arguments = new List<IdentifierExpression>();
        Token Function = currentToken; 
        if(Lexer.function.Contains(Function.dato)|| currentToken.type==TokenType.ReservedFunctionToken)  
        {
            Error Error = new Error($"! SINTAX ERROR: The function '{Function.dato}' is already defined.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }      
        
        if (currentToken.type != TokenType.IdentifierToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
          
        if (NextToken().type != TokenType.OpenParenthesesToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '('.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        NextToken();

        if (currentToken.type == TokenType.CloseParenthesesToken)
        {
            arg=false;
            aux=true;
        }

        else if (currentToken.type != TokenType.IdentifierToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        while (currentToken.type == TokenType.IdentifierToken && arg)                // Busco los argumentos que recibe la funcion.
        {
           
            IdentifierExpression id = new IdentifierExpression(currentToken);
            id.FunctionName=Function.dato;
            arguments.Add(id); 
            variablesContext.Add(id.Identifier.dato);
               
            aux = true;
            if (NextToken().type == TokenType.ColonToken)
            {
                NextToken();
                aux = false;
            }
        }

        if (!aux)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        if (currentToken.type != TokenType.CloseParenthesesToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected ')'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        if (NextToken().type != TokenType.AssignmentFunctionToken)
        {
            Error Error = new Error($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected '=>'.", $"({currentToken.position})");
            Lexer.error_list.Add(Error);
            return result;
        }
        NextToken();
        
        contextOfFunction=Function.dato;  

        Expression corpus = ParseOperatorOr();                                   // el cuerpo de la funcion 
        Function function = new Function(Function,arguments, corpus);
        if(correct) 
        {
            Lexer.function.Add(Function.dato);
            Execute.functionList.Add(function);
        }
        
        return function;
    }
}
