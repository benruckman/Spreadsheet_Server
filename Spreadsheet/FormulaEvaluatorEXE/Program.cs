using System;
using FormulaEvaluator;

namespace FormulaEvaluatorEXE
{
    class Program
    {
        public static int Lookup(string s)
        {
            if (s.Equals("a2"))
                return 5;
            else if (s.Equals("c4"))
                return 0;

            throw new ArgumentException("unknown variable");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Expected: 7 Actual: " + Evaluator.Evaluate("6+(7-4)/3", Lookup));
            Console.WriteLine("Expected: 8 Actual: " + Evaluator.Evaluate("6*(8/4)-7+3", Lookup));
            Console.WriteLine("Expected: 26 Actual: " + Evaluator.Evaluate("5 * c4 + (5 + (4 *6) -3) / 1", Lookup));

            // Test unknown variable
            string message = "Expected: pass Actual: ";
            try
            {
                Console.WriteLine(Evaluator.Evaluate("5-3*b3", Lookup));
                Console.WriteLine(message + "fail");
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(message + "pass: " + e.Message);
            }

            try
            {
                Console.WriteLine(Evaluator.Evaluate("87 / / 6", Lookup));
                Console.WriteLine(message + "fail");
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(message + "pass: " + e.Message);
            }

            try
            {
                Console.WriteLine(Evaluator.Evaluate("5 / 7b + 3", Lookup));
                Console.WriteLine(message + "fail");
            }
            catch (FormatException e)
            {
                Console.WriteLine(message + "pass: " + e.Message);
            }

            try
            {
                Console.WriteLine(Evaluator.Evaluate("2000 / 0", Lookup));
                Console.WriteLine(message + "fail");
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine(message + "pass: " + e.Message);
            }



            Console.Read();
        }
    }
}
