using HUlK;

namespace Program;
 
public class Program
{

    static void Main()
    {
       
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

            if( string.IsNullOrWhiteSpace(code))    continue;
            

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
            
          
        }
    }
}
