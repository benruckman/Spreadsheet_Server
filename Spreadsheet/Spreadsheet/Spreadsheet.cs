// Written by Jaden Turner for CS 3500, September 2020
// Version 1.1
// Revision history:
//   Version 1.0                     PS4 Spreadsheet.
// Branched from PS4 Spreadsheet
//   Version 1.1
//           Updated methods to reflect changes in AbstractSpreadsheet.
//           Implemented methods to create xml file of the spreadsheet.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SpreadsheetUtilities;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Represents all of the cell's dependencies in this spreadsheet
        /// </summary>
        private DependencyGraph dependencies;

        /// <summary>
        /// Represents all cell which contain values.
        /// </summary>
        private Dictionary<string, Cell> nonEmptyCells;

        private bool p_Changed;
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get => p_Changed; protected set => p_Changed = value; }

        /// <summary>
        /// Constructs a spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            dependencies = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            if (version == null)
                throw new ArgumentNullException();

            dependencies = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
        }

        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            if (filename == null || version == null)
                throw new ArgumentNullException();

            if (filename.Length == 0)
                throw new SpreadsheetReadWriteException("Empty path name is not legal.");

            dependencies = new DependencyGraph();
            nonEmptyCells = new Dictionary<string, Cell>();
            Version = version;

            try
            {
                // Convert input file into spreadsheet
                string cellName = ""; // Tracks current cell's name
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    // Check if versions match
                                    if (reader["version"] != version)
                                        throw new SpreadsheetReadWriteException("Version of the saved spreadsheet does not match the version parameter provided. Expected: " + reader["version"]);
                                    break; // no more direct info to read on Spreadsheet

                                case "cell":
                                    break;

                                case "name":
                                    reader.Read();
                                    cellName = reader.Value;
                                    break;

                                case "contents":
                                    reader.Read();
                                    try
                                    {
                                        SetContentsOfCell(cellName, reader.Value);
                                    }
                                    catch (CircularException)
                                    {
                                        throw new SpreadsheetReadWriteException("a circular exception was detected within the saved spreadsheet while adding this cell: " + cellName);
                                    }
                                    catch (InvalidNameException)
                                    {
                                        throw new SpreadsheetReadWriteException("a name of cell in the saved spreadsheet is invalid: " + cellName);
                                    }
                                    break;
                            }
                        }
                    }
                }

                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("file invalid.");
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            if (!Validate(name))
                throw new InvalidNameException();

            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].Contents;

            return "";
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            name = Normalize(name);
            if (!Validate(name))
                throw new InvalidNameException();

            if (nonEmptyCells.ContainsKey(name))
                return nonEmptyCells[name].Value;

            return "";
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the double value (as opposed to another value) of the named cell.
        /// </summary>
        private double LookupCellValue(string name)
        {
            name = Normalize(name);
            if (!Validate(name))
                throw new InvalidNameException();

            object val = GetCellValue(name);
            if (double.TryParse(val.ToString(), out double result))
                return result;
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// If a cell name is valid, return true.
        /// 
        /// A string is a valid cell name if and only if:
        ///   (1) its first character is an underscore or a letter
        ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
        /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
        /// 
        /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
        /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
        /// different cell names.
        /// </summary>
        private bool Validate(string name)
        {
            if (ReferenceEquals(null, name))
                throw new InvalidNameException();
            return Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$") && IsValid(Normalize(name));
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (string s in nonEmptyCells.Keys)
            {
                yield return s;
            }
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = Normalize(name);

            // Validate cell name
            if (!Validate(name))
                throw new InvalidNameException();

            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();

            // The list to return as defined in the summary of this method
            IList<string> dependencies = new List<string>();

            // Contents is a double
            if (double.TryParse(content, out double result))
                dependencies = SetCellContents(name, result);

            // Contents is nonempty
            else if (content.Length != 0)
            {
                // Contents is a Formula
                if (content[0] == '=')
                {
                    try
                    {
                        dependencies = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
                    }
                    catch(FormulaFormatException e)
                    {
                        Cell before;
                        if (nonEmptyCells.TryGetValue(name, out Cell value))
                            before = value;
                        else
                            before = new Cell("");
                        dependencies = SetContentsOfCell(name, before.Contents.ToString());

                        throw e;
                    }
                    
                }
                   

                //Contents is a string
                else
                    dependencies = SetCellContents(name, content);
            }

            // Contents is an empty string
            else
                dependencies = SetCellContents(name, content);

            // recalculate cells
            foreach (string cell in GetCellsToRecalculate(name))
            {
                if (nonEmptyCells.ContainsKey(cell))
                    nonEmptyCells[cell].Recalculate(LookupCellValue);
            }

            // Mark as changed
            Changed = true;

            return dependencies;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            // Check if cell already has a value
            if (nonEmptyCells.ContainsKey(name))
            {
                // Removes (if any existed) dependees of name
                dependencies.ReplaceDependees(name, new List<string>());
                // replace cell value in nonEmptyCells
                nonEmptyCells[name] = new Cell(number);
            }
            else
            {
                // add to nonEmptyCells
                nonEmptyCells.Add(name, new Cell(number));
                // mark spreadsheet as changed
                Changed = true;
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // Check if cell already has a value
            if (nonEmptyCells.ContainsKey(name))
            {
                // Removes (if any existed) dependees of name
                dependencies.ReplaceDependees(name, new List<string>());
                // replace cell value in nonEmptyCells
                if (text.Length != 0)
                    nonEmptyCells[name] = new Cell(text);
                else
                    nonEmptyCells.Remove(name);
            }
            else if (text.Length != 0)
            {
                // add to nonEmptyCells
                nonEmptyCells.Add(name, new Cell(text));
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            string current;
            if (nonEmptyCells.TryGetValue(name, out Cell c))
                current = c.Contents.ToString();
            else
                current = "";

            try
            {
                // if we have something in the cell already, change it
                if (nonEmptyCells.ContainsKey(name))
                    nonEmptyCells[name] = new Cell(formula, LookupCellValue);

                // else create new spot for new cell
                else
                    nonEmptyCells.Add(name, new Cell(formula, LookupCellValue));

                // update our cells dependees with the variables in its formula
                this.dependencies.ReplaceDependees(name, formula.GetVariables());

                // the cells we now need to recalculate
                IEnumerable<string> recalculate = GetCellsToRecalculate(name);

                // get all dependents of the cell, add to list
                List<string> list = new List<string>();

                foreach (string val in recalculate)
                {
                    list.Add(val);
                }

                // return list of new cells dependents and of cell
                return list;
            }
            catch (CircularException e)
            {
                SetContentsOfCell(name, current);
                throw e;
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();
            try
            {
                using (XmlReader r = XmlReader.Create(filename))
                {
                    while (r.Read())
                        if (r.IsStartElement())
                            if(r.Name == "spreadsheet")
                                return r.GetAttribute(0);
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Invalid File");
            }
            throw new SpreadsheetReadWriteException("Cannot Find Attribute");
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();

            if (filename.Length == 0)
                throw new SpreadsheetReadWriteException("Empty path name is not legal.");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter w = XmlWriter.Create(filename))
            {
                w.WriteStartDocument();
                w.WriteStartElement("spreadsheet");
                w.WriteAttributeString("version", Version);
                foreach (string cell in nonEmptyCells.Keys)
                {
                    // Convert cell contents to string
                    Cell currentCell = nonEmptyCells[cell];
                    string contents;
                    if (currentCell.Contents is Formula)
                        contents = "=" + currentCell.Contents.ToString();
                    else
                        contents = currentCell.Contents.ToString();

                    w.WriteStartElement("cell");
                    w.WriteElementString("name", cell);
                    w.WriteElementString("contents", contents);
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            Changed = false;
        }
    }
}