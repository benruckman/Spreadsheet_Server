// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// Normalizer placeholder as described in the class comment.
        /// </summary>
        private Func<string, string> p_Normalize;
        public Func<string, string> Normalizer
        {
            get { return p_Normalize; }
            private set { p_Normalize = value; }
        }

        /// <summary>
        /// Validator placeholder as described in the class comment.
        /// </summary>
        private Func<string, bool> p_IsValid;
        public Func<string, bool> IsValid
        {
            get { return p_IsValid; }
            private set { p_IsValid = value; }
        }

        /// <summary>
        /// Placeholder for the formula. Used in formula creation.
        /// </summary>
        private readonly string formula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            Normalizer = normalize;
            IsValid = isValid;

            // Used in assesing a valid formula
            int numOpenPar = 0;
            int numClosedPar = 0;
            int tokenCount = 0;
            string prevToken = "";

            this.formula = formula;
            IEnumerable<string> tokens = GetTokens(formula);
            foreach (string t in tokens)
            {
                // First token
                if (tokenCount == 0)
                {
                    if (!Double.TryParse(t, out double num) && !IsVariable(t, Normalizer, IsValid) && t != "(")
                    {
                        throw new FormulaFormatException("Starting Token Rule: The first token of an" +
                            " expression must be a number, a variable, or an opening parenthesis.");
                    }
                    if (t == "(")
                        numOpenPar++;
                }
                // Any token following a '(' or operator must be either a num, var or '('
                else if (prevToken == "(" || IsOperator(prevToken))
                {
                    if (t == "(")
                        numOpenPar++;
                    else if (!Double.TryParse(t, out double num) && !IsVariable(t, normalize, isValid) && t != "(")
                        throw new FormulaFormatException("Parenthesis/Operator Following Rule: Any token" +
                            " that immediately follows an opening parenthesis or an operator must be either" +
                            " a number, a variable, or an opening parenthesis.");
                }
                // Any token following a num, var or ')' must be either an operator or ')'.
                else if (Double.TryParse(prevToken, out double num) || !IsVariable(t, normalize, isValid) || prevToken == ")")
                {
                    if (t == ")")
                        numClosedPar++;
                    else if (!IsOperator(t))
                        throw new FormulaFormatException("Extra Following Rule: Any token that immediately" +
                            " follows a number, a variable, or a closing parenthesis must be either an operator" +
                            " or a closing parenthesis.");
                }

                // number of ')' should never exceed the number of '('
                if (numClosedPar > numOpenPar)
                    throw new FormulaFormatException("Right Parentheses Rule: When reading tokens from left to right," +
                        " at no point should the number of closing parentheses seen so far be greater than the number" +
                        " of opening parentheses seen so far");

                prevToken = t;
                tokenCount++;
            }

            // All tokens have been read

            // Must have at least one token
            if (tokenCount == 0)
                throw new FormulaFormatException("One Token Rule: There must be at least one token.");

            // Last token must be a num, var, or ')'
            if (!Double.TryParse(prevToken, out double x) && !IsVariable(prevToken, normalize, isValid) && prevToken != ")")
                throw new FormulaFormatException("Ending Token Rule: The last token of an expression must be a number," +
                    " a variable, or a closing parenthesis.");

            // Num of '(' must equal num of ')'
            if (numOpenPar != numClosedPar)
                throw new FormulaFormatException("Balanced Parentheses Rule: The total number of opening parentheses must" +
                    " equal the total number of closing parentheses.");

        }

        /// <summary>
        /// returns true if t is +, -, *, or /
        /// </summary>
        private bool IsOperator(string t)
        {
            if (t == "+" || t == "-" || t == "*" || t == "/")
                return true;
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<string> opStack = new Stack<string>(); // Operator stack
            Stack<double> valStack = new Stack<double>();      // Value stack

            // Seperates values, operators, and variables into an array of Strings
            IEnumerable<string> tokens = GetTokens(this.formula);

            foreach (string token in tokens)
            {
                // If token is an integer:
                if (Double.TryParse(token, out double num))
                {
                    // If * or / at top of opStack
                    if (opStack.IsOnTop("*", "/"))
                    {
                        // Compute new value
                        object result = Compute(valStack.Pop(), num, opStack.Pop());

                        // Check for FormulaError
                        if (result.GetType() == new FormulaError().GetType())
                            return result;

                        // Push value
                        valStack.Push((double)result);
                    }

                    else
                        valStack.Push(num);
                }

                // If token is + or -:
                else if (token.Equals("+") || token.Equals("-"))
                {
                    // If + or - at top of opStack
                    if (opStack.IsOnTop("+", "-"))
                    {
                        // Compute new value and push
                        double val = valStack.Pop();
                        valStack.Push((double)Compute(valStack.Pop(), val, opStack.Pop()));

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
                        double val = valStack.Pop();
                        valStack.Push((double)Compute(valStack.Pop(), val, opStack.Pop()));

                    }

                    // Pop ( operator
                    opStack.Pop();
                    // If top of op stack is * or /
                    if (opStack.IsOnTop("*", "/"))
                    {
                        // Compute new value
                        double val = valStack.Pop();
                        object result = Compute(valStack.Pop(), val, opStack.Pop());

                        // Check for formula error
                        if (result.GetType() == new FormulaError().GetType())
                            return val;

                        // Push value
                        valStack.Push((double)result);
                    }

                }

                // If token is a variable:
                else if (IsVariable(token, Normalizer, IsValid))
                {
                    string t = Normalizer(token);

                    object val;

                    try
                    {
                        val = lookup(t);
                    }
                    catch(ArgumentException)
                    { 
                        return new FormulaError("Formula Error: Could not find value");
                    }

                    val = lookup(t);

                    // If * or / at top of opStack
                    if (opStack.IsOnTop("*", "/"))
                    {
                        // Compute new value
                        val = Compute(valStack.Pop(), (double)val, opStack.Pop());

                        // Check for formula error
                        if (val.GetType() == new FormulaError().GetType())
                        {
                            return val;
                        }

                        // Push value
                        valStack.Push((double)val);
                    }

                    else
                        valStack.Push((double)val);

                }
            }

            // Last token has been processed

            // If operator stack is empty
            if (opStack.Count == 0)
            {
                return valStack.Pop();
            }

            // If operator stack is not empty
            else
            {
                // Compute final answer
                double val = valStack.Pop();
                return Compute(valStack.Pop(), val, opStack.Pop());
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            // Tracks variables visited
            List<String> vars = new List<String>();

            IEnumerable<String> tokens = GetTokens(formula);
            foreach (String t in tokens)
            {
                if (IsVariable(t, Normalizer, IsValid))
                {
                    String tNorm = Normalizer(t);
                    if (!vars.Contains(tNorm))
                    {
                        vars.Add(tNorm);
                        yield return tNorm;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            IEnumerable<String> tokens = GetTokens(formula);
            List<String> normTokens = new List<String>();

            // Add tokens to a list, normalizing variables
            foreach (string t in tokens)
            {
                if (IsVariable(t, Normalizer, IsValid))
                    normTokens.Add(Normalizer(t));
                else
                    normTokens.Add(t);
            }

            // Convert list to string
            return String.Concat(normTokens);
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null || this.GetType() != obj.GetType())
                return false;
            Formula input = (Formula)obj;

            IEnumerable<String> f1 = GetTokens(this.ToString());
            List<String> f2 = GetTokens(obj.ToString()).ToList();
            bool areEqual = true;

            // Iterate through this formula's tokens and compare to the given objects tokens
            int i = 0;
            foreach (String t in f1)
            {
                // Current token is a number
                if (Double.TryParse(t, out double num1))
                {
                    if (Double.TryParse(f2[i], out double num2))
                    {
                        areEqual = num1.ToString() == num2.ToString();
                    }
                    else
                        return false;
                }
                // Current token is a variable
                else if (IsVariable(t, Normalizer, IsValid))
                {
                    if (IsVariable(f2[i], input.Normalizer, input.IsValid))
                    {
                        string testVar1 = Normalizer(t);
                        string testVar2 = Normalizer(f2[i]);
                        areEqual = Normalizer(t) == Normalizer(f2[i]);
                    }
                    else
                        return false;
                }
                // Current token is an operator, (, or )
                else
                {
                    areEqual = t == f2[i];
                }

                if (!areEqual)
                    return false;
                i++;
            }

            return areEqual;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            // Handles null operands
            if (f1 is null)
                return f2 is null;

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            // Handles null operands
            if (f1 is null)
                return !(f2 is null);
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // Same as ToString method, but changes numbers to
            // double form then back to strings for "equal" formulas

            IEnumerable<String> tokens = GetTokens(formula);
            List<String> normTokens = new List<String>();

            // Add tokens to a list, normalizing variables
            foreach (string t in tokens)
            {
                if (IsVariable(t, Normalizer, IsValid))
                    normTokens.Add(Normalizer(t));
                else if (Double.TryParse(t, out double num))
                    normTokens.Add(num.ToString());
                else
                    normTokens.Add(t);
            }

            // Convert list to string
            return String.Concat(normTokens).GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }


        /// Takes two ints and an operator (+, -, *, or /) and completes the operation
        private object Compute(double val1, double val2, string optr)
        {
            object result = new FormulaError("Error: Incorrect operator");

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
                    return new FormulaError("Error: Divided by zero");
                result = val1 / val2;
            }

            return result;
        }

        /// <summary>
        /// Determines if a given token is a variable.
        /// Variables consist of a letter or underscore followed by zero
        /// or more letters, underscores, or digets"
        /// For example:
        /// A_2, a_2, _a_2, _2, _, A, a, etc. are all variables.
        /// 2_A, _2_A, a2, etc. are not variables.
        /// 
        /// Variables are normalized and checked for validity with this
        /// Formula's Validator and Normalizer
        /// </summary>
        private static bool IsVariable(string token, Func<string, string> normalizer, Func<string, bool> isValid)
        {
            int numIndex = 0;

            // Normalize token
            token = normalizer(token);

            // Check is token is valid
            if (!isValid(token))
                return false;

            //First must be letter/underscore
            if (!char.IsLetter(token[0]) && !(token[0] == '_'))
                return false;

            // Check for first num location
            for (int i = 1; i < token.Length; i++)
            {
                // Found number, break out of loop
                if (char.IsNumber(token[i]))
                {
                    numIndex = i;
                    break;
                }
            }
            // Check for Letters/Underscores after num
            for (int i = numIndex + 1; i < token.Length; i++)
            {
                if (char.IsLetter(token[i]) || (token[i] == '_'))
                {
                    return false;
                }
            }
            // True if this point is reached
            return true;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
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

