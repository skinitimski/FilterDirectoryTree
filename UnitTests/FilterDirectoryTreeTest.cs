using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

using Utility = Atmosphere.FilterDirectoryTree.FilterDirectoryTree;

namespace Atmosphere.UnitTests
{
    [TestFixture]
    public class FilterDirectoryTreeTest
    {
        private struct FilterTestCase
        {
            public string Input { get; set; }
            public string Expected { get; set; }
        }
        
        [Test]
        public void TestCopyAndFilter()
        {
            Utility.Reset();
            Utility.FilterValues.Add("SOLUTION_NAME", "TimskiLoader");
            
            //DirectoryInfo source = new DirectoryInfo("/vol/mono/templates/app-with-lib");
            //DirectoryInfo target = new DirectoryInfo("/vol/mono/test");
            
            //Utility.CopyAndFilterRecursively(source, target);
        }
        
        [Test]
        public void TestFilter()
        {
            Utility.Reset();
            Utility.FilterValues.Add("KEY", "Value");
            Utility.FilterValues.Add("SOLUTION_NAME", "FilterTemplate");
            
            List<FilterTestCase> testCases = new List<FilterTestCase>
            {
                new FilterTestCase
                {
                    Input = "",
                    Expected = ""
                },
                
                new FilterTestCase
                {
                    Input = "This is a string",
                    Expected = "This is a string"
                },
                
                new FilterTestCase
                {
                    Input = "%SOLUTION_NAME% and %SOLUTION_NAME%",
                    Expected = "FilterTemplate and FilterTemplate"
                },
                
                new FilterTestCase
                {
                    Input = "SOLUTION_NAME",
                    Expected = "SOLUTION_NAME"
                },
                
                new FilterTestCase
                {
                    Input = "%SOLUTION_NAME% and %KEY%",
                    Expected = "FilterTemplate and Value"
                }
            };
            
            foreach (FilterTestCase testCase in testCases)
            {
                string actual = Utility.Filter(testCase.Input);
                
                //Console.WriteLine("Expected: {0}, Actual: {1}", testCase.Expected, actual);
                
                Assert.AreEqual(testCase.Expected, actual);
            }
            
            Assert.Throws<KeyNotFoundException>(() => Utility.Filter("%UNKNOWN_KEY%"));
        }
    }
}

