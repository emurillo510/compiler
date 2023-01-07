using System;
namespace CompilerLearning {

    public class Scanner {
        private string source;

        public Scanner(string source) {
            this.source = source;
        }
        public List<Token> scanTokens() {
            Token t = new Token(TokenType.STRING, source, source, 0);
            return new List<Token>(){t};
        }
    }

    class Parser {

    }

    public class Lox {

        private static Boolean hadError = false;

        public static void runFile(string file) {
            Console.WriteLine("running prompt");
            Console.WriteLine("file: " + file);
            run(file);
        }

        public static void runPrompt() {
            try {
                while (true) { 
                    Console.WriteLine("> ");
                    string line = Console.ReadLine();
                    if (line == null) break;
                    else run(line);
                }
            } catch (IOException e) {
                Console.WriteLine("some error occured." + e.StackTrace);
            }
        }
        public static void run(String source) {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            
            // For now, just print the tokens.
            foreach (Token token in tokens) {
                Console.WriteLine(token);
            }
        }

        public static void Main(String[] args) {
            Console.WriteLine("Starting compiler.");

            if (args.Length > 1) {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            } else if (args.Length == 1) {
                runFile(args[0]);
                if (hadError) Environment.Exit(64);
            } else {
                runPrompt();
                hadError = false;
            }

            Console.WriteLine("Ending compiler.");
        }


        static void error(int line, String message) {
            report(line, "", message);
        }

        static void report(int line, String where, String message) {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}