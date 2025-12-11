using System;
using System.Collections.Generic;

namespace BLang
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _position = 0;
        public Parser(List<Token> tokens) { _tokens = tokens; }
        private Token Current => _tokens[_position];
        private Token Consume(TokenType type)
        {
            if (Current.Type == type) { var t = Current; _position++; return t; }
            throw new Exception($"Expected {type} but found {Current.Type}");
        }
        private Token ConsumeAny() { var t = Current; _position++; return t; }

        public List<Node> Parse()
        {
            var statements = new List<Node>();
            while (Current.Type != TokenType.EOF) statements.Add(ParseStatement());
            return statements;
        }

        private Node ParseStatement()
        {
            if (Current.Type == TokenType.IntKeyword || Current.Type == TokenType.StringKeyword)
                return ParseAssignment();

            if (Current.Type == TokenType.Identifier)
            {
                // Could be: x = 10; OR myFunction();
                if (_tokens[_position + 1].Type == TokenType.LParen) return ParseFunctionCallStmt();
                return ParseReassignment();
            }

            if (Current.Type == TokenType.Print) return ParsePrint();
            if (Current.Type == TokenType.If) return ParseIf();
            if (Current.Type == TokenType.While) return ParseWhile();
            if (Current.Type == TokenType.For) return ParseFor();
            if (Current.Type == TokenType.Func) return ParseFuncDef();
            if (Current.Type == TokenType.Return) return ParseReturn();
            if (Current.Type == TokenType.LBrace) return ParseBlock();

            throw new Exception($"Unexpected token: {Current}");
        }

        private Node ParseBlock()
        {
            Consume(TokenType.LBrace);
            var block = new BlockNode();
            while (Current.Type != TokenType.RBrace && Current.Type != TokenType.EOF)
            {
                block.Statements.Add(ParseStatement());
            }
            Consume(TokenType.RBrace);
            return block;
        }

        private Node ParseAssignment()
        {
            string type = ConsumeAny().Value;
            string name = Consume(TokenType.Identifier).Value;
            Consume(TokenType.Assign);
            Node val = ParseExpression();
            Consume(TokenType.SemiColon);
            return new AssignmentNode { VariableType = type, VariableName = name, ValueExpression = val };
        }

        private Node ParseReassignment()
        {
            string name = Consume(TokenType.Identifier).Value;
            Consume(TokenType.Assign);
            Node val = ParseExpression();
            Consume(TokenType.SemiColon);
            return new ReassignmentNode { VariableName = name, ValueExpression = val };
        }

        private Node ParseIf()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            Node cond = ParseExpression();
            Consume(TokenType.RParen);
            Node thenBranch = ParseStatement(); // Usually a Block
            Node elseBranch = null;
            if (Current.Type == TokenType.Else)
            {
                Consume(TokenType.Else);
                elseBranch = ParseStatement();
            }
            return new IfNode { Condition = cond, ThenBranch = thenBranch, ElseBranch = elseBranch };
        }

        private Node ParseWhile()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            Node cond = ParseExpression();
            Consume(TokenType.RParen);
            Node body = ParseStatement();
            return new WhileNode { Condition = cond, Body = body };
        }

        private Node ParseFor()
        {
            Consume(TokenType.For);
            Consume(TokenType.LParen);
            Node init = ParseStatement(); // int i = 0;
            Node cond = ParseExpression();
            Consume(TokenType.SemiColon);

            // Increment is usually "i = i + 1", we parse it as a reassignment but without semicolon in the loop header usually
            // To keep it simple, let's assume syntax: for(int i=0; i<10; i=i+1)
            string varName = Consume(TokenType.Identifier).Value;
            Consume(TokenType.Assign);
            Node incExp = ParseExpression();
            Node incNode = new ReassignmentNode { VariableName = varName, ValueExpression = incExp };

            Consume(TokenType.RParen);
            Node body = ParseStatement();

            return new ForNode { Initialization = init, Condition = cond, Increment = incNode, Body = body };
        }

        private Node ParseFuncDef()
        {
            Consume(TokenType.Func);
            string name = Consume(TokenType.Identifier).Value;
            Consume(TokenType.LParen);
            var parsedParams = new List<string>();
            while (Current.Type != TokenType.RParen)
            {
                // int x
                ConsumeAny(); // Type (ignored in this simple version)
                parsedParams.Add(Consume(TokenType.Identifier).Value);
                if (Current.Type == TokenType.Comma) Consume(TokenType.Comma);
            }
            Consume(TokenType.RParen);
            Node body = ParseBlock();
            return new FunctionDefNode { Name = name, Parameters = parsedParams, Body = body };
        }

        private Node ParseReturn()
        {
            Consume(TokenType.Return);
            Node val = ParseExpression();
            Consume(TokenType.SemiColon);
            return new ReturnNode { Value = val };
        }

        private Node ParseFunctionCallStmt()
        {
            string name = Consume(TokenType.Identifier).Value;
            Consume(TokenType.LParen);
            var args = new List<Node>();
            while (Current.Type != TokenType.RParen)
            {
                args.Add(ParseExpression());
                if (Current.Type == TokenType.Comma) Consume(TokenType.Comma);
            }
            Consume(TokenType.RParen);
            Consume(TokenType.SemiColon);
            return new FunctionCallNode { Name = name, Arguments = args };
        }

        private Node ParseFunctionCallExpr()
        {
            string name = Consume(TokenType.Identifier).Value;
            Consume(TokenType.LParen);
            var args = new List<Node>();
            while (Current.Type != TokenType.RParen)
            {
                args.Add(ParseExpression());
                if (Current.Type == TokenType.Comma) Consume(TokenType.Comma);
            }
            Consume(TokenType.RParen);
            return new FunctionCallNode { Name = name, Arguments = args };
        }

        private Node ParsePrint()
        {
            Consume(TokenType.Print);
            Consume(TokenType.LParen);
            Node exp = ParseExpression();
            Consume(TokenType.RParen);
            Consume(TokenType.SemiColon);
            return new PrintNode { ExpressionToPrint = exp };
        }

        private Node ParseExpression()
        {
            // Simple order of operations
            Node left = ParseTerm();

            while (Current.Type == TokenType.Plus || Current.Type == TokenType.PlusPlus ||
                   Current.Type == TokenType.Minus || Current.Type == TokenType.LT ||
                   Current.Type == TokenType.GT || Current.Type == TokenType.EqEq)
            {
                var op = ConsumeAny().Type;
                Node right = ParseTerm();
                left = new BinaryOpNode { Left = left, Op = op, Right = right };
            }
            return left;
        }

        private Node ParseTerm()
        {
            if (Current.Type == TokenType.Number) return new NumberNode { Value = int.Parse(ConsumeAny().Value) };
            if (Current.Type == TokenType.String) return new StringNode { Value = ConsumeAny().Value };
            if (Current.Type == TokenType.Identifier)
            {
                // Could be variable OR function call
                if (_tokens[_position + 1].Type == TokenType.LParen) return ParseFunctionCallExpr();
                return new VariableNode { Name = ConsumeAny().Value };
            }
            throw new Exception($"Unexpected term: {Current}");
        }
    }
}