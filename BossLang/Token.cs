namespace BLang
{
    public enum TokenType
    {
        // Keywords
        IntKeyword, StringKeyword, Print,
        If, Else, While, For, Func, Return,

        // Symbols
        Assign,     // =
        Plus,       // +  (Math)
        PlusPlus,   // ++ (Concat)
        Minus,      // -
        LT,         // <
        GT,         // >
        EqEq,       // ==

        SemiColon,  // ;
        Comma,      // ,
        LParen, RParen, // ( )
        LBrace, RBrace, // { }

        // Data
        Identifier, Number, String, EOF
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public Token(TokenType type, string value) { Type = type; Value = value; }
        public override string ToString() => $"{Type}: {Value}";
    }
}