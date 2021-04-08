using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// An empty spreadsheet should return an empty string for any
        /// valid cell contents.
        /// </summary>
        [TestMethod]
        public void EmptySpreadsheetTest()
        {
            Spreadsheet empty = new Spreadsheet();
            Assert.AreEqual("", empty.GetCellContents("A2"));
            Assert.AreEqual("", empty.GetCellContents("b9"));
            Assert.AreEqual("", empty.GetCellContents("c6"));
        }

        /// <summary>
        /// Setting a cells value should return a list consisting of the name
        /// plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// </summary>
        [TestMethod]
        public void SetContentsTest()
        {
            Spreadsheet s = new Spreadsheet();
            IList<string> cellList = s.SetContentsOfCell("A2", "2.0");
            Assert.AreEqual("A2", cellList[0]);
            Assert.AreEqual(1, cellList.Count);

            cellList = s.SetContentsOfCell("B3", "=A2");
            Assert.AreEqual("B3", cellList[0]);
            Assert.AreEqual(1, cellList.Count);

            cellList = s.SetContentsOfCell("A2", "3.0");
            Assert.AreEqual("A2", cellList[0]);
            Assert.AreEqual("B3", cellList[1]);
            Assert.AreEqual(2, cellList.Count);

            cellList = s.SetContentsOfCell("B3", "hello world");
            Assert.AreEqual("B3", cellList[0]);
            Assert.AreEqual(1, cellList.Count);

            cellList = s.SetContentsOfCell("B3", "=A2");
            Assert.AreEqual("B3", cellList[0]);
            Assert.AreEqual(1, cellList.Count);
        }

        /// <summary>
        /// GetCellContents method can return a double, formula, or string.
        /// </summary>
        [TestMethod]
        public void GetContentsTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("C21", "2.1");
            Assert.AreEqual(2.1, s.GetCellContents("C21"));
            s.SetContentsOfCell("B22", "=C21 + 0.1");
            Assert.AreEqual(new Formula("C21 + 0.1"), s.GetCellContents("B22"));
            s.SetContentsOfCell("D23", "23!");
            Assert.AreEqual("23!", s.GetCellContents("D23"));
        }

        /// <summary>
        /// GetCellContencts method should throw an invalid name exception
        /// when given a null value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetNullContentsTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        /// <summary>
        /// GetCellContencts method should throw an invalid name exception
        /// when given an invalid value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvlaidGetContentsTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("0.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvlaidGetContentsTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvlaidGetContentsTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("a_2");
        }

        /// <summary>
        /// SetContentsOfCell method should throw an invalid name exception
        /// when given a null value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetNullContentsTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "2.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetNullContentsTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "= A2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetNullContentsTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello world");
        }

        /// <summary>
        /// SetContentsOfCell method should throw an invalid name exception
        /// when given an invalid value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvlaidSetContentsTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("25", "2.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidSetContentsTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2x", "=A2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidSetContentsTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("&", "hello world");
        }

        /// <summary>
        /// Get value can return a string, a double, or a FormulaError
        /// </summary>
        [TestMethod]
        public void GetValueTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("c001", "hello world");
            s.SetContentsOfCell("A2", "0");
            s.SetContentsOfCell("B4", "=7/A2");

            Assert.AreEqual(new FormulaError().GetType(), s.GetCellValue("B4").GetType());
            Assert.AreEqual("hello world", s.GetCellValue("c001"));
            Assert.AreEqual(0.0, s.GetCellValue("A2"));
            Assert.AreEqual("", s.GetCellValue("Z3"));
        }

        /// <summary>
        /// GetCellValue method should throw an invalid name exception when
        /// given an invalid value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidGetValTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue("00");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidGetValTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue("x_10");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidGetValTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue("$");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidVarTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A2", "=B");
        }

        /// <summary>
        /// Adding an empty string to a cell means it is still empty.
        /// </summary>
        [TestMethod]
        public void GetNamesTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "0.0");
            s.SetContentsOfCell("aa1", "hello");
            s.SetContentsOfCell("A1", "= a1 + 1");

            List<string> nonEmptyCells = s.GetNamesOfAllNonemptyCells().ToList();
            Assert.IsTrue(nonEmptyCells.Contains("a1"));
            Assert.IsTrue(nonEmptyCells.Contains("aa1"));
            Assert.IsTrue(nonEmptyCells.Contains("A1"));

            s.SetContentsOfCell("aa1", "");
            nonEmptyCells = s.GetNamesOfAllNonemptyCells().ToList();
            Assert.IsTrue(nonEmptyCells.Contains("a1"));
            Assert.IsFalse(nonEmptyCells.Contains("aa1"));
            Assert.IsTrue(nonEmptyCells.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "20.0");
            s.SetContentsOfCell("A3", "= A1 + 12");
            s.SetContentsOfCell("B17", "= A1 * A3");

            s.SetContentsOfCell("A1", "=A3 / 2");
        }

        /// <summary>
        /// Cells values should update if the cell whom they depend on
        /// changes value.
        /// </summary>
        [TestMethod]
        public void ComplexSetGetValueTest()
        {
            Spreadsheet s = new Spreadsheet();

            IList<string> cellList = s.SetContentsOfCell("A1", "20.0");
            Assert.AreEqual("A1", cellList[0]);
            Assert.AreEqual(1, cellList.Count);
            cellList = s.SetContentsOfCell("A3", "= A1 + 12");
            Assert.AreEqual("A3", cellList[0]);
            Assert.AreEqual(1, cellList.Count);
            cellList = s.SetContentsOfCell("B17", "= A1 * A3");
            Assert.AreEqual("B17", cellList[0]);
            Assert.AreEqual(1, cellList.Count);

            Assert.AreEqual(20.0, s.GetCellValue("A1"));
            Assert.AreEqual(32.0, (double)s.GetCellValue("A3"), 1e-9);
            Assert.AreEqual(640.0, (double)s.GetCellValue("B17"), 1e-9);

            cellList = s.SetContentsOfCell("A1", "=3.0");
            Assert.AreEqual("A1", cellList[0]);
            Assert.AreEqual("A3", cellList[1]);
            Assert.AreEqual("B17", cellList[2]);

            Assert.AreEqual(3.0, s.GetCellValue("A1"));
            Assert.AreEqual(15.0, (double)s.GetCellValue("A3"), 1e-9);
            Assert.AreEqual(45.0, (double)s.GetCellValue("B17"), 1e-9);

            cellList = s.SetContentsOfCell("A3", "=12");
            Assert.AreEqual("A3", cellList[0]);
            Assert.AreEqual("B17", cellList[1]);

            Assert.AreEqual(3.0, s.GetCellValue("A1"));
            Assert.AreEqual(12.0, (double)s.GetCellValue("A3"), 1e-9);
            Assert.AreEqual(36.0, (double)s.GetCellValue("B17"), 1e-9);
        }

        [TestMethod]
        public void SaveTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A2", "0");
            s.SetContentsOfCell("A1", "= A2");
            s.SetContentsOfCell("B3", "hello world");

            s.Save("spreadsheetXML.xml");
            Assert.AreEqual("default", s.GetSavedVersion("spreadsheetXML.xml"));

            s.SetContentsOfCell("B3", "");

            s.Save("spreadsheetXMLNew.xml");
            Assert.AreEqual("default", s.GetSavedVersion("spreadsheetXMLNew.xml"));


            s = new Spreadsheet(s => true, s => s, "1.1");
            s.Save("spreadsheetEmpty.xml");
            Assert.AreEqual("1.1", s.GetSavedVersion("spreadsheetEmpty.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSaveTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGetVersionTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion(null);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidFileGetVersionTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion("nonexistantFile");
        }

        [TestMethod]
        [Timeout(2000)]
        public void StressTest1()
        {
            Spreadsheet s = new Spreadsheet();
            Dictionary<string, string> correctCells = new Dictionary<string, string>();

            // Adding Values
            string preCell = null;
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                for (int number = 1; number <= 99; number++)
                {
                    string name = "" + letter + number;
                    if (preCell == null)
                    {
                        correctCells.Add("A1", "10");
                        s.SetContentsOfCell("A1", "10");
                    }
                    else
                    {
                        correctCells.Add(name, preCell + "+1");
                        s.SetContentsOfCell(name, "=" + preCell + "+1");
                    }
                    preCell = name;
                }
            }

            // Check that values are correct
            double val = 10;
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                for (int number = 1; number <= 99; number++)
                {
                    string name = "" + letter + number;
                    Assert.AreEqual(correctCells[name], s.GetCellContents(name).ToString());
                    Assert.AreEqual(val, s.GetCellValue(name));
                    val++;
                }
            }

            // Replace value which all other cells depend on
            preCell = null;
            for (char letter = 'A'; letter <= 'A'; letter++)
            {
                for (int number = 1; number <= 10; number++)
                {

                    string name = "" + letter + number;
                    correctCells[name] = "2";
                    s.SetContentsOfCell(name, "2");
                }
            }
            correctCells["A1"] = "2";
            s.SetContentsOfCell("A1", "2");

            // Check that values are correct
            val = 2;
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                for (int number = 1; number <= 99; number++)
                {
                    string name = "" + letter + number;
                    Assert.AreEqual(correctCells[name], s.GetCellContents(name).ToString());

                    if (letter == 'A' && number <= 10)
                        Assert.AreEqual(val, s.GetCellValue(name));
                    else
                        val++;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ThreeArgumentTest()
        {
            Spreadsheet s = new Spreadsheet(s => s.StartsWith('A'), s => s.ToUpper(), "1.1");
            s.SetContentsOfCell("a3", "5");
            s.SetContentsOfCell("A7", "=A3");
            Assert.AreEqual(5.0, s.GetCellContents("A3"));
            Assert.AreEqual(new Formula("A3"), s.GetCellContents("a7"));
            Assert.AreEqual(5.0, s.GetCellValue("a3"));
            Assert.AreEqual(5.0, s.GetCellValue("A7"));
            s.SetContentsOfCell("b2", "10");
        }

        [TestMethod]
        public void FourArgumentTest()
        {
            Spreadsheet s1 = new Spreadsheet(s => true, s => s, "1.1");
            s1.SetContentsOfCell("a1", "2");
            s1.SetContentsOfCell("a2", "=a1+18");
            s1.SetContentsOfCell("a3", "hello world");
            s1.Save("spreadsheetONE.xml");

            Spreadsheet s2 = new Spreadsheet("spreadsheetONE.xml", s => true, s => s, "1.1");
            Assert.AreEqual("1.1", s1.GetSavedVersion("spreadsheetONE.xml"));
            s2.SetContentsOfCell("a4", "90");
            s2.Save("spreadsheetSML.xml");
            Assert.AreEqual("1.1", s2.GetSavedVersion("spreadsheetSML.xml"));

            Assert.AreEqual(2.0, s2.GetCellContents("a1"));
            Assert.AreEqual(new Formula("a1+18"), s2.GetCellContents("a2"));
            Assert.AreEqual("hello world", s2.GetCellContents("a3"));
            Assert.AreEqual(90.0, s2.GetCellContents("a4"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullThreeArgsTest()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFourArgsTest1()
        {
            Spreadsheet s = new Spreadsheet(null, s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFourArgsTest2()
        {
            Spreadsheet s = new Spreadsheet("someFile", s => true, s => s, null);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void EmptyFilePathTestConstructor()
        {
            Spreadsheet s = new Spreadsheet("", s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SavedCircularTest()
        {
            WriteCircularSpreadsheetXML("circularSpreadsheet.xml");
            try
            {
                Spreadsheet s = new Spreadsheet("circularSpreadsheet.xml", s => true, s => s, "default");
            }
            catch (SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("a circular exception was detected within the saved spreadsheet while adding this cell: A3", e.Message);
                throw e;
            }
        }

        public void WriteCircularSpreadsheetXML(string filename)
        {
            Dictionary<string, string> Cells = new Dictionary<string, string>();
            Cells.Add("A1", "=A2");
            Cells.Add("A2", "=A3");
            Cells.Add("A3", "=A1");
            using (XmlWriter w = XmlWriter.Create(filename))
            {
                w.WriteStartDocument();
                w.WriteStartElement("spreadsheet");
                w.WriteAttributeString("version", "default");
                foreach (string cell in Cells.Keys)
                {
                    w.WriteStartElement("cell");
                    w.WriteElementString("name", cell);
                    w.WriteElementString("contents", Cells[cell]);
                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void MismatchedVersionTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save("defaultVersion.xml");
            try
            {
                s = new Spreadsheet("defaultVersion.xml", s => true, s => s, "notDefault");
            }
            catch (SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("Version of the saved spreadsheet does not match the version parameter provided. Expected: default", e.Message);
                throw e;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidFileGetVersionTest2()
        {
            using (XmlWriter w = XmlWriter.Create("InvalidFile"))
            {
                w.WriteStartElement("something");
                w.WriteEndElement();
            }
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion("InvalidFile");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SavedInvalidNameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A2", "3");
            s.Save("simpleSpreadsheet.xml");
            try
            {
                s = new Spreadsheet("simpleSpreadsheet.xml", s => true, s => "A", "default");
            }
            catch (SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("a name of cell in the saved spreadsheet is invalid: A2", e.Message);
                throw e;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidFilenameTest1()
        {
            try
            {
                Spreadsheet s = new Spreadsheet("unnamedFile", s => true, s => s, "default");
            }
            catch (SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("file not found.", e.Message);
                throw e;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidFilenameTest2()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.Save("");
            }
            catch (SpreadsheetReadWriteException e)
            {
                Assert.AreEqual("Empty path name is not legal.", e.Message);
                throw e;
            }
        }

        [TestMethod]
        public void ChangedTest()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("a2", "20");
            Assert.IsTrue(s.Changed);
            s.Save("changedTest.xml");
            Assert.IsFalse(s.Changed);
        }

        /// <summary>
        /// PS4 GradingTests Modified for PS5 Spreadsheet
        /// </summary>

        // EMPTY SPREADSHEETS
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetContents()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestGetEmptyContents()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetInvalidNameDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        public void TestSimpleSetDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullStringVal()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullStringName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestSetGetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullFormVal()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", null);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullFormName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "=2");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        public void TestSetGetForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(CircularException))]
        public void TestComplexCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        public void TestEmptyNames()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        public void TestExplicitEmptySet()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        public void TestSimpleNamesString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestSimpleNamesDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestSimpleNamesFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestMixedNames()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestSetSingletonDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestSetSingletonString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestSetSingletonFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SequenceEqual(new List<string>() { "C1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestSetChain()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestChangeFtoD()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestChangeFtoS()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestChangeStoF()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestStress1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            IList<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestStress1a()
        {
            TestStress1();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestStress1b()
        {
            TestStress1();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestStress1c()
        {
            TestStress1();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestStress2()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestStress2a()
        {
            TestStress2();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestStress2b()
        {
            TestStress2();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestStress2c()
        {
            TestStress2();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestStress3()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestStress3a()
        {
            TestStress3();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestStress3b()
        {
            TestStress3();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestStress3c()
        {
            TestStress3();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestStress4()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestStress4a()
        {
            TestStress4();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestStress4b()
        {
            TestStress4();
        }
        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestStress4c()
        {
            TestStress4();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestStress5()
        {
            RunRandomizedTest(47, 2519);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestStress6()
        {
            RunRandomizedTest(48, 2521);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestStress7()
        {
            RunRandomizedTest(49, 2526);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestStress8()
        {
            RunRandomizedTest(50, 2521);
        }

        /// <summary>
        /// Sets random contents for a random cell 10000 times
        /// </summary>
        /// <param name="seed">Random seed</param>
        /// <param name="size">The known resulting spreadsheet size, given the seed</param>
        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        /// <summary>
        /// Generates a random cell name with a capital letter and number between 1 - 99
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        /// <summary>
        /// Generates a random Formula
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }
    }
}

