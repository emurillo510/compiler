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

    /*
        the parser we'll take tokens and transform them into an ever richer
        more complex representation.

        before we can produce that representation, we need to define it.

        we'll cover some theory around formal grammers, and feel the difference
        between functional and oop programming.

        go over some design patterns and do some meta programming.

        it should be simple for the parser to produce and easy for the interpreter
        to consume.

        
        post-order traversal

        chomsky hierarchy.

        context-free grammars

        the formalism we used for defining lexical grammar.
        (the rules for how characters get grouped into tokens was called regular language)

        but regular languages aren't powerful enough to handle expressions which
        can be nest arbitrarily deeply.

        we need a bigger hammer, and that hammer is a context-free grammer (CFG).
        it's the next heaviest tool in the toolbox of formal grammers.

        a formal grammar takes a set of atomic pieces it calls its "alphabets".
        Then it defines a (usually infinite) set of "strings" that are "in" in the
        grammar. each string is a sequence of "letters" in the alphabet.

        I'm using all those quotes because the terms get a little confusing as
        you move from lexical to syntatical grammars. in our scannr's grammar,
        the alphabet consists of individual characters and the strings are valid
        lexemes -- roughly "words".

        in the syntactic grammar we're talking about now, we're at a different
        granularity. now each "letter" in the alphabet is an entire token
        and a "string" is a sequence of tokens -- an entire expression.

        terminology | lexical grammar | syntactic grammar |
        the "alphabet" | characters | tokens |
        a "string" | lexeme or tokens | expression |
        it's implemented by the... | scanner | parser

        a formal grammar's job is to specify which strings are valid and which 
        aren't. if we were defining a grammar for english sentences 
        "eggs are tasty for breakfast" would be in the grammar, but
        "tasty breakfast for eggs" would probably not.

        Rules for grammars

        how do we write down a grammar that contains an infinite number of valid strings?
        we obviously can't list them all out. instead, we create a finite set of
        rules. you can think of them as a game that you can "play" in one of two directions

        if you start with the rules, you can use them to generate strings that are
        in the grammar. strings created this way are called derivations because each
        is "derived" from the rules of the grammar. in each step of the game, you pick
        a rule and follow what it tells you to do. most of the lingo around
        formal grammars come from playing them in this direction. rules are called
        productions because they produce strings in the gramamr.

        keywords
        - derivations
        - productions
        - head
        - body
        - terminal
        - nonterminal

        each production in a CFG has a head(it's name) and a body (which describe 
        what it generates). in it's pure form, the body is simply a list of symbols.
        symbols come in two delectable flavors:


        - a terminal

        a terminal is a letter from the grammar's alphabet. you can think of it
        like a literal value. in the syntactic grammar we're defining, the terminals
        are individual lexemes - tokens coming from the scannar like if or 1234.

        - a nonterminal is a named reference to another rule in the grammar.
        it means "play the rule and insert whatever it produces here."
        in this way, the grammar composes.

        there is one last refinement: you may have multiple rules with the same name.
        when you reach a nonterminal with that name, you are allowed to pick any of
        the rules for it, whichever floats your boat.

        to make this concrete, we need a way to write down these production rules.
        people have been trying to crystallize grammar all the way back to
        panini's astadhyayi which codified sankrit grammar a mere couple thousand
        years ago. not much progress happened until John Backus and company needed
        a notation for specifying ALGOL 58 and came up with the Backus-Naur form.
        Since then, nearly everyone uses some flavor of BNF, and tweaked to their
        own tastes.

        Each rule is a name, followed by an array ->
        followed by a seqeuence of symbols and finally ending with a semicolon.
        terminals are quoted strings and nonterminals are lowercase words.

        using that, here's a grammar for breakfast menus:

        breakfast -> protein "with" breakfast "on the side";
        breakfast -> protein;
        breakfast -> bread;

        protein -> crispiness "crispy" "bacon";
        protein -> "sausage";
        protein -> cooked "eggs";

        crispiness -> "really" ;
        crispiness -> "really" crispiness;

        cooked -> "scrambled";
        cooked -> "poached";
        cooked -> "fried";

        bread -> "toast";
        bread -> "biscuits";
        bread -> "English muffin";

        We can use this grammar to generate random breakfasts. 
        Let's play around and see how it works.
        by age-old convention, the game starts with teh first rule in the grammar.
        breakfast.

        there are three production rules for that, and we randomly pick up the
        first one.

        protein "with" breakfast "on the side".


        we need to expand theat first nonterminal, protein, so we pick a production 
        for that. let's picked:

        protein -> cooked "eggs";


        next, we need a production for cooked, and so we picked "poached". that's
        a terminal, so we add that. now our string looks like :

        "poached" "eggs" "with" breakfast "on the side"


        the next non-terminal is breakfast again. the first breakfast production
        we chose recursively refers back to the breakfast rule. recursion
        like this usually indicates that the langauge is context-free instead
        of regular. in inparticular, this kind of nested recursion where the
        recursive nonterminal has productions on both sides of it means that it's
        not regular.


        we could keep picking the first production for breakfats over and over again
        yielding all manner of breakfasts like "bacon with susage with scrambled eggs
        with bacon.." we won't though. this time we'll pick bread. there are three
        rules for that, each of which contains only a terminal. we'll pick
        "englsh muffin"

        with that, every nonterminal in string has been expanded until it finally
        contains only terminals and we're left with


        throw in some ham and hollandaise and yo've got eggs benedict.

        any time we hit a rule that had multiple productions, we just picked one 
        arbitrarily. it is this flexibility that allows a short number of grammar 
        rules to encode a combinatorially larger set of strings. the fact that
        a rule can refer to itself -- directly or indirectly -- kicks it up even more,
        letting us pack an infinite number of strings into a finite grammar.


        Enhancing our notation

        stuffing an infinite set of strings in a handful of rules is pretty
        fantastic, but let's take it further. our notation works, but it's
        a little tedious. so, like any good langauge designer, we'll sprinkle
        some syntactic sugar on top. in addition to terminals and nonterminals
        we'll allow a few other kinds of expressions in the body of a rule:

        

        instead of repeating the rule name each time we want to add another
        production for it, we'll allow a series of productions separated by a
        pipe (|):

        bread -> "toast" | "biscuits" | "English muffin"


        further, we'll allow parentheses for grouping and then allow | 
        within that to select one from a series of options within the middle
        of a production

        protein -> ("scrambled" | "poached" | "fried" ) "eggs";


        using recursion to support repeated sequences of symbols has a certain
        appealing purity, but it's kind of a chore to make a separate named
        sub-rule each time we want to loop. so, we also use a postfix *
        to allow the previous symbol or group to be repeated zero or more times.

        crispiness -> "really" "really"*;


        a postfix + is similar, but requires the preceding production to appear
        at least once


        cripiness -> "really"+ ;



        a postfix ? is for an optional production. the thing before it can appear
        zero or one time, but not more.


        breakfast -> protein ( "with" breakfast "on the side" )? ;


        with all those syntactic niceties, our breakfast grammar condenses
        down to:



        breakfast -> protein ( "with" breakfast "on the side" )?;
                     | bread;

        protein -> "really"+ "crispy" "bacon"
                | "sausage"
                | ("scrambled" | "poached" | "fried" ) "eggs";

        bread -> "toast" | "biscuits" | "English muffin";


        Not too bad, I hope. If you're used to grep or using regular expressions
        in your text editor, most of the punctuation should be familiar. 
        The main difference is that symbols here represent entire tokens, 
        not single characters.


        we'll use this notation throughout the rest of the book to precisely
        describe Lox's grammar. As you work on programming languages, you'll
        find context free gramamrs (using this or EBNF or some other notation)
        help you crystallize your informal syntax design ideas. they are also
        a handy medium for communicating with other language hackers about syntax).


        the rules and productions we define for Lox are also our guide to the tree
        data structure we're going to implement to represent code in memory.
        Before we can do that, we need an actual grammar for Lox, or at least
        enough for one for us to get started.





        A Grammar for Lox expressions

        In the previous chapter, we did Lox's entire lexical grammar in one fell
        swoop. Every keyword and a bit of punctuation is there.

        The syntactic grammar is larger, and it would be a real bore to grind
        through the entire thing before we actually get our interpreter up and
        running.

        Instead, we'll crank through a subset of the language in the next
        couple of chapters. Once we have that mini-language represented,
        parsed and interpreted, then later chapters will progressively
        add new features to it, including the new syntax. for now,
        we are only going to worry about a handful of expressions:



        - literals - numbers, strings, booleans and nil.
        - unary expressions - a prefix ! to perform a logical not and - to negate
        a number.
        - binary exprsesions - the infix arithmetic (+, -, *, /) and
        logic (==, !=, <, <=, >, >=) operators we know and love.

        - parentheses - a pair of ( and ) wrapped around an expression.



        example usage: 1 - (2 * 3) < 4 == false


        using our handy dandy new notation, here's a grammar for those:

        expression -> literal
                    | unary
                    | binary
                    | grouping;

        literal -> NUMBER | STRING | "true" | "false" | "nil";
        grouping -> "(" expression ")";
        unary -> ( "-" | "!" ) expression;
        binary -> expression operator expression;
        operator -> "==" | "!=" | "<" | "<=" | ">" | ">="
                    |"+"| "-" |"*"|"/";


        There's one bit of extra metasyntax here. In addition to
        quoted strings for terminals that match exact lexemes,
        we CAPITALIZE terminals that are a single lexeme
        whose text expresentation may vary. NUMBER is any number literal,
        and STRING is any string literal. Later, we'll do the same
        for IDENTIFIER.

        This grammar is actually ambiguous, which we'll see when we get to
        parsing it. But it's good enough for now.



        # Implementing Syntax Trees

        that little expression grammar is our skeleton. since the grammar
        is recursiv -- note how grouping, unary, and binary all refer back to
        expression, our data structure will form a tree. since this structure
        represents the syntax of our language, it's called a syntax tree.

        our scanner used a single Token class to represent all kinds of lexemes.
        to distinguish the different kinds - think the number 123 versus the string
        "123" - we included a simple TokenType enum. Syntax trees are not so
        homogeneous. Usary expressions have a single operand, binary expressions
        have two, and literals have none.

        we could much that all together into a single Expression class with an
        arbitrary list of children. some compilers do. but I like getting most
        out of Java's type system. So we'll define a base class for expresions.
        
        Then, for each kind of expression -- each production under expression --
        we create a subclass that has fields for the nonterminals specific of that
        rule.

        this way, we get a compile error if we, say, try to access the second
        operand of an  unary expression.
    */
    
    /*
    
        Expr is the base class that expression classes inherit from.
        As you can see from Binary, the subclasses are nested inside of it.
        There's no technical need for this, but it lets us cram all of the
        classes into a single Java file.

        # Disoriented objects

        You'll note that, much like the Token class, there aren't any methods
        here. It's a dumb structure. Nicely typed, but merely a bag of data.
        This feels strange in an object-oriented language like Java.
        Shouldn't the class do stuff?

        The problem is that these tree classes aren't owned by any single domain.
        should they have methods for parsing since that's where
        the trees are created? or interpreting since that's where they are
        consumed? Trees span the border between those territories, which mean
        they are really owned by neither.

        in fact, these types exist to enable the parser and interpreter to
        communicate. that lends itself to types that are simply data
        with no associated behavior. this style is very natural in functional
        languages like Lisp and ML where all data is separated from behavior,
        but it feels odd in Java.

        Functional programming aficionados right now are jumping to explain
        "See! OOP langauges are a bad fit for an interpreter!" I won't go that far.
        you'll recall that the scanner itself was admirably suited to 
        object-orientation. It had all of the mutable state to keep track of where it was
        in the source code, a well-defined set of public methods, and a handeful of private
        helpers.

        My feeling is taht each phase or part of the interpreter works fine in an object-oriented
        style. It is the data structures taht flow between them that are stripped of behavior.



        #   Metaprogramming the trees

        Java can express behavior-less classes, but I wouldn't say that it's
        particularly great at it. Eleven lines of code to stuff three
        fields in an object is pretty tedious and when we're all done, we're
        going to have 21 of these classes.

        I don't want to waste your time or my ink writing all that down.
        Really, what is the essence of each subclass? A name, and a list of
        typed fields. that's it. we're smart language hackers, right? let's
        automate.


        instead of tediously hand-writing each class definition, field declaration,
        constructor, and initializer, we'll hack together a script that does it for us.
        it has a description of each tree type- its name and fields - and it prints
        out the Java code needed to define a class with that name and state.


        this script is a tiny Java command-line app that generates a field named Expr.java


        ```
        package com.craftinginterpreters.tools;

        public class GenerateAst {
            public static void main(String[] args) { throws IOException
                if(args.length != 1) {
                    System.err.println("Usage: generate_ast <output directory>");
                    System.exit(64);
                }

                String outputDir = args[0];
            }
        }
        ```


        this script isn't part of the interpreter itself. it's a tool we, the people
        hacking on the interpreter, run ourselves to generate the syntax tree 
        classes. when it's done, we treat "Expr.java" like any other file
        in the implementation. We are merely automating how that file gets authored.

        To generate the classes, it needs to have some description of each type
        and its fields.


        string outputDir = args[0];

        defineAst(outputDir, "Expr", Arrays.asList(
            "Binary : Expr left, Token op, Expr right",
            "Grouping : Expr expression,
            "Literal : Object value",
            "Unary : Token op, Expr right" 
        ));



        For brevity's sake, I jammed the descriptions of the expression types
        into strings. Each is the name of the class followed by : and the
        list of fields, separated by commas. Each field has a type and name.

        The first thing defineAst() needs to do is output the base Expr class.

```
        private state void defineAst(
            String outputDir, String baseName, List<String> types) throws IOException {

                String path = outputDir  + "/" + baseName + ".java";
                PrintWriter writer = new PrintWriter(path, "UTF-8");

                writer.println("package com.craftinginterpreters.lox;");
                writer.println();
                writer.println("import java.util.List;");
                writer.println();
                writer.println("abstract class " + baseName + " {");

                writer.println("}");
                writer.close();
            }
        )
```

    when we call this baseName is "Expr", which is both the name of the class and
    the name of the file it outputs. We pass this as an argumetn instead of hardcoding
    the name because we'll add a separate family of classes later for statements.

    Inside the base class, we define each subclass.

    
    writer.println("abstract class " + baseName + " {");

    // The AST classes.
    for(String type : types) {
        String className = type.split(":")[0].trim();
        String fields = type.split(":")[1].trim();
        defineType(writer, baseName, className, fields);
    }

    writer.println("}");




    That code in turn calls:


    private static void defineType()



    There we go. All of that glorious Java boilerplate is done. It declares
    each field in the class body. It defines a constructor for the class with
    parameters for each field and initializes them in the body.

    Compile and run this Java program now and it blasts out a new ".java"
    file containing a few dozen lines of code. That file's about to get
    even longer.

    
    # Working wtih Trees


    put on your imagination hat for a moment. even though we aren't there yet, consider what the interpreter will do with the syntax
    trees. each kind of expression in Lox behaves differently at runtime. That means that the interpreter needs to select a different
    chunk of code to handle each expression type. with tokens, we can simply switch on the TokenType. But we don't have a "type"
    enum for the syntax trees, just a separate Java class for each one.

    we could write a long chain of type test:

    if(expr instanceof Expr.Binary) {
        //..
    } else if(expr instanceof.Expr.Grouping) {
        //..
    } else {
        //..
    }

    but all of those sequential type tests are slow. Expression types whose names are alphabetically later would take longer to execute
    because they'd fall through more if cases before finding the right type. that's not my idea of an elegant solution.

    we have a family of classes and we need to associate a chunk of behavior with each one. the natural solution in an object-oriented
    language like Java is to put the behavior into methods on the classes themselves. we could add an abstract interpret() method 
    on Expr which each subclass then implements to interpret itself.

    This works alright for tiny projects, but it scales poorly. like I noted before, these tree classes span a few domains. at the very,
    least both the parser and interpreter will mess with them. as you'll see later, we need to do name resolution on them.
    if our langauge was statically typed, we'd have a type checking pass.

    if we added instance methods to do the expression classes for everyone of those operations, that would smush a bunch of different
    domains together. that violates separation of concerns and leads to hard to maintain code.



    # The Expression Problem

    This problem is more fundamental than it may seem at first. We have a handful of types, and a handful of high level
    operations like "interpret". For each pair of type and operation, we need a specific implementation. Picture a table:

             interpret() resolve() analyze()
    Binary
    Grouping
    Literal
    Unary

    Rows are types, and columns are operations. Each cell represents the unique piece of code to implement that operation on that type.




    An object-oriented languages like Java assumes that all the code in one row naturally hangs together. It figures all the things
    you do with a type are likely related to each other, and the language makes it easy to define them together as methods inside
    the same class.


    classes

    new class



    This makes it easy to extend the table by adding new rows. simply define a new class. No existing code has to be touched. but imagine
    if you want to add a new operation -- a new column. In Java, that means cracking open each of those existing classes and adding a 
    method to it.


    Functional paradigm languages in the ML family flip that around. There, you don't have classes with methods. Types and functions
    are totally distinct. To implement an operation for a number of different types, you define a single function. In the body of that
    function, you use pattern matching -- sort of a type based switch on steriods -- to implement the operation for each type all in
    one place.

    This makes it trivial to add a new operations -- simply define another function that pattern matches on all of the types.

    [...] [...] [...]           [...]
    [pattern matching functions]  new function


    but, conversely, adding a new type is hard. you have to go back and add a new case to all of the pattern matches in all of the
    existing functions.


    each style has a certain "grain" to it. that's what the paradigm name literally says -- an object-oriented language
    wants you to orient your code along the rows of types. a functional language instead encourages you to lump each column's
    worth of code together into functions.


    a bunch of smart language nerds noticed that neither style made it easy to add both rows and columns to the table. they called
    this difficulty the "expression problem" because -- like we are now -- they first ran into it when they were trying to figure
    out the best way to model expression syntax tree nodes in a compiler.


    people have thrown all sorts of langauge features, design patterns and programming tricks to try and knock that problem down
    but no perfect language has finished it off yet. in the meantime, the best we can do is try and pick a language whose orientation
    matches the natural architectural seams in the programming we're writing.

    object-orientation works fine for many parts of our interpreter, but these tree classes rub against the grain of Java.
    Fortunately, there's a design pattern we can bring to bear on it.




    # The visitor pattern

    the visitor pattern is the most widely misunderstood pattern in
    all of Design Patterns, which is really saying something when you look at the
    software architecture excesses of the past couple of decades.

    The trouble starts with terminology. The pattern isn't about "visiting"
    and the "accept" method in it doesn't conjure up any helpful imagery either.
    many think the pattern has to do with traversing trees, which isn't the
    case at all. we are going to use it on a set of classes that are tree-like,
    but that's a coincidence. as you'll see, the pattern works as well on a 
    single object.

    the visitor pattern is really about approximating the functional style within
    an OOP langauge. it lets us add a new column to that table easily.
    we can define all of the behavior for a new operation on a set of types
    in one place, without having to touch the types themselves. it does this
    the same way we solve most every problem in computer science: by adding
    a layer of indirection.

    before we apply it to our auto-generated expr classes, let's walk through
    a simpler example. say we have to kinds of pastries: beignets and crullers.


    abstract class Pastry {
    }

    class Beignet extends Pastry {

    }

    class Cruller extends Pastry {

    }

    we want to be able to define new pastry operations -- cooking them,
    eating them, decorating them, etc... -- without having to add a new method
    to each class everytime.

    Here's how we do it. First, we define a separate interface:

    interface PastryVistor {
        void visitBeignet(Beignet beignet);
        void visitCruller(Cruller cruller);
    }



    each operation that can be performed on pastries is a new class that
    implements that interface. it has a concrete method for each type of
    pastry. that keep sthe code for the operation on both types all nestled
    snugly together in one class.

    given some pastry, how do we route it to the correct method on the visitor
    based on its type? polymorphism to the rescue! we add this method to pastry:

    abstract class Pastry {
        abstract void accept(PastryVisitor vistor);
    }

    Each subclass implements it:

    class Beignet extends Pastry {
        @Override
        void accept(PastryVisitor vistor) {
            visitor.visitBeignet(this);
        }
    }

    class Cruller extends Pastry {
        @Override
        void accept(PastryVisitor vistor) {
            visitor.visitCruller(this);
        }
    }


    to perform an operation on a pastry, we call its accept() method
    and pass in the visitor for the operation we want to execute. the pastry
    -- the specific subclass's overriding implementation of accept()-- turns
    around and calls the appropriate visit method on the visitor and passes
    itself to it.

    that's the heart of the trick right there. it lets us use polymorphic
    dispatch on the pastry classes to select the appropriate method on the
    visitor class. in the table, each pastry class is a row, but if you look
    at all of the methods for a single visitor, they form a column.


    beignet accept() -> visitBeignet()
    cruller accept() -> visitCruller()

    we added one accept method() to each class, and we can use it
    for as many visitors as we want without ever having to touch the pastry
    classes again. it's a clever pattern.

    Visitors for expressions

    OK, let's weave it into our expression classes. we'll also refine
    the pattern a little. in the pastry example, the visit and accept() 
    methods don't return anything. in practice, visitors often want to
    define operations that produce values. but what return type should
    accept() have? we can't assume every visitor class wants to produce
    the same type, so we'll use generics to let each implementation
    fill in a return type

    first, we define the visitor interface. again, we nest it inside the
    base class so that we can keep everything in one file.


    writer.println("abstract class " + baseName + " {");

    defineVisitor(writer, baseName, types);

    // The AST classes

    private static void defineVisitor(
        PrintWriter writer, String baseName, List<String> types) {

            writerln.println(" interface Visitor<R> {");

            for(String type : types) {
                String typeName = type.split(":")[0].trim();
                writer.println(" R visit" + typeName + baseName + "("
                    + typeName + " " + baseName.toLowerCase() + ");");
            }

            writer.println(" }");
        }
    )


    here, we iterate through all of the subclasses and declare a visit method
    for each one. when we define new expression types later, this will
    automatically include them.

    Inside the base class, we define the abstract accept() method.

        defineType(writer, baseName, className, fields);
    }

    // the base accept() method.
    writer.println();
    writer.println(" abstract<R> R accept(Visitor<R> visitor);");
    writer.println("}");
    writer.println("  }");

    // Visitor pattern.
    writer.println();
    writer.println(" @Override");
    writer.println(" <R> R accept(Visitor<R> visitor) {");
    writer.println(" return visitor.visit" + 
        className + baseName + "(this);");
    writer.println(" }");

    // Fields.

    There we go. Now we can define operations on expressions without having to
    muck with the classes or our generator script. Compile and run this generator
    script to output an updated "Expr.java" file. It contains a generated
    Visitor interface and a set of expression node classes that support the
    Visitor pattern using it.

    Before we end this rambling chaper, let's implement that Visitor interface
    and see the pattern in action.



    # A (Not Very) Pretty Printer

    When we debug our parser and interpreter, it's often useful to look at a
    parsed syntax tree and make sure it has the structure we expect. we could
    inspect it in the debugger, but that can be a chore.

    instead, we'd like some code that, given a syntax tree, produces an
    unambigious string representation of it. converting a tree to a string is
    sort of the opposite of a parser, and is often called "pretty printing"
    when the goal is to produce a string of text that is valid syntax in the
    source language.

    that's not our goal here. we want the string to very explicitly show the
    nesting structure of the tree. a printer that returned 1 + 2 * 3
    isn't super helpful if what we're trying to debug is whether operator 
    precedence is handled correctly.
    we want to know if the + or * is at the top of the tree.

    to that end, the string representation we produce isn't going to be Lox
    syntax. instead, it will look a lot like, well Lisp. Expression is
    explicitly parenthesized, and all of its subexpressions and tokens are
    contained in that.

    Give a syntax tree like:

        *
       /  \
    -      ()
    /        /
    123     45.67


it produces:

(* (-123) (group 45.67))


Not exactly "pretty", but it does show the nesting and grouping explicitly.
to implement this, we define a new class.


package com.craftinginterpreters.lox;

class AstPrinter implements Expr.Visitor<String> {
    String print(Expr expr) {
        return expr.accept(this);
    }
}


as you can see, it implements the visitor interface. that means we need
visit methods for each of the expression types we have so far.


@Override
public String visitBinaryExpr(Expr.Binary expr) {
    return parenthesize(expr.operator.lexme, expr.left, expr.right);
}

@Override
public String visitGroupingExpr(Expr.Grouping expr) {
    return parenthesize("group", expr.expression);
}

@Override
public String visitLiteralExpr(Expr.Literal expr) {
    if(expr.value == null) return "nil";
    return expr.value.toString();
}

@Override
public String visitUnaryExpr(Expr.Unary expr) {
    return parenthesize(expr.operator.lexeme, expr.right);
}
}

Literal expressions are easy -- they convert the value to a string with a little
check to handle Java's null standing in for Lox's nil. The other expressions
have subexpressions, so they use this parenthesize() helper method


private String parenthesize(String name, Expr... exprs) {
    StringBuilder builder = new StringBuilder();

    builder.append("(").append(name);
    for(Expr expr: exprs) {
        builder.append(" ");
        builder.append(expr.accept(this));
    }

    builder.append(")");

    return builder.toString();
}


It takes a name and a list of subexpressions and wraps them all up in parentheses,
yielding a string like :

(+ 1 2)


Note that it calls accept() on each subexpression and passes in itself. This
is the recursive step taht lets us print an entire tree.

we don't have a parser yet, so it's hard to see this in action. for now,
we'll hack together a little main() method that manually instantiates a tree
and prints it:

public static void main(String[] args) {
    Expr expression = new Expr.Binary(
        new Expr.Unary(
            new Token(TokenType.MINUS, "-", null, 1),
            new Expr.Literal(123)),
        new Token(TokenType.STAR, "*", null, 1),
        new Expr.Grouping(
            new Expr.Literal(45.67)));
        )
    
    System.out.println(new AstPrinter().print(expression));
}


if we did everything right, it prints:
(* (- 123) (group 45.67))


you can go ahead and delete this method. we won't need it. also, 
as we add new syntax tree types, I won't bother showing the necessary visit methods
for them in AstPrinter. If you want to (and you want the Java compiler to
not yet all you), go ahead and add them yourself. It will come
in handy in the next chapter we start parting Lox code into Syntax trees.
Or, if you don't care to maintain AstPrinter, feel free to delete it. 
We won't need it again.

    */
    public abstract class Expr {
        public class Binary : Expr  {
            private readonly Expr left;
            private readonly Token op;
            private readonly Expr right;

            public Binary(Expr left, Token op, Expr right) {
                this.left = left;
                this.op = op;
                this.right = right;
            }
        } 
    }


    /*
        # Parsing Expressions

        this chapter marks the first major milestone of the book. many of us have cobbled together a mishmash
        of regular expressions and substring operations to extract some sense out of a pile of text. the code was probably
        riddled with bugs and a beast to maintain. 
        
        writing a real parser -- one with a decent error handling, a coherent internal
        structure, and the ability to robustly chew through a sophisticated syntax -- is considered a rare, impressive skill.
        in this chapter you will attain it.

        it's easiser than you think, partially because we front-loaded a lot of the hard work in the last chapter. 
        
        you already know your way around a formal grammar.
        you're familiar with syntax trees, and we have some java classes to represent them. the only remaining piece
        is parsing -- transmogrifying a sequence of tokens into one of those syntax trees.

        some CS textbook make a big deal out of parsers. in the 60's, computer scientists -- understandably tired of programming
        in assembly langauges -- started designing more sophisticated, human-friendly languages like FORTRAN and ALGOL. 
        alas, they weren't very machine-friendly for the primitive computers of the time.

        these pioneers designed languages that they honestly weren't even sure how to write compilers for, and then did
        ground-breaking work inventing parsing and compiling techniques that could handle these new big languages on those
        old tiny machines.

        class compiler books read like fawning hagiographies of these heroes and their tools. the cover of the dragon book
        labeled "complexity of compiler design" being slain by a knight bearing a sword and shield branded "LALR parser generator"
        and "syntax-directed translation". They laid on it thick.

        A little self-congratulation is well-deserved, but the truth is you don't need to know most of that stuff to bang out a 
        high quality parser for a modern machine. as always, I encourage you to broaden your education and take it in later, but
        this book omits the trophy case.

        # Ambiguity and the Parsing Game

        In the last chapter, I said you can "play" a context free grammar like a game in order to generate strings. Parsers
        play that game in reverse. Given a string -- a series of tokens -- we map those tokens to terminals in the grammar
        to figure out which rules could have generated that string.

        the "could have" part is interesting. It's entirely possible to create a grammar that is ambiguous, where different choices
        of productions can lead to the same string. when you're using the grammar to generate strings, that doesn't matter much.
        once you have the string, who cares how you got to it?

        when parsing, ambiguity means the parser may misunderstand the user's code. as we parse, we aren't just determining if the
        strings is valid Lox code, we're also tracking which rules match which parts of it so that we know what part of the langauge
        each token belongs to. here's the lox expression gramma we put together in the last chapter:


         expression -> literal
                    |  unary
                    |  binary
                    |  grouping;

        literal -> NUMBER | STRING | "true" | "false" | "nil";
        grouping -> "(" expression ")";
        unary -> ( "-" | "!" ) expression;
        binary -> expression operator expression;
        operator -> "==" | "!=" | "<" | "<=" | ">" | ">="
                    |"+"| "-" |"*"|"/";


        this is a valid string in that grammar:

        6 / 3 - 1

        But there are two ways we could have generated it. One way is:
        
        1. Starting at expression, pick binary.
        2. For the left-hand expression, pick NUMBER, and use 6.
        3. For the operator, pick "/".
        4. For the right-hand expression, pick binary again.
        5. In that nested binary expression, pick 3 - 1.


        Another is:
        1. Starting at expression, pick binary.
        2. For the left-hand expression, pick binary again.
        3. In that nested binary expression, pick 6 / 3.
        4. Back at the outer binary, for the operator, pick "-".
        5. For the right-hand expression, pick NUMBER, and use 1.


        those produce the same strings, but not the same syntax trees.

        In other words, the grammar allows seeing the expression as (6 / 3) - 1 or
        6 / (3 - 1). 
        
        The binary rule lets operands nest any which way you want.

        That in turn affects the result of evaluating the parsed tree. 
        
        The way mathematicians have addressed this ambiguity since blackboards were first
        invented is by defining rules for precedence and associativity.

        Precedence determines which operator is evaluated first in an expression
        containing a mixture of different operators. Precedence rules tell us that we
        evaluate the / before the - in the above example. Operators with higher
        precedence are evaluated before operators with lower precedence.
        Equivalently, higher precedence operators are said to “bind tighter”.

        Associativity determines which operator is evaluated first in a series of the
        same operator. When an operator is left-associative (think “left-to-right”),
        operators on the left evaluate before those on the right. Since - is left associative, this expression:

        5 - 3 - 1

        is equivalent to:
        
        (5 - 3) - 1


        Assignment, on the other hand, is right-associative. This:

        a = b = c
        
        is equivalent to:
        
        a = (b = c)


        Without well-defined precedence and associativity, an expression that uses
        multiple operators is ambiguous—it can be parsed into different syntax trees,
        which could in turn evaluate to different results. We’ll fix that in Lox by
        applying the same precedence rules as C, going from lowest to highest:

        Name Operators Associates
        Equality == != Left
        Comparison > >= < <= Left
        Term - + Left
        Factor / * Left
        Unary ! - Right



        Right now, the grammar stuffs all expression types into a single expression
        rule. That same rule is used as the non-terminal for operands, which lets the
        grammar accept any kind of expression as a subexpression, regardless of
        whether the precedence rules allow it.


        We fix that by stratifying the grammar. We define a separate rule for each
        precedence level.

        expression → ...
        equality → ...
        comparison → ...
        term → ...
        factor → ...
        unary → ...
        primary → ...


        Each rule here only matches expressions at its precedence level or higher. For
        example, 
        
        unary matches a unary expression like !negated or a primary
        expression like 1234. 
        
        And term can match 1 + 2 but also 3 * 4 / 5. The
        final primary rule covers the highest-precedence forms—literals and
        parenthesized expressions.


        We just need to fill in the productions for each of those rules. We’ll do the easy
        ones first. The top expression rule matches any expression at any precedence
        level. 
        
        Since equality has the lowest precedence, if we match that, then it
        covers everything:
        
        expression → equality
        
        Over at the other end of the precedence table, a primary expression contains all
        the literals and grouping expressions.


        primary → NUMBER | STRING | "true" | "false" | "nil"
        | "(" expression ")" ;


        A unary expression starts with a unary operator followed by the operand. Since
        unary operators can nest— !!true is a valid if weird expression—the operand
        can itself be a unary operator. A recursive rule handles that nicely.

        unary → ( "!" | "-" ) unary ;


        But this rule has a problem. It never terminates. Remember, each rule needs to
        match expressions at that precedence level or higher, so we also need to let this
        match a primary expression.

        unary → ( "!" | "-" ) unary
        | primary ;


        That works.




        The remaining rules are all binary operators. We’ll start with the rule for
        multiplication and division. Here’s a first try:
        
        factor → factor ( "/" | "*" ) unary
            | unary ;

        The rule recurses to match the left operand. That enables the rule to match a
        series of multiplication and division expressions like 1 * 2 / 3. 

        Putting the recursive production on the left side and unary on the 
        right makes the rule left-associative and unambiguous.

        All of this is correct, but the fact that the first nonterminal in the body of the
        rule is the same as the head of the rule means this production is left-recursive.

        Some parsing techniques, including the one we’re going to use, have trouble
        with left recursion. 
        
        (Recursion elsewhere, like we have in unary and the indirect recursion 
        for grouping in primary are not a problem.)


        There are many grammars you can define that match the same language. 
        The choice for how to model a particular language is partially a matter of taste and
        partially a pragmatic one. 
        
        This rule is correct, but not optimal for how we intend to parse it. 
        Instead of a left recursive rule, we’ll use a different one.
        
        factor → unary ( ( "/" | "*" ) unary )* ;

        We define a factor expression as a flat sequence of multiplications and divisions.
        This matches the same syntax as the previous rule, but better mirrors the code
        we’ll write to parse Lox. 
        
        We use the same structure for all of the other binary operator precedence levels, 
        giving us this complete expression grammar:

        expression → equality ;
        equality → comparison ( ( "!=" | "==" ) comparison )* ;
        comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
        term → factor ( ( "-" | "+" ) factor )* ;
        factor → unary ( ( "/" | "*" ) unary )* ;
        unary → ( "!" | "-" ) unary
            | primary ;
        primary → NUMBER | STRING | "true" | "false" | "nil"
            | "(" expression ")" ;

        This grammar is more complex than the one we had before, but in return we
        have eliminated the previous one’s ambiguity. It’s just what we need to make a parser




        # Recursive Descent Parsing
        
        There is a whole pack of parsing techniques whose names are mostly
        combinations of “L” and “R”—LL(k), LR(1), LALR—along with more exotic
        beasts like parser combinators, Earley parsers, the shunting yard algorithm, and
        packrat parsing. 
        
        For our first interpreter, one technique is more than sufficient recursive descent.

        Recursive descent is the simplest way to build a parser, and doesn’t require
        using complex parser generator tools like Yacc, Bison or ANTLR. 
        
        All you need is straightforward hand-written code. 
        
        Don’t be fooled by its simplicity, though. Recursive descent parsers 
        are fast, robust, and can support sophisticated error handling.
        
        In fact, GCC, V8 (the JavaScript VM in Chrome), 
        Roslyn (the C# compiler written in C#) and 
        many other heavyweight production language 
        implementations use recursive descent. It rocks.


        Recursive descent is considered a top-down parser because it starts from the
        top or outermost grammar rule (here expression) and works its way down
        into the nested subexpressions before finally reaching the leaves of the syntax
        tree. 
        
        This is in contrast with bottom-up parsers like LR that start with primary
        expressions and compose them into larger and larger chunks of syntax.

        A recursive descent parser is a literal translation of the grammar’s rules straight
        into imperative code. 
        
        Each rule becomes a function. The body of the rule translates to code roughly like:

        ## Grammar notation Code representation

        Terminal - Code to match and consume a token
        Nonterminal - Call to that rule’s function
        | - if or switch statement
        * or + - while or for loop
        ? - if statement

        The “recursive” part of recursive descent is because when a grammar rule refers
        to itself—directly or indirectly—that translates to a recursive function call.

        The parser class

        Each grammar rule becomes a method inside this new class.

        package com.craftinginterpreters.lox;

        import java.util.List;

        import static com.craftinginterpreters.lox.TokenType.*;

        class Parser {
            private final List<Token> tokens;
            private int current = 0;

            Parser(List<Token> tokens) {
                this.tokens = tokens;
            }
        }

        Like the scanner, the parser consumes a flat input sequence, only now we're
        reading tokens instead of characters.

        We store the list of tokens and use current to point to the next token eagarly
        waiting to be parsed.

        we're going to run straight through the expression grammar now and translate
        each rule to Java code. This first rule, expression, simply expands to the
        equality rule, so that's straightforward.

        private Expr expression() {
            return equality();
        }

        
        Each method for parsing a grammar rule produces a syntax tree for that rule
        and returns it to the caller. 
        
        When the body of the rule contains a nonterminal—a reference to another 
        rule—we call that other rule’s method.


        The rule for equality is a little more complex.

        equality → comparison ( ( "!=" | "==" ) comparison )* ;


        In Java, that becomes:

        private Expr equality() {
            Expr expr = comparison();
            
            while (match(BANG_EQUAL, EQUAL_EQUAL)) {
                Token operator = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, operator, right);
            }
            return expr;
        }

        Let’s step through it. 
        
        The first comparison nonterminal in the body translates to the first call to 
        comparison() in the method. We take that result and store it in a local variable.


        Then, the ( ... )* loop in the rule maps to a while loop. We need to know
        when to exit that loop. 
        
        We can see that inside the rule, we must first find either a != or == token. 
        
        So, if we don’t see one of those, we must be done with the sequence of 
        equality operators. We express that check using a handy match() method.






        private boolean match(TokenType... types) {
            for (TokenType type : types) {
                if (check(type)) {
                    advance();
                    return true;
                }
            }
            return false;
        }


        This checks to see if the current token has any of the given types.
        
        If so, it consumes the token and returns true.
        Otherwise, it returns false and leaves the current token alone.
        The match() method is defined in terms of two more fundamental operations.

        private boolean check(TokenType type) {
            if (isAtEnd()) return false;
            return peek().type == type;
        }


        The check() method returns true if the current token is of the given type.
        Unlike match(), it never consumes the token, it only looks at it.

        private Token advance() {
            if (!isAtEnd()) current++;
            return previous();
        }

        The advance() method consumes the current token and returns it, similar to
        how our scanner’s corresponding method crawled through characters.
        
        
        These methods bottom out on the last handful of primitive operations.

        private boolean isAtEnd() {
            
            return peek().type == EOF;
        }

        private Token peek() {
            return tokens.get(current);
        }

        private Token previous() {
            return tokens.get(current - 1);
        }





        isAtEnd() checks if we’ve run out of tokens to parse. 
        
        peek() returns the current token we have yet to consume and 
        
        previous() returns the most recently consumed token. 
        
        The latter makes it easier to use match() and then access the just-matched token.


        That’s most of the parsing infrastructure we need. Where were we? 
        Right, so if we are inside the while loop in equality(), then we know we have 
        found a != or == operator and must be parsing an equality expression.


        
        We grab the matched operator token so we can track which kind of equality
        expression we have. 
        
        Then we call comparison() again to parse the right-hand operand. 
        
        We combine the operator and its two operands into a new Expr.
        
        Binary syntax tree node, and then loop around. 
        
        Each iteration, we store the resulting expression back in the same expr 
        local variable. 
        
        As we zip through a sequence of equality expressions, 
        that creates a left-associative nested tree of binary operator nodes.


        
        
        The parser falls out of the loop once it hits a token that’s not an equality
        operator. Finally, it returns the expression. 
        
        Note that if the parser never encounters an equality operator, 
        then it never enters the loop. In that case, the equality() method effectively 
        calls and returns comparison(). In that way, this method matches an 
        equality operator or anything of higher precedence.



        Moving on to the next rule…
        
        comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
        
        Translated to Java:
        
            private Expr comparison() {
            
                Expr expr = term();
                while (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL)) {
                    Token operator = previous();
                    Expr right = term();
                    expr = new Expr.Binary(expr, operator, right);
                }

                return expr;
            }

        And finally multiplication and division:


        private Expr factor() {
            
            Expr expr = unary();
            while (match(SLASH, STAR)) {
                Token operator = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, operator, right);
            }
            return expr;
        }


        That’s all of the binary operators, parsed with the correct precedence and
        associativity. 
        
        We’re crawling up the precedence hierarchy and now we’ve reached the unary operators.

        unary → ( "!" | "-" ) unary
        | primary ;

        The code for this is a little different.
        
        private Expr unary() {
            if (match(BANG, MINUS)) {
                Token operator = previous();
                Expr right = unary();

                return new Expr.Unary(operator, right);
            }
            return primary();
        }


        Again, we look at the current token to see how to parse. 
        
        If it’s a ! or -, we must have a unary expression. 
        
        In that case, we grab the token, and then recursively call unary() 
        again to parse the operand. 
        
        Wrap that all up in a unary expression syntax tree and we’re done.


        Otherwise, we must have reached the highest level of precedence, primary
        expressions.

        primary → NUMBER | STRING | "true" | "false" | "nil"
            | "(" expression ")" ;

        Most of the cases for the rule are single terminals, so parsing is straightforward.


        private Expr primary() {
            if (match(FALSE)) return new Expr.Literal(false);
            if (match(TRUE)) return new Expr.Literal(true);
            if (match(NIL)) return new Expr.Literal(null);
            if (match(NUMBER, STRING)) {
                return new Expr.Literal(previous().literal);
            }
            
            if (match(LEFT_PAREN)) {
                Expr expr = expression();
                consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
        }



        The interesting branch is the one for handling parentheses. 
        
        After we match an opening ( and parse the expression inside it, 
        we must find a ) token. If we don’t, that’s an error.



        # Syntax Errors

        
        A parser really has two jobs:
        
        1. Given a valid sequence of tokens, produce a corresponding syntax tree.
        2. Given an invalid sequence of tokens, detect any errors and tell the user about
        their mistakes.



        Don’t underestimate how important the second job is! In modern IDEs and
        editors, the parser is constantly reparsing code—often while the user is still
        editing it—in order to syntax highlight and support things like auto-complete.
        
        That means it will encounter code in incomplete, half-wrong states all the time.
        
        When the user doesn’t realize the syntax is wrong, it is up to the parser to help
        guide them back onto the right path. 
        
        The way it reports errors is a large part of your language’s user interface. 
        Good syntax error handling is hard. 
        
        By definition, the code isn’t in a well-defined state, 
        so there’s no infallible way to know what the user meant to write. 
        
        The parser can’t read your mind. There are a couple of hard requirements 
        for when the parser runs into a syntax error:



        There are a couple of hard requirements for when the parser runs into a syntax
        error:


        It must detect and report the error. If it doesn’t detect the error and passes
        the resulting malformed syntax tree on to the interpreter, all manner of
        horrors may be summoned.

        It must not crash or hang. Syntax errors are a fact of life and language tools
        have to be robust in the face of them. Segfaulting or getting stuck in an
        infinite loop isn’t allowed. While the source may not be valid code, it’s still a
        valid input to the parser because users use the parser to learn what syntax is
        allowed.

        Those are the table stakes if you want to get in the parser game at all, but you
        really want to raise the ante beyond that. A decent parser should:
        
        Be fast. Computers are thousands of times faster than they were when
        parser technology was first invented. The days of needing to optimize your
        parser so that it could get through an entire source file during a coffee break are over. 
        But programmer expectations have risen as quickly, if not faster. 
        They expect their editors to reparse files in milliseconds after every
        keystroke. Report as many distinct errors as there are. 
        
        Aborting after the first error is easy to implement, but it’s annoying for users if every time 
        they fix what they think is the one error in a file, a new one appears. They want to see
        them all. Minimize cascaded errors. Once a single error is found, the parser no longer
        really knows what’s going on. It tries to get itself back on track and keep
        going, but if it gets confused, it may report a slew of ghost errors that don’t
        indicate other real problems in the code. 
        
        When the first error is fixed, those phantoms disappear, because they reflect only the parser’s own confusion.
        Cascaded errors are annoying because they can scare the user into thinking their code is in a worse state than it is.
        
        The last two points are in tension. We want to report as many separate errors as
        we can, but we don’t want to report ones that are merely side effects of an
        earlier one.
        
        The way a parser responds to an error and keeps going to look for later errors is
        called error recovery. This was a hot research topic in the 60s. Back then, you’d
        hand a stack of punch cards to the secretary and come back the next day to see
        if the compiler succeeded. With an iteration loop that slow, you really wanted to
        find every single error in your code in one pass.
        
        Today, when parsers complete before you’ve even finished typing, it’s less of an
        issue. Simple, fast error recovery is fine.



        # Panic Mode Error Recovery


        Of all the recovery techniques devised in yesteryear, the one that best stood the
        test of time is called—somewhat alarmingly—panic mode. 
        
        As soon as the parser detects an error, it enters panic mode. It knows at least one token doesn’t
        make sense given its current state in the middle of some stack of grammar
        productions. Before it can get back to parsing, it needs to get its state and the sequence of
        forthcoming tokens aligned such that the next token does match the rule being
        parsed. This process is called synchronization.
        
        To do that, we select some rule in the grammar that will mark the
        synchronization point. The parser fixes its parsing state by jumping out of any
        nested productions until it gets back to that rule. Then it synchronizes the token
        stream by discarding tokens until it reaches one that can appear at that point in
        the rule.
        
        Any additional real syntax errors hiding in those discarded tokens aren’t
        reported, but it also means that any mistaken cascaded errors that are side
        effects of the initial error aren’t falsely reported either, which is a decent trade
        off.

        The traditional place in the grammar to synchronize is between statements. We
        don’t have those yet, so we won’t actually synchronize in this chapter, but we’ll
        get the machinery in place for later.

        # Entering Panic Mode


        Back before we went on this side trip around error recovery, we were writing
        the code to parse a parenthesized expression. After parsing the expression, it
        looks for the closing ) by calling consume(). Here, finally, is that method:

        private Token consume(TokenType type, String message) {
            if (check(type)) return advance();
            throw error(peek(), message);
        }

        It’s similar to match() in that it checks to see if the next token is of the
        expected type. If so, it consumes it and everything is groovy. If some other
        token is there, then we’ve hit an error. We report it by calling this:

        private ParseError error(Token token, String message) {
            Lox.error(token, message);
            return new ParseError();
        }

        
        First, that shows the error to the user by calling:
        
            static void error(Token token, String message) {
                if (token.type == TokenType.EOF) {
                    report(token.line, " at end", message);
                } else {
                    report(token.line, " at '" + token.lexeme + "'", message);
                }
            }
        
        This reports an error at a given token. It shows the token’s location and the
        token itself. This will come in handy later since we use tokens throughout the
        interpreter to track locations in code.
        After we report the error, the user knows about their mistake, but what does the
        parser do next? Back in error(), it creates and returns a ParseError, an
        instance of this new class:
        
        
        class Parser {
            private static class ParseError extends RuntimeException {}
            private final List<Token> tokens;


        This is a simple sentinel class we use to unwind the parser. The error()
        method returns it instead of throwing because we want to let the calling method
        inside the parser decide whether to unwind or not. Some parse errors occur in
        places where the parser isn’t likely to get into a weird state and we don’t need to
        synchronize. In those places, we simply report the error and keep on truckin’.
        For example, Lox limits the number of arguments you can pass to a function. If
        you pass too many, the parser needs to report that error, but it can and should
        simply keep on parsing the extra arguments instead of freaking out and going
        into panic mode.
        In our case, though, the syntax error is nasty enough that we want to panic and
        synchronize. Discarding tokens is pretty easy, but how do we synchronize the
        parser’s own state?

        # Synchronizing a recursive descent parser




        With recursive descent, the parser’s state—which rules it is in the middle of
recognizing—is not stored explicitly in fields. Instead, we use Java’s own call
stack to track what the parser is doing. Each rule in the middle of being parsed
is a call frame on the stack. In order to reset that state, we need to clear out
those call frames.
The natural way to do that in Java is exceptions. When we want to synchronize,
we throw that ParseError object. Higher up in the method for the grammar rule
we are synchronizing to, we’ll catch it. Since we synchronize on statement
boundaries, we’ll catch the exception there. After the exception is caught, the
parser is in the right state. All that’s left is to synchronize the tokens.
We want to discard tokens until we’re right at the beginning of the next
statement. That boundary is pretty easy to spot—it’s one of the main reasons
we picked it. After a semicolon, we’re probably finished with a statement. Most
statements start with a keyword— for, if, return, var, etc. When the next
token is any of those, we’re probably about to start a statement.
This method encapsulates that logic:
 
 
    private void synchronize() {
        advance();
    
        while (!isAtEnd()) {
            if (previous().type == SEMICOLON) 
                return;

            switch (peek().type) {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
            return;
            }
            advance();
        }
    }

It discards tokens until it thinks it found a statement boundary. After catching a
ParseError, we’ll call this and then we are hopefully back in sync. When it
works well, we have discarded tokens that would have likely caused cascaded
errors anyway and now we can parse the rest of the file starting at the next
statement.
Alas, we don’t get to see this method in action, since we don’t have statements
yet. We’ll get to that in a couple of chapters. For now, if an error occurs, we’ll
panic and unwind all the way to the top and stop parsing. Since we can only
parse a single expression anyway, that’s no big loss.
    

        # Wiring up the Parser

    We are mostly done parsing expressions now. There is one other place where
    we need to add a little error handling. As the parser descends through the
    parsing methods for each grammar rule, it eventually hits primary(). If none
    of the cases in there match, it means we are sitting on a token that can’t start an
    expression. We need to handle that error too.

    if (match(LEFT_PAREN)) {
        Expr expr = expression();
        consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
    }

    throw error(peek(), "Expect expression.");
    }

        With that, all that remains in the parser is to define an initial method to kick it
        off. That method is called, naturally enough, parse().

        Expr parse() {
            try {
                return expression();
            } catch (ParseError error) {
                return null;
            }
        }


We’ll revisit this method later when we add statements to the language. For
now, it parses a single expression and returns it. We also have some temporary
code to exit out of panic mode. Syntax error recovery is the parser’s job, so we
don’t want the ParseError exception to escape into the rest of the interpreter.
When a syntax error does occur, this method returns null. That’s OK. The
parser promises not to crash or hang on invalid syntax, but it doesn’t promise to
return a usable syntax tree if an error is found. As soon as the parser reports an
error, hadError gets set, and subsequent phases are skipped.
Finally, we can hook up our brand new parser to the main Lox class and try it
out. We still don’t have an interpreter so, for now, we’ll parse to a syntax tree
and then use the AstPrinter class from the last chapter to display it.
Delete the old code to print the scanned tokens and replace it with this:


 List<Token> tokens = scanner.scanTokens();
 Parser parser = new Parser(tokens);
 Expr expression = parser.parse();
 
     // Stop if there was a syntax error.
    if (hadError) return;
    System.out.println(new AstPrinter().print(expression));
    }
Congratulations, you have crossed the threshold! That really is all there is to
hand-writing a parser. We’ll extend the grammar in later chapters with
assignment, statements, and other stuff, but none of that is any more complex
than the binary operators we tackled here.
Fire up the interpreter and type in some expressions. See how it handles
precedence and associativity correctly? Not bad for less than 200 lines of code.





    */
    public class Parser {

        private class ParseError : Exception {}

        private List<Token> tokens;
        private int current = 0;

        Parser(List<Token> tokens) {
            this.tokens = tokens;
        }

        private Expr expression() {
            return equality();
        }

        private Expr equality() {
            Expr expr = comparison();
            while(match(TokenType.BANG_EQUAL), TokenType.EQUAL_EQUAL)) {
                Token op = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr comparison() {
            Expr expr = term();

            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token op = previous();
                Expr right = term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr term() {
            Expr expr = factor();

            while(match(TokenType.MINUS, TokenType.PLUS)) {
                Token op = previous();
                Expr right = factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr factor() {
            Expr expr = unary();

            while(match(TokenType.SLASH, TokenType.STAR)) {
                Token op = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr unary() {
            if(match(TokenType.BANG, TokenType.MINUS)) {
                Token op = previous();
                Expr right = unary();
                return Expr.unary(op, right);
            }
            return primary();
        }

        private Expr primary() {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);
            if (match(TokenType.NUMBER, TokenType.STRING)) {
                return new Expr.Literal(previous().literal);
            }
            
            if (match(TokenType.LEFT_PAREN)) {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
        }

        /* primitive operations below */
        private Boolean match(params TokenType[] types) {
            foreach(TokenType type in types) {
                if(check(type)) {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private Boolean check(TokenType type) {
            if(isAtEnd()) return false;
            return peek().type == type;
        }

        private Token advance() {
            if(!isAtEnd()) current++;
            return previous();
        }

        private Boolean isAtEnd() {
            return peek().type == TokenType.EOF;
        }

        private Token peek() {
            return tokens[current];
        }

        private Token previous() {
            return tokens[current - 1];
        }

        private Token consume(TokenType type, String message) {
            if(check(type)) return advance();

            throw error(peek(), message);
        }

        private ParseError error(Token token, String message) {
            Lox.error(token, message);
            return new ParseError();
        }

        private void synchronize() {
            advance();

            while(!isAtEnd()) {
                if(previous().type == TokenType.SEMICOLON) return;

                switch(peek().type) {
                    case TokenType.CLASS:
                    case TokenType.FUN: 
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                    default:
                        break;
                }
            }
        }
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
        /*
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
        */

        public static void error(int line, String message) {
            report(line, "", message);
        }

        public static void error(Token token, String message) {
            if(token.type == TokenType.EOF) {
                report(token.line, " at end", message);
            } else {
                report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        public static void report(int line, String where, String message) {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}