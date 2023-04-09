namespace CompilerLearning {
    public class Token {
        // token typed specified by the enums.
        public TokenType type;
        // this is the text representation of the token.
        public string lexeme;

        // this could be a string, int or, double, etc.
        public Object literal;
        // line (in text file) the token is located.
        public int line;


        public Token(TokenType type, String lexeme, Object literal, int line) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }
        
        public override String ToString() {
            return type + " " + lexeme + " " + literal;
        }
    }
}