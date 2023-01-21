using System;
using System.Collections.Generic;
  
namespace CompilerLearning {

    public class Scanner {
        private string source;

        private List<Token> tokens;

        private int start = 0;

        private int current = 0;

        private int line = 1;

        private static Dictionary<String, TokenType> keywords = new Dictionary<string, TokenType>();

        public Scanner(string source) {
            this.source = source;
            this.tokens = new List<Token>();
            
            keywords.Add("and", TokenType.AND);
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("fun", TokenType.FUN);
            keywords.Add("if", TokenType.IF);
            keywords.Add("nil", TokenType.NIL);
            keywords.Add("or", TokenType.OR);
            keywords.Add("print", TokenType.PRINT);
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.SUPER);
            keywords.Add("this", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("while", TokenType.WHILE);
        }

        public Boolean isAtEnd() {
            return current >= source.Length;
        }

        /*
        In each turn of the loop, we scan a single token. 
        This is the real heart of the scanner. 
        We’ll start simple. 
        Imagine if every lexeme were only a single character long. 
        All you would need to do is consume the next character and pick a token type for it. 
        Several lexemes are only a single character in Lox, so let’s start with those.
        
        */
        public List<Token> scanTokens() {
            Console.WriteLine("line 55 source.Length : " + source.Length);
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");


            while(!isAtEnd()) {
                Console.WriteLine("line 59 start: " + start);
                Console.WriteLine("line 59 current: " + current);
                // this line moves the parsing position forward
                start = current;

                Console.WriteLine("line 64 start: " + start);
                Console.WriteLine("line 64 current: " + current);

                // this reads the next character
                char c = advance();
                
                // single character token parsing for now
                switch (c) {

                    /*
                    how to handle single characters
                    */
                    case '(': addToken(TokenType.LEFT_PAREN); break;
                    case ')': addToken(TokenType.RIGHT_PAREN); break;
                    case '{': addToken(TokenType.LEFT_BRACE); break;
                    case '}': addToken(TokenType.RIGHT_BRACE); break;
                    case ',': addToken(TokenType.COMMA); break;
                    case '.': addToken(TokenType.DOT); break;
                    case '-': addToken(TokenType.MINUS); break;
                    case '+': addToken(TokenType.PLUS); break;
                    case ';': addToken(TokenType.SEMICOLON); break;
                    case '*': addToken(TokenType.STAR); break;

                    /*
                    how to handle double characters


                    */
                    case '!':
                        addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                        break;
                    case '=':
                        addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                        break;
                    case '<':
                        addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                        break;
                    case '>':
                        addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                        break;

                    /*
                    How to handle longer lexemes

                    division needs special handling because comments begin with a slash too
                    */
                    case '/':
                        if(match('/')) {
                            // a comment goes until the end of the line
                            while(peek() != '\n' && !isAtEnd()) advance();
                        } else {
                            addToken(TokenType.SLASH);
                        }
                        break;

                    /*
                        how to handle meaningless characters(newlines and whitespace)

                        when encountering whitespace, we simply go back to the beginning
                        of the scan loop. that starts a new lexeme after the whitespace character.

                        for newlines, we do the samething, but we also increment the line counter.

                        this is why we used peek() to find the newline ending a comment instead
                        of match(). we want that newline to get us here so we can update line.


                    */
                    case ' ':
                    case '\r':
                    case '\t':
                        // ignore whitespcae
                        break;
                    case '\n':
                        line++;
                        break;



                    /*
                        how to handle string literals

                        strings always begin with ".

                        like with comments, we consume characters until we hit the "
                        that ends the string.

                        we also gracefully handle running out of input before the string is
                        closed and report an error for that.

                        lox supports multi-line strings. there are pros and cons
                        to taht, but prohibiting them was a little more complex than allowing
                        them, so I left them in.

                        that does mean we also need to update line 
                        when we hit a newline inside a string.

                        finally, the last interesting bit is taht when we create the token,
                        we also produce the actual string value that will be used later
                        by the interpreter.

                        here, that conversion only requires a substring() to strip off the
                        surrounding quotes. if lox supported escape sequences like \n,
                        we'd unescape those here.
                    */

                    case '"': handlestring(); break;


                    /*
                        reserve words and identifiers

                        our scanner is almost done.
                        the only remaining pieces of the lexical grammar to
                        implement are identifiers and their close cousins,
                        the reserve words.

                        you might think we could match keywords like or 
                        in the same we handle multiple-character
                        operators like <= :

                        consider what would happen if a user named a variable
                        orchid.

                        the scanner would se ethe first two letters, or
                        and immediately emit an or keyword token.

                        this gets us to an important principle called maximal
                        munch. when two lexical grammar rules can both match a chunk
                        of code that the scanner is looking at, 
                        whichever one matches the most
                        characters win.

                        the rule states that if we can match orchid as an identifier and
                        or as a keyword,then the former wins. this is also why we tacitly
                        assumed previously that <= should be scanned as a single <=
                        token and not < followed by  =.

                        maximal munch means we can't easily detect a reserved word until we've
                        reached the end of what we might instead be an identifier. after all,
                        a reserve word is an identifier, it's just one that has been claimed by
                        the langauge for it's own use. that's where the term reserved word comes from.

                        so we begin by assuming any lexeme starting with a letter or underscore is
                        an identifier.
                    */




                    




                    /*
                    How to handle errors via default


                    Note that the erroneous character is still consumed by the earlier call to 
                    advance(). That’s important so that we don’t get stuck in an infinite loop.

                    Note also that we keep scanning. 
                    There may be other errors later in the program. 
                    It gives our users a better experience if we detect as many of those 
                    as possible in one go. 
                    
                    Otherwise, they see one tiny error and fix it, only to have the next 
                    error appear, and so on. Syntax error Whac-A-Mole is no fun.
                    
                    (Don’t worry. Since hadError gets set, 
                    we’ll never try to execute any of the code, 
                    even though we keep going and scan the rest of it.)   
                    */
                    default: 

                    /*
                        how to handle number literals

                        all numbers in lox are floating point at runtime, but
                        both integer and decimal literals are supported.

                        a number literal is a series of digits optionally followed by
                        a . and one or more trailing digits.

                        1234
                        12.34

                        we don't allow a leading or trailing decimal point, so these are 
                        both invalid:

                        .1234

                        1234.


                        to recognize the beginning of a number lexeme, we look for any digit.
                        it's kind of tedious to add cases for every decimal digit, so
                        we'll stuff it in the default case instead.


                    */
                        if(isDigit(c)) {
                            Console.WriteLine("line 273 default handlenumber");
                            handlenumber();
                        } else if(isAlpha(c)) {
                            Console.WriteLine("line 273 default handleidentifier");
                            handleidentifier();
                        } else {
                            Lox.error(line, "unexpected character.");
                        }
                        break;
                }
                // this keeps the loop going

                Console.WriteLine("line 277 current: " + current);
                Console.WriteLine("line 277 start: " + start);
                scanTokens();

            }
            
            tokens.Add(new Token(TokenType.EOF, "", "", 0));
            return tokens;
        }

