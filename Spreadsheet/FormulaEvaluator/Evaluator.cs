using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /**
     * A class that evaluates arithmetic expressions using standard infix notation.
     * Respects the usual precedence rules. Supports expressions with variables whose
     * values are looked up via a delegate.
     */
    public static class Evaluator
    {
        public delegate int Lookup(String s);

        /// Evaluates the given expression, provided a variable evaluator, and returns a single integer
        public static int Evaluate(String expr, Lookup variableEvaluator)
        {
            Stack<string> opStack = new Stack<string>(); // Operator stack
            Stack<int> valStack = new Stack<int>();      // Value stack

            // Ensures non-empty expression
            if (expr.Length == 0)
                throw new ArgumentException("Empty expresion");

            // Seperates values, operators, and variables into an array of Strings
            string[] substrings = Regex.Split(expr, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            for (int i = 0; i < substrings.Length; i++)
            {
                string token = substrings[i].Trim(); // Note: extra whitespace is removed

                // Ensure no blank tokens
                if (token.Length != 0)
                {
                    // If token is an integer:
                    if (char.IsNumber(token[0]))
                    {
                        int val = int.Parse(token);

                        // If * or / at top of opStack
                        if (opStack.IsOnTop("*", "/"))
                            // Compute new value and push
                            valStack.Push(Compute(valStack.Pop(), val, opStack.Pop()));
                        
                        else
                            valStack.Push(val);
                    }

                    // If token is + or -:
                    else if (token.Equals("+") || token.Equals("-"))
                    {
                        // If + or - at top of opStack
                        if (opStack.IsOnTop("+", "-"))
                        {
                            // Compute new value and push
                            int val = valStack.Pop();
                            valStack.Push(Compute(valStack.Pop(), val, opStack.Pop()));

                        }

                        opStack.Push(token);
                    }

                    // If token is * or /:
                    else if (token.Equals("*") || token.Equals("/"))
                        opStack.Push(token);

                    // If token is (:
                    else if (token.Equals("("))
                        opStack.Push(token);

                    // If token is ):
                    else if (token.Equals(")"))
                    {
                        // If top of op stack is + or -
                        if (opStack.IsOnTop("+", "-"))
                        {
                            // Compute new value and push
                            int val = valStack.Pop();
                            valStack.Push(Compute(valStack.Pop(), val, opStack.Pop()));

                        }

                        if (opStack.Count == 0 || !opStack.Pop().Equals("("))
                            throw new ArgumentException("opening parenthesis not found where expected");
                        
                        // If top of op stack is * or /
                        if (opStack.IsOnTop("*", "/"))
                        {
                            // Compute new value and push
                            int val = valStack.Pop();
                            valStack.Push(Compute(valStack.Pop(), val, opStack.Pop()));
                        }

                    }

                    // If token is a variable:
                    else if (IsVariable(token))
                    {
                        int val = variableEvaluator(token);

                        // If * or / at top of opStack
                        if (opStack.IsOnTop("*", "/"))
                            // Compute new value and push
                            valStack.Push(Compute(valStack.Pop(), val, opStack.Pop()));
                        
                        else
                            valStack.Push(val);

                    }

                    // Invalid token within operation
                    else
                        throw new ArgumentException("Invalid token within expression");
                }
            }

            // Last token has been processed

            // If operator stack is empty
            if (opStack.Count == 0)
            {
                if (valStack.Count > 1)
                {
                    throw new ArgumentException("Value with no operation found");
                }
                return valStack.Pop();
            }

            // If operator stack is not empty
            else
            {
                // Must have exactly one operator on operator stack
                if (opStack.Count != 1)
                {
                    throw new ArgumentException("Too many operators");
                }
                // Exactly two values on the value stack
                if (valStack.Count != 2)
                {
                    throw new ArgumentException("Missing values or Too many values");
                }

                string op = opStack.Pop();
                // Final operator must be + or -
                if (!(op.Equals("+") || op.Equals("-")))
                {
                    throw new ArgumentException("Final operator on stack must be + or -");
                }

                // Compute final answer
                int val = valStack.Pop();
                return Compute(valStack.Pop(), val, op);
            }
        }

        /// Takes two int and an operator (+, -, *, or /) and completes the operation
        private static int Compute(int val1, int val2, string optr)
        {
            int result = int.MaxValue;

            // Addition
            if (optr.Equals("+"))
            {
                result = val1 + val2;
            }

            // Subtraction
            else if (optr.Equals("-"))
            {
                result = val1 - val2;
            }

            // Multiplication
            else if (optr.Equals("*"))
            {
                result = val1 * val2;
            }

            // Division
            else if (optr.Equals("/"))
            {
                if (val2 == 0)
                    throw new ArgumentException("Divided by zero");
                result = val1 / val2;
            }

            return result;
        }

        /**
         * Checks if a given token is a variable
         * Returns bool
         */
        private static bool IsVariable(string token)
        {
            bool foundLet = false;
            bool foundNum = false;
            int numIndex = 0;

            /* DEF: a variable is any number of 
             * letters followed by any number of numbers
             * ex: a2, aaa222, etc. but not a22a, a, etc.
             */

            // Check for letters first
            for (int i = 0; i < token.Length; i++)
            {
                // Found a letter
                if (char.IsLetter(token[i]))
                {
                    foundLet = true;
                }

                // Found number, break out of loop
                if (char.IsNumber(token[i]))
                {
                    numIndex = i;
                    break;
                }
            }
            for (int i = numIndex; i < token.Length; i++)
            {
                // Found a number
                if (char.IsNumber(token[i]))
                {
                    foundNum = true;
                }

                // Letter is after a number, not a var
                if (char.IsLetter(token[i]))
                {
                    return false;
                }
            }
            // True if found a letter and number
            return foundLet && foundNum;
        }
    }

    /// Class extention methods
    static class PS1StackExtendion
    {
        /// Checks if the top of a stack is either one token or another
        public static bool IsOnTop(this Stack<String> s, string token, string token2)
        {
            if (s.Count == 0)
                return false;
            return s.Peek().Equals(token) || s.Peek().Equals(token2);
        }
    }
}
