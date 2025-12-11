using System.Collections.Generic;

namespace BLang
{
    public abstract class Node { }

    // int x = 10;
    public class AssignmentNode : Node
    {
        public string VariableType { get; set; }
        public string VariableName { get; set; }
        public Node ValueExpression { get; set; }
    }

    // x = 10; (Reassigning existing var)
    public class ReassignmentNode : Node
    {
        public string VariableName { get; set; }
        public Node ValueExpression { get; set; }
    }

    // { ... }
    public class BlockNode : Node
    {
        public List<Node> Statements { get; set; } = new List<Node>();
    }

    // if (cond) { ... } else { ... }
    public class IfNode : Node
    {
        public Node Condition { get; set; }
        public Node ThenBranch { get; set; }
        public Node ElseBranch { get; set; }
    }

    // while (cond) { ... }
    public class WhileNode : Node
    {
        public Node Condition { get; set; }
        public Node Body { get; set; }
    }

    // for (init; cond; step) { ... }
    public class ForNode : Node
    {
        public Node Initialization { get; set; }
        public Node Condition { get; set; }
        public Node Increment { get; set; }
        public Node Body { get; set; }
    }

    // func name(arg1, arg2) { ... }
    public class FunctionDefNode : Node
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
        public Node Body { get; set; }
    }

    // name(1, 2)
    public class FunctionCallNode : Node
    {
        public string Name { get; set; }
        public List<Node> Arguments { get; set; } = new List<Node>();
    }

    // return x;
    public class ReturnNode : Node
    {
        public Node Value { get; set; }
    }

    public class PrintNode : Node { public Node ExpressionToPrint { get; set; } }

    // Handles +, ++, -, <, >, ==
    public class BinaryOpNode : Node
    {
        public Node Left { get; set; }
        public TokenType Op { get; set; }
        public Node Right { get; set; }
    }

    public class NumberNode : Node { public int Value { get; set; } }
    public class StringNode : Node { public string Value { get; set; } }
    public class VariableNode : Node { public string Name { get; set; } }
}