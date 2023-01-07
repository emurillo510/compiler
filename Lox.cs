using System;
namespace CompilerLearning {

    class Scanner {

    }

    class Parser {

    }

    class Lox {

        private static Boolean hadError = false;

        public static void runFile(string file) {
            Console.WriteLine("running prompt");
            Console.WriteLine("file: " + file);
            run(file);
        }

        public static void runPrompt() {
            Console.WriteLine("running prompt.");
        }

        public static void run(String source) {
            try
            {
                using (StreamReader sr = new StreamReader(source)) {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
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