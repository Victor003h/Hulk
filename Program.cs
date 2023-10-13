using HUlK;

namespace Program;
 
public class Program
{

    static void Main()
    {
       // bool ok;
        int x= 0 ;
        while(true)
        {

            Lexer.error_list.Clear();
            Execute.GlobalContext=null;
            Execute.functionContext=null;
            Parser.contextOfFunction=null;
            Execute.StackOverFlow=0;
        
            Console.ForegroundColor= ConsoleColor.Yellow;
            System.Console.Write ("> ");
            Console.ResetColor();
            var code = Console.ReadLine();                               

           // string code="(1+);"  ;
            //if(x==1)  code= "f(2);" ;  
            //else if(x==2)   code="  cuatr(3);";
            if( string.IsNullOrWhiteSpace(code))    continue;
            
            if(code=="clear")
            {
                Console.Clear();
                continue;
            }

            Parser parser = new Parser(code);
            var tree= parser.Parse();
                       
            if (Lexer.error_list.Count != 0)
            {
                Console.ForegroundColor= ConsoleColor.DarkRed;
                for (int i = 0; i < Lexer.error_list.Count; i++)
                {
                    Console.WriteLine(Lexer.error_list[i].error_type + " " + Lexer.error_list[i].position);
                    break;
                }
                Console.ResetColor();
            }
            else
            {    
                if(tree is Function f)
                {
                    x++;
                   //ok=true;
                    continue;
                }       
                var resul = Execute.Evaluator(tree);
                
                if (Lexer.error_list.Count != 0)
                {
                    Console.ForegroundColor= ConsoleColor.DarkRed;
                    for (int i = 0; i < Lexer.error_list.Count; i++)
                    {
                        System.Console.WriteLine(Lexer.error_list[i].error_type + " " + Lexer.error_list[i].position);
                        break;
                    }
                    Console.ResetColor();           

                }
                else
                {
                    Console.ForegroundColor= ConsoleColor.Green;
                    Console.WriteLine(resul.Resultado);
                    Console.ResetColor();
                }
       
            }
            
            x++;
        }
    }
}