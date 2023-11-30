namespace HUlK

{
    class Lexer
    {
        public static List<string> function = new List<string>();    
        public static List<Error> error_list= new List<Error>();
        
        public static List<Token> Scanner(string code)
        {
            List<Token> token_list = new List<Token>();
            string aux=string.Empty;               
            
            for (int i = 0; i < code.Length; i++)
            {
        
                if((code[i]==' ' || code[i]=='\t' || code[i]=='\0' || code[i]=='\n' || code[i]=='\r') && (aux==string.Empty )) continue;
                
                if(!char.IsLetterOrDigit(code[i])  && aux!=string.Empty &&!string.IsNullOrWhiteSpace(aux) )
                {
                    Token word = RerservedWord(aux, i);
                    token_list.Add(word);
                    aux=string.Empty;
                }

                
                if((code[i]==' ' || code[i]=='\t' || code[i]=='\0' || code[i]=='\n' || code[i]=='\r') && (aux==string.Empty )) continue;
                
                if (code[i]=='"')
                {   
                    string dato= IsLiteral(code,i);
                    if(dato!=string.Empty)
                    {
                        Token nuevo = new Token(dato,i,TokenType.StringToken);
                        token_list.Add(nuevo);
                        i+=dato.Length+1;
                    }

                }
                else if ( char.IsDigit(code[i]) && aux==string.Empty)
                {
                    double number= IsNumber(code[i].ToString(), code, i);

                    if( !double.IsNaN(number)) 
                    {
                        Token nuevo = new Token(number.ToString(), i, TokenType.NumberToken);
                        token_list.Add(nuevo);
                        i=i+nuevo.dato.Length-1;
                    } 
                    else
                        break;

                }
                else if(char.IsLetterOrDigit(code[i]) || code[i]=='_' )
                {
                    aux +=code[i];
                }
                else if(code[i]=='+')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.PlusToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='^')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.ExponentToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='-')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.MinusToken); 
                    token_list.Add(nuevo);
                }
                else if (code[i]=='*')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.MultiplicationToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='/')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.DivisionToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='%')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.RestoToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='=')
                {
                    if(code[i+1]=='=')
                    {
                        Token nuevo = new Token("==", i, TokenType.EqualEqualToken);
                        token_list.Add(nuevo);
                        i++;
                    }
                    else if(code[i+1]=='>')
                    {
                        Token nuevo = new Token("=>", i, TokenType.AssignmentFunctionToken);
                        token_list.Add(nuevo);
                        i++;
                    }
                    else
                    {
                        Token nuevo = new Token(code[i].ToString(), i, TokenType.AssignmentToken);
                        token_list.Add(nuevo);
                    }
                }
                else if (code[i]=='(')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.OpenParenthesesToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]==')')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.CloseParenthesesToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='@')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.ArrobaToken);
                    token_list.Add(nuevo);
                }
                else if(code[i]=='>')
                {
                    if(code[i+1]=='=')
                    {
                        Token nuevo = new Token(">=", i, TokenType.ComparisonToken);
                        token_list.Add(nuevo);
                        i++;   
                    }
                    else
                    {
                        Token nuevo = new Token(code[i].ToString(), i, TokenType.ComparisonToken);
                        token_list.Add(nuevo);
                    }
                }
                else if(code[i]=='<')
                {
                    if(code[i+1]=='=')
                    {
                        Token nuevo = new Token("<=", i, TokenType.ComparisonToken);
                        token_list.Add(nuevo);
                        i++;   
                    }
                    else
                    {
                        Token nuevo = new Token(code[i].ToString(), i, TokenType.ComparisonToken);
                        token_list.Add(nuevo);
                    }
                }    
                else if (code[i]=='&')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.AndOperatorToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='|')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.OrOperatorToken);
                    token_list.Add(nuevo);
                }
                else if (code[i]=='!')
                {
                    if(code[i+1]=='=')
                    {
                        Token nuevo = new Token("!=", i, TokenType.NotEqualToken);
                        token_list.Add(nuevo);
                        i++;
                    }
                    else
                    {
                        Token nuevo = new Token(code[i].ToString(), i, TokenType.NotOperatorToken);
                        token_list.Add(nuevo);
                    }
                    
                    
                }
                else if (code[i]==',')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.ColonToken);
                    token_list.Add(nuevo);
                }
                
                else if (code[i]==';')
                {
                    Token nuevo = new Token(code[i].ToString(), i, TokenType.SemiColonToken);
                    token_list.Add(nuevo);
                }
                else
                {
                    Error error= new Error($"! LEXICAL ERROR: Invalid Token '" +code[i]+"'","("+i.ToString()+")"  );
                    error_list.Add(error);
                }
                if(i==code.Length-1 && aux!=string.Empty && !string.IsNullOrWhiteSpace(aux))
                {
                    Token word = RerservedWord(aux, i);
                    token_list.Add(word);
                    aux=string.Empty;
                }
                
            }
            
            
            return token_list;
        }

        private static Token RerservedWord(string aux, int i)
        {
            int reserved = CheckWord(aux);
            if (reserved == 1)
            {
                Token nuevo = new Token(aux, i - aux.Length, TokenType.ReservedWordToken);
                return nuevo;
            }
            else if (reserved == 2)
            {
                Token nuevo = new Token(aux, i - aux.Length, TokenType.ReservedFunctionToken);
                return nuevo;
                
            }
            else if (function.Contains(aux))
            {

                Token nuevo = new Token(aux, i - aux.Length, TokenType.FunctionCall);
                return nuevo;
            }
            else
            {
                Token nuevo = new Token(aux, i - aux.Length, TokenType.IdentifierToken);
                return nuevo;

            }
        }

        private static int CheckWord(string str)
        {

            return str switch
            {
                "true" => 1,
                "false" => 1,
                "let" => 1,
                "in" => 1,
                "if" => 1,
                "else" => 1,
                "function" => 1,
                "PI" => 1,
                "E" => 1,
                "print"=>2,
                "sin"=>2,
                "cos"=>2,
                "log"=>2,
                "rand"=>2,
                "sqrt"=>2,
                "pow"=>2,
                _ => 0,
            };
        }
        
        private static double IsNumber(string num,string code,int pos)
        {    
            int cont=0;
            double x;
            for (int i = pos+1; i < code.Length; i++)
            {
                if (char.IsLetter(code[i]))
                {
                    for (int j = i; j < code.Length; j++)
                    {
                        if (char.IsLetterOrDigit(code[j])|| code[j]=='.') 
                            num+=code[j];
                        else break;
                    }
                    Error error= new Error("! LEXICAL ERROR:" +" '"+ num + "' is not  valided token.", '('+pos.ToString()+')');
                    Lexer.error_list.Add(error);
                    
                    return double.NaN ;

                }
                else if (!char.IsDigit(code[i])  && code[i]!='.') 
                {
                    break;
                }    
                else if(char.IsDigit(code[i]) || code[i]=='.'  )
                {
                    if(code[i]=='.')
                    {
                        num+=',';
                        cont++;
                    }
                    else num+=code[i];
                    
                }    
            }

            if(cont<=1)
                x=  double.Parse(num);

            else
            {        
                Error error= new Error("! LEXICAL ERROR:" +" '"+ num + "' is not  valided token.",'('+pos.ToString()+')');
                Lexer.error_list.Add(error);      
                x=double.NaN;
            } 
            return x;
        }
        private static string IsLiteral(string code,int pos )
        {
            string result="";
            bool aux= false;
            for (int i = pos+1; i < code.Length; i++)
            {
                if (code[i]=='"') 
                {
                    aux= true;
                    break;
                }
                result += code[i];
            
            }
            if(aux) return result;
            else
            {
                Error error= new Error($"! LEXICAL ERROR: Expected end of the string.","("+pos.ToString()+")"  );
                error_list.Add(error);
                return string.Empty;
            }
        }
    }

    public class Token
    {
        public string dato;
        public int position ;
        public TokenType type ;
        
        public Token (string dato , int position , TokenType type  )
        {
            this.dato=dato;
            this.position= position;
            this.type=type;
        }
    }

    class Error
    {
        public string error_type{ get; }
        public string position{ get; }
        public Error(string error_type,string position)                                  
        {
            this.error_type=error_type;
            this.position= position;            
        }

    }
}