        /*
        It’s like a conditional advance(). 
        We only consume the current character if it’s what we’re looking for.
        */
        private Boolean match(char expected) {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            Console.WriteLine("line 295 current " + current);
            return true;
        }

        public char advance() {
            current++;
            return source[current - 1];
        }

        private char peek() {
            if(isAtEnd()) return '\0';
            char currentChar = source[current];
            return currentChar;
        }

        private void handlestring() {
            Console.WriteLine("inside handlestring.");
            while(peek() != '"' && !isAtEnd()) {
                if(peek() == '\n') line++;
                advance();
            }

            if(isAtEnd()) {
                Lox.error(line, "unterminated string.");
                return;
            }

            advance();

            Console.WriteLine("start + 1: " + source[start + 1]);
            Console.WriteLine("current - 1: " + source[current - 1]);
        
            String value = source.Substring(start + 1, current - 1);
            Console.WriteLine("line 335: " + value);
            addToken(TokenType.STRING, value);
        }

        private void handleidentifier() {
           // while(Char.IsLetterOrDigit(peek())) advance();
           Console.WriteLine("line 341 inside handleidentifier.");
            while(isAlphaNumeric(peek())) advance();
            Console.WriteLine("line 343 start: " + start);
            Console.WriteLine("line 344 current: " + current);
            //string text = source.Substring(start, current - 1);
            string text = source.Substring(start, current - start);

            Console.WriteLine("line 347 text: " + text);
            if(!keywords.ContainsKey(text)) {
                Console.WriteLine("line 349 is identifier");
                addToken(TokenType.IDENTIFIER);
            } else {
                Console.WriteLine("line 352 is keyword");
                TokenType type = keywords[text];
                addToken(type);
            }
        }
        private Boolean isDigit(char c) {
            return c >= '0' && c <= '9';
        }

        private Boolean isAlpha(char c) {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c == '-');
        }

        /*
            looking past the decimal point requires a second character of
            lookahead since we don't want to consume the . until we're sure
            there is a digit after it. so we add peekNext()
        */
        private char peekNext() {
            if(current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private void handlenumber() {
            while(isDigit(peek())) advance();

            if(peek() == '.' && isDigit(peekNext())) {
                advance();

                while(isDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start)));
        }

        private Boolean isAlphaNumeric(char c) {
            return isAlpha(c) || isDigit(c);
            //return Char.IsLetterOrDigit(c);
        }
        public void addToken(TokenType tokenType) {
            addToken(tokenType, "");
        }

        public void addToken(TokenType tokenType, Object literal) {
            Console.WriteLine("line 398 start: " + start + " current: " + current);
                string text = source.Substring(start, current - start);
                Console.WriteLine("line 393 text: " + text);
                tokens.Add(new Token(tokenType, text, literal, line));
                foreach(Token t in tokens) {
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                    Console.WriteLine("line 401 lexeme: " + t.lexeme);
                    Console.WriteLine("line 401 type: " + t.type);
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                }
        }
    }

    class Parser {

    }

    public class Lox {

        private static Boolean hadError = false;

        public static void runFile(string file) {
            Console.WriteLine("running prompt");
            Console.WriteLine("file: " + file);
            string fileContent = System.IO.File.ReadAllText(file);
            run(fileContent);
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

            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");

        }

        public static void Main(String[] args) {
            Console.WriteLine("Starting compiler.");
            Console.WriteLine("args [0] " + args[0]);

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


        public static void error(int line, String message) {
            report(line, "", message);
        }

        public static void report(int line, String where, String message) {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}