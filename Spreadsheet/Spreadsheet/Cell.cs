using System;
using SpreadsheetUtilities;

namespace SS
{
    internal class Cell
    {
        /// <summary>
        /// The input string for this cell
        /// </summary>
        private object p_contents;
        public object Contents
        { get => p_contents; private set => p_contents = value; }

        /// <summary>
        /// Represents what the cell should display
        /// </summary>
        private object p_value;
        public object Value
        { get => p_value; private set => p_value = value; }

        /// <summary>
        /// Creates a new cell from a name(location) and a string
        /// </summary>
        public Cell(string input)
        {
            Contents = input;
            Value = input;
        }

        /// <summary>
        /// Creates a new cell from a name(location) and a double
        /// </summary>
        public Cell(double input)
        {
            Contents = input;
            Value = input;
        }

        /// <summary>
        /// Creates a new cell from a name(location) and a formula
        /// </summary>
        public Cell(Formula input, Func<string, double> lookup)
        {
            Contents = input;
            Value = input.Evaluate(lookup);
        }

        /// <summary>
        /// Updates this cells value (i.e. a variable changed value)
        /// </summary>
        public void Recalculate(Func<string, double> lookup)
        {
            if (Contents is Formula formula)
                Value = formula.Evaluate(lookup);
        }
    }
}