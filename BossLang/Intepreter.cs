using System;
using System.Collections.Generic;

namespace BLang
{
    // Used to handle 'return' statements
    public class ReturnException : Exception
    {
        public dynamic Value { get; }
        public ReturnException(dynamic val) { Value = val; }
    }

    public class Interpreter
    {
        // Stack of scopes. The last one is the "Current" scope.
        // Global variables are at index 0.
        private List<Dictionary<string, dynamic>> _scopes = new List<Dictionary<string, dynamic>>();

        // Store functions globally
        private Dictionary<string, FunctionDefNode> _functions = new Dictionary<string, FunctionDefNode>();

        public Interpreter()
        {
            // Create global scope
            _scopes.Add(new Dictionary<string, dynamic>());
        }

        private Dictionary<string, dynamic> CurrentScope => _scopes[_scopes.Count - 1];

        // Helper to find variable in current or global scope
        private dynamic GetVar(string name)
        {
            // Check local first
            if (CurrentScope.ContainsKey(name)) return CurrentScope[name];
            // Check global
            if (_scopes[0].ContainsKey(name)) return _scopes[0][name];
            throw new Exception($"Undefined variable: {name}");
        }

        private void SetVar(string name, dynamic value)
        {
            // Update local if exists
            if (CurrentScope.ContainsKey(name)) { CurrentScope[name] = value; return; }
            // Update global if exists
            if (_scopes[0].ContainsKey(name)) { _scopes[0][name] = value; return; }
            // Define new in local
            CurrentScope[name] = value;
        }

        public void Run(List<Node> statements)
        {
            try
            {
                foreach (var stmt in statements) Execute(stmt);
            }
            catch (ReturnException)
            {
                // Ignore return at top level
            }
        }

        private void Execute(Node node)
        {
            if (node is BlockNode b)
            {
                foreach (var stmt in b.Statements) Execute(stmt);
            }
            else if (node is AssignmentNode a)
            {
                dynamic val = Evaluate(a.ValueExpression);
                if (a.VariableType == "int" && !(val is int)) throw new Exception($"Type Mismatch for {a.VariableName}");
                if (a.VariableType == "string" && !(val is string)) throw new Exception($"Type Mismatch for {a.VariableName}");
                CurrentScope[a.VariableName] = val; // Always declare in current scope
            }
            else if (node is ReassignmentNode r)
            {
                dynamic val = Evaluate(r.ValueExpression);
                SetVar(r.VariableName, val);
            }
            else if (node is PrintNode p)
            {
                Console.WriteLine(Evaluate(p.ExpressionToPrint));
            }
            else if (node is IfNode i)
            {
                if (IsTruth(Evaluate(i.Condition))) Execute(i.ThenBranch);
                else if (i.ElseBranch != null) Execute(i.ElseBranch);
            }
            else if (node is WhileNode w)
            {
                while (IsTruth(Evaluate(w.Condition))) Execute(w.Body);
            }
            else if (node is ForNode f)
            {
                // Create new scope for loop var
                _scopes.Add(new Dictionary<string, dynamic>());
                Execute(f.Initialization);
                while (IsTruth(Evaluate(f.Condition)))
                {
                    Execute(f.Body);
                    Execute(f.Increment);
                }
                _scopes.RemoveAt(_scopes.Count - 1); // Pop scope
            }
            else if (node is FunctionDefNode fd)
            {
                _functions[fd.Name] = fd;
            }
            else if (node is ReturnNode ret)
            {
                throw new ReturnException(Evaluate(ret.Value));
            }
            else if (node is FunctionCallNode fc) // For void calls
            {
                CallFunction(fc);
            }
        }

        private dynamic Evaluate(Node node)
        {
            if (node is NumberNode n) return n.Value;
            if (node is StringNode s) return s.Value;
            if (node is VariableNode v) return GetVar(v.Name);

            if (node is FunctionCallNode fc) return CallFunction(fc);

            if (node is BinaryOpNode b)
            {
                dynamic left = Evaluate(b.Left);
                dynamic right = Evaluate(b.Right);

                switch (b.Op)
                {
                    case TokenType.Plus: // STRICT MATH
                        if (left is int && right is int) return left + right;
                        throw new Exception("Use ++ to add strings, + is only for Math.");

                    case TokenType.PlusPlus: // STRING CONCAT
                        return left.ToString() + right.ToString();

                    case TokenType.Minus: return left - right;
                    case TokenType.LT: return left < right;
                    case TokenType.GT: return left > right;
                    case TokenType.EqEq: return left == right;
                }
            }
            return 0;
        }

        private dynamic CallFunction(FunctionCallNode node)
        {
            if (!_functions.ContainsKey(node.Name)) throw new Exception($"Unknown function: {node.Name}");
            var func = _functions[node.Name];

            if (node.Arguments.Count != func.Parameters.Count)
                throw new Exception($"Function {node.Name} expects {func.Parameters.Count} args");

            // Create Scope
            var scope = new Dictionary<string, dynamic>();
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                scope[func.Parameters[i]] = Evaluate(node.Arguments[i]);
            }

            _scopes.Add(scope); // PUSH

            dynamic result = 0;
            try
            {
                Execute(func.Body);
            }
            catch (ReturnException ret)
            {
                result = ret.Value;
            }

            _scopes.RemoveAt(_scopes.Count - 1); // POP
            return result;
        }

        private bool IsTruth(dynamic val)
        {
            if (val is int i) return i != 0;
            if (val is bool b) return b;
            return true;
        }
    }
}