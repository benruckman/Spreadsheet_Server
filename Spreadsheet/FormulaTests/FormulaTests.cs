using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// Basic Normalizer functions to uppercase/lowercase.
        /// </summary>
        Func<string, string> ToLower = var => var.ToLower();
        Func<string, string> ToUpper = var => var.ToUpper();

        /// <summary>
        /// Basic Validator function for uppercase.
        /// No letters returns true.
        /// </summary>
        public bool IsUpper(string var)
        {
            foreach (Char c in var)
                if (char.IsLetter(c))
                    return char.IsUpper(c);
            return true;
        }

        /// <summary>
        /// Basic Validator function for lowercase.
        /// No letters returns true;
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        public bool IsLower(string var)
        {
            foreach (Char c in var)
                if (char.IsLetter(c))
                    return char.IsLower(c);
            return true;
        }

        public double Lookup(string var)
        {
            if (var == "x")
                return 2;
            if (var == "X")
                return 4;
            return 0;
        }

        /// <summary>
        /// Formula must have at least one token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OneTokenRuleTest()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// When reading tokens from left to right, at no point should the number of closing
        /// parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void RightParRuleTest()
        {
            Formula f = new Formula("(17 * 8)) + 19");
        }

        /// <summary>
        /// The total number of opening parentheses must equal the total number of closing parentheses.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BallancedParRuleTest()
        {
            Formula f = new Formula("((4 * 5) + 2");
        }

        /// <summary>
        /// The first token of an expression must be a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenRuleTest1()
        {
            Formula f = new Formula("+ 6 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenRuleTest2()
        {
            Formula f = new Formula("- 6 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenRuleTest3()
        {
            Formula f = new Formula("* 6 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenRuleTest4()
        {
            Formula f = new Formula("/ 6 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenRuleTest5()
        {
            Formula f = new Formula(") 6 + 3");
        }

        /// <summary>
        /// The last token of an expression must be a number, a variable, or a closing parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenRuleTest1()
        {
            Formula f = new Formula("17 - 4 + ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenRuleTest2()
        {
            Formula f = new Formula("17 - 4 - ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenRuleTest3()
        {
            Formula f = new Formula("17 - 4 * ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenRuleTest4()
        {
            Formula f = new Formula("17 - 4 / ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenRuleTest5()
        {
            Formula f = new Formula("17 - 4 ( ");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or an operator must be either a
        /// number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParOrOpFollowingRuleTest1()
        {
            Formula f = new Formula("8 + (* 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParOrOpFollowingRuleTest2()
        {
            Formula f = new Formula("8 + (/ 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParOrOpFollowingRuleTest3()
        {
            Formula f = new Formula("8 + () + 2");
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing parenthesis must be
        /// either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest1()
        {
            Formula f = new Formula("8 (7)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest2()
        {
            Formula f = new Formula("8 7");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest3()
        {
            Formula f = new Formula("A2 (7)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest4()
        {
            Formula f = new Formula("A2 7");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest5()
        {
            Formula f = new Formula("(7) 7");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest6()
        {
            Formula f = new Formula("(7) A2");
        }

        /// <summary>
        /// Formulas should evaluate properly.
        /// Normalized variables take precidence.
        /// </summary>
        [TestMethod]
        public void SimpleEvaluateTest()
        {
            //Lookup("x") is 2, Lookup("X") is 4
            Assert.AreEqual(11.0, (double) new Formula("x+7", ToUpper, s => true).Evaluate(Lookup), 1E-09);
            Assert.AreEqual(9.0, (double) new Formula("x+7").Evaluate(Lookup), 1E-09);
        }

        [TestMethod]
        public void SimpleEqualsTest()
        {
            Formula lowercase = new Formula("x1+y2");
            Formula lowerToUpper = new Formula("x1+y2", ToUpper, s => true);
            Formula uppercase = new Formula("X1+Y2");
            Formula oneDec = new Formula("2.0 + x7");
            Formula threeDec = new Formula("2.000 + x7");
            Formula twoDec = new Formula("x7 + 2.00");

            Assert.IsFalse(lowercase.Equals(uppercase));
            Assert.IsFalse(lowercase.Equals(uppercase));
            Assert.IsTrue(oneDec.Equals(threeDec));
            Assert.IsTrue(lowerToUpper.Equals(uppercase));
            Assert.IsFalse(twoDec.Equals(oneDec));
            Assert.IsFalse(threeDec.Equals(twoDec));

            Assert.IsFalse(lowercase == uppercase);
            Assert.IsFalse(lowercase == uppercase);
            Assert.IsTrue(oneDec == threeDec);
            Assert.IsTrue(lowerToUpper == uppercase);

            Assert.IsTrue(lowercase != uppercase);
            Assert.IsTrue(lowercase != uppercase);
            Assert.IsFalse(oneDec != threeDec);
            Assert.IsFalse(lowerToUpper != uppercase);
        }

        [TestMethod]
        public void NullEqualsTest()
        {
            Formula null1 = null;
            Formula null2 = null;
            Formula f = new Formula("A2 + 7");

            Assert.IsFalse(f.Equals(null1));

            Assert.IsFalse(f == null1);
            Assert.IsFalse(null1 == f);
            Assert.IsTrue(null1 == null2);

            Assert.IsTrue(f != null1);
            Assert.IsTrue(null1 != f);
            Assert.IsFalse(null1 != null2);
        }

        /// <summary>
        /// Formulas converted to strings should have no spaces.
        /// Variables in returned string should be normalized if given.
        /// </summary>
        [TestMethod]
        public void SimpleToStringTest()
        {
            Assert.AreEqual("X+Y", new Formula("x + y", ToUpper, s => true).ToString());
            Assert.AreEqual("x+Y", new Formula("x + Y").ToString());
        }

        /// <summary>
        /// Get Variables should return all variables in order.
        /// Duplicate variables are not accounted.
        /// </summary>
        [TestMethod]
        public void SimpleGetVarsTest()
        {
            Formula f = new Formula("A2 + a2 - B2 + B2");
            List<String> variables = f.GetVariables().ToList();
            Assert.AreEqual("A2", variables[0]);
            Assert.AreEqual("a2", variables[1]);
            Assert.AreEqual("B2", variables[2]);
            Assert.IsTrue(3 == variables.Count);
        }

        /// <summary>
        /// GetVariables should return normalized variables.
        /// Duplicates are not returned.
        /// </summary>
        [TestMethod]
        public void NormGetVarsTest()
        {
            Formula f = new Formula("A2 + a2 - B2 + B2", ToLower, s => true);
            List<string> variables = f.GetVariables().ToList();
            Assert.AreEqual("a2", variables[0]);
            Assert.AreEqual("b2", variables[1]);
            Assert.IsTrue(2 == variables.Count);
        }

        /// <summary>
        /// Not directly dividing by zero does not return a formula error.
        /// </summary>
        [TestMethod]
        public void EvaluateAlmostZeroTest()
        {
            Formula f = new Formula("5.5 / (3.3 - 2.2 - 1.1)");
            object result = f.Evaluate(x => 0);
            Assert.IsFalse(result.GetType() == new FormulaError().GetType());
        }

        /// <summary>
        /// Dividing by zero returns a formula error.
        /// </summary>
        [TestMethod]
        public void EvaluateDivideByZeroTest()
        {
            Formula f = new Formula("5.5 / 0");
            object result = f.Evaluate(x => 0);
            Assert.IsTrue(result.GetType() == new FormulaError().GetType());

            f = new Formula("5.5 / x");
            result = f.Evaluate(x => 0);
            Assert.IsTrue(result.GetType() == new FormulaError().GetType());
            Assert.AreEqual("Error: Divided by zero", ((FormulaError) result).Reason);
        }

        /// <summary>
        /// Equal formulas return the same hash code
        /// </summary>
        [TestMethod]
        public void HashCodeTest()
        {
            Formula f1 = new Formula("X + 2", ToLower, IsLower);
            Formula f2 = new Formula("x+2.0");
            Formula f3 = new Formula("17 - x");
            Formula f4 = new Formula("x+2.0000000000000001");
            Assert.AreEqual(f1, f2);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.IsFalse(f1.GetHashCode() == f3.GetHashCode());

            Assert.IsTrue(f2 == f4);
            Assert.AreEqual(f2.GetHashCode(), f4.GetHashCode());
        }

        /// <summary>
        /// Incorrect vars result in exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadVarTest1()
        {
            Formula f = new Formula("a8a");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadVarTest2()
        {
            Formula f = new Formula("a44", s => s, IsUpper);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadVarTest3()
        {
            Formula f = new Formula("a88_");
        }

        /// <summary>
        /// Testing several different evaluations
        /// </summary>
        [TestMethod]
        public void EvaluateTest()
        {
            Formula f1 = new Formula("X * 2");
            Formula f2 = new Formula("2 * x + (10 / 2)");
            Formula f3 = new Formula("14 / (0)");
            Formula f4 = new Formula("14 / (1 + 1)");

            Assert.AreEqual(8.0, (double)f1.Evaluate(Lookup), 1E-9);
            Assert.AreEqual(9.0, (double)f2.Evaluate(Lookup), 1E-9);
            Assert.AreEqual(f3.Evaluate(Lookup).GetType(), 0.0.GetType());
            Assert.AreEqual(7.0, (double)f4.Evaluate(Lookup), 1E-9);
        }
    }
}
