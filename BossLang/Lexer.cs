using System.Collections.Generic;

namespace BLang
{
    public class Lexer
    {
        private string _input;
        private int _position;

        public Lexer(string input) { _input = input; _position = 0; }

        private char Peek(int offset = 0)
        {
            if (_position + offset >= _input.Length) return '\0';
            return _input[_position + offset];
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_position < _input.Length)
            {
                char current = Peek();

                if (char.IsWhiteSpace(current)) { _position++; }
                else if (char.IsLetter(current)) { tokens.Add(ReadIdentifier()); }
                else if (char.IsDigit(current)) { tokens.Add(ReadNumber()); }
                else if (current == '"') { tokens.Add(ReadString()); }

                // Symbols
                else if (current == '{') { tokens.Add(new Token(TokenType.LBrace, "{")); _position++; }
                else if (current == '}') { tokens.Add(new Token(TokenType.RBrace, "}")); _position++; }
                else if (current == '(') { tokens.Add(new Token(TokenType.LParen, "(")); _position++; }
                else if (current == ')') { tokens.Add(new Token(TokenType.RParen, ")")); _position++; }
                else if (current == ';') { tokens.Add(new Token(TokenType.SemiColon, ";")); _position++; }
                else if (current == ',') { tokens.Add(new Token(TokenType.Comma, ",")); _position++; }

                // Operators
                else if (current == '=')
                {
                    if (Peek(1) == '=') { tokens.Add(new Token(TokenType.EqEq, "==")); _position += 2; }
                    else { tokens.Add(new Token(TokenType.Assign, "=")); _position++; }
                }
                else if (current == '<') { tokens.Add(new Token(TokenType.LT, "<")); _position++; }
                else if (current == '>') { tokens.Add(new Token(TokenType.GT, ">")); _position++; }
                else if (current == '-') { tokens.Add(new Token(TokenType.Minus, "-")); _position++; }
                else if (current == '+')
                {
                    // Logic: Is it + or ++?
                    if (Peek(1) == '+') { tokens.Add(new Token(TokenType.PlusPlus, "++")); _position += 2; }
                    else { tokens.Add(new Token(TokenType.Plus, "+")); _position++; }
                }
                else { _position++; }
            }
            tokens.Add(new Token(TokenType.EOF, ""));
            return tokens;
        }

        private Token ReadString()
        {
            _position++;
            string result = "";
            while (_position < _input.Length && Peek() != '"') { result += Peek(); _position++; }
            _position++;
            return new Token(TokenType.String, result);
        }

        private Token ReadNumber()
        {
            string result = "";
            while (char.IsDigit(Peek())) { result += Peek(); _position++; }
            return new Token(TokenType.Number, result);
        }

        private Token ReadIdentifier()
        {
            string result = "";
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_') { result += Peek(); _position++; }

            switch (result)
            {
                case "int": return new Token(TokenType.IntKeyword, result);
                case "string": return new Token(TokenType.StringKeyword, result);
                case "print_out": return new Token(TokenType.Print, result);
                case "if": return new Token(TokenType.If, result);
                case "else": return new Token(TokenType.Else, result);
                case "while": return new Token(TokenType.While, result);
                case "for": return new Token(TokenType.For, result);
                case "func": return new Token(TokenType.Func, result);
                case "return": return new Token(TokenType.Return, result);
                default: return new Token(TokenType.Identifier, result);
            }
        }
    }
}