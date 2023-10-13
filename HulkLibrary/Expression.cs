namespace HUlK;
public abstract class Expression 
{
    public virtual TokenType Type { get => type;set=>type=value; }
    public  TokenType type;
    
}
public class UnaryExpression: Expression
{
    public override TokenType Type { get => TokenType.BinaryExpression; }
    private Expression right;
    private Token nodo;

    public UnaryExpression(Token nodo,Expression right)
    {
        this.right = right;
        this.nodo = nodo;
    }
    public Expression Right{get => right; set => right = value; }
    public Token Nodo { get => nodo; set => nodo = value; }    

}
public class ParenthesesExpression: Expression
{
    public override TokenType Type { get => TokenType.ParenthesesExpression; }
    private Expression corpus;
    public ParenthesesExpression(Token nodo,Expression corpus)
    {
        this.corpus = corpus;
    }
    public Expression Corpus{get =>corpus ; set => corpus = value; }
}
public class BinaryExpression : Expression
{
    private Expression left;
    private Expression right;
    private Token nodo;

    public Expression Left{get => left; set => left = value; }
    public Expression Right{get => right; set => right = value; }
    public Token Nodo { get => nodo; set => nodo = value; }
    public override TokenType Type { get => TokenType.BinaryExpression; }

    public BinaryExpression (Expression left, Token nodo,Expression right)
    {
        this.left= left;
        this.right=right;
        this.nodo=nodo;
        
    }
}

public class ArrobaExpression: Expression
{
    
    private Expression left;
    private Expression right;
    private Token nodo;

    public Expression Left{get => left; set => left = value; }
    public Expression Right{get => right; set => right = value; }
    public Token Nodo { get => nodo; set => nodo = value; }
    public override TokenType Type { get => TokenType.BinaryExpression; }

    public ArrobaExpression (Expression left, Token nodo,Expression right)
    {
        this.left= left;
        this.right=right;
        this.nodo=nodo;
        
    }
    
}
public class NumberExpression:Expression
{
    public override TokenType Type { get => TokenType.NumberExpression; }
    public Token Number { get => number; set => number = value; }

    private Token number;

    public NumberExpression(Token number)
    {
        this.number = number;
    }
}

public class BoolExpression : Expression
{
    private  Expression? left;
    private  Token nodo;
    private Expression? right;
    public Expression? Left { get => left; set => left = value; }
    public Token Nodo { get => nodo; set => nodo = value; }
    public Expression? Right { get => right; set => right = value; }
    public override TokenType Type {get => TokenType.BoolExpression;}
    public BoolExpression(Expression? left,Token nodo, Expression? right )
    {
        this.left = left;
        this.nodo = nodo;
        this.right = right;
    }
}

public class StringExpression : Expression
{
    private Token nodo;
    public StringExpression(Token nodo)
    {
        this.nodo= nodo;
    }

    public Token Nodo { get => nodo; set => nodo = value; }

    public override TokenType Type {get =>TokenType.StringExpression; }

}
public class IdentifierExpression : Expression
{
    private Token identifier;
    private string? functionName;
    public IdentifierExpression( Token identifier)
    {
        this.identifier = identifier;
    }
    
    public new TokenType type = TokenType.Identifier;
    public override TokenType Type { get => this.type ; set => this.type = value; }
    public Token Identifier { get => identifier; set => identifier = value;}
    public string? FunctionName { get => functionName; set => functionName = value; }
}
public class FunctionExpression: Expression
{
    private Token function ;

    private List<Expression> child;
    
    public FunctionExpression(Token function, List<Expression> child)
    {
        this.function = function;
        this.child = child;
       
    }
    public Token Function { get => function; set => function = value; }
    public List<Expression> Child { get => child; set => child = value; }

    public override TokenType Type { get => TokenType.FunctionExpression ; set => type = value; }

}
public class LetExpression: Expression
{
    private Dictionary<IdentifierExpression, Expression>variableList;
    private Expression scope;

    public LetExpression( Expression scope, Dictionary<IdentifierExpression, Expression>variableList)
    {
        this.variableList=variableList;
        this.scope = scope;
    }

    public override TokenType Type { get => TokenType.FunctionExpression ; set => type = value; }
    public Expression Scope { get => scope; set => scope = value; }
    public Dictionary<IdentifierExpression, Expression> VariableList { get => variableList; set => variableList = value; }
}
public class Function : Expression
{
    public new TokenType type = TokenType.Function;
    public override TokenType Type { get => this.type; set => this.type= value; }

    private Expression corpus ;
    public Expression Corpus { get => corpus; set => corpus = value; }
    
    private Token name;
    public Token Name { get => name; set => name = value; }
    
    
    private List<IdentifierExpression> arguments;
    public List<IdentifierExpression> Arguments { get => arguments; set => arguments = value; }


    public Function(Token name,List<IdentifierExpression> arguments, Expression corpus)
    {
        this.name = name;
        this.arguments= arguments;
        this.corpus = corpus;
    }
}
public class FunctionCall : Expression
{
    private Token function;
    private  List<Expression> arg;
    
    public FunctionCall(Token function,List<Expression>arg )
    {
        this.function = function;
        this.arg = arg;
    }

    public  Token Function => function;

    public List<Expression> Arg => arg;
}

public class ErrorExpression: Expression
{
        public int position{get; set;}
         public Token token {get;set;}

    public ErrorExpression(int position,Token token)
    {
        this.token=token;
        this.position=position;
    }
}

public class Context
{
    private Dictionary<IdentifierExpression, Expression> variables;
    private Context? father;
    private string? nameFunction;
    public Context(Dictionary<IdentifierExpression, Expression>variables, Context? father,string? nameFunction)
    {
        this.variables = variables;
        this.father = father;
        this.nameFunction=nameFunction;
    }
    public Dictionary<IdentifierExpression, Expression> Variables { get => variables; set => variables = value; }
    public Context? Father { get => father; set => father = value; }
    public string? NameFunction { get => nameFunction; set => nameFunction = value; }
    
    public Expression? GetValue(Token identifier)
    {
        for (int i = 0; i < this.Variables.Count; i++)
        {
            if(this.Variables.ElementAt(i).Key.Identifier.dato== identifier.dato)
            {
                return this.Variables[this.Variables.ElementAt(i).Key];
            }
        }
        
        if(this.Father==null)
        {
            string error_type= $"! SEMANTIC ERROR: The name '{identifier}' does not exist in the current context.";
            Error Error= new Error(error_type,'('+identifier.position.ToString()+')');
            Lexer.error_list.Add(Error);
            return null;
        }
        return this.Father.GetValue(identifier);
    }
    public Expression? GetValue(Token identifier,string functionName)
    {   
        for (int i = 0; i < this.Variables.Count; i++)
        {
            if(this.Variables.ElementAt(i).Key.Identifier.dato== identifier.dato && this.Variables.ElementAt(i).Key.FunctionName==functionName )
            {
                return this.Variables[this.Variables.ElementAt(i).Key];
            }
        }
        if(this.Father==null)
        {
            
            string error_type= $"! SEMANTIC ERROR: The name '{identifier}' does not exist in the current context ";
            Error Error= new Error(error_type,'('+identifier.position.ToString()+')');
            Lexer.error_list.Add(Error);
            return null;
        }

        return this.Father.GetValue(identifier,functionName);
    }   
}


