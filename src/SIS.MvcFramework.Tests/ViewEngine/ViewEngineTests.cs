using System.Collections.Generic;
using System.IO;
using SIS.MvcFramework.ViewEngine;
using Xunit;

namespace SIS.MvcFramework.Tests.ViewEngine
{
    public class ViewEngineTests
    {
        [Theory]
        [InlineData("IfForAndForeach")]
        [InlineData("ViewWithNoCode")]
        [InlineData("WorkWithViewModel")]
        public void RunTestViews(string testViewName)
        {
            var viewCode = File.ReadAllText($"TestViews/{testViewName}.html");
            var expectedResult = File.ReadAllText($"TestViews/{testViewName}.Result.html");
            IViewEngine viewEngine = new MvcFramework.ViewEngine.ViewEngine();
            var model = new TestModel
            {
                String = "Username",
                List = new List<string> { "Item1", "item2", "test", "123", "" }
            };
            var result = viewEngine.GetHtml(testViewName, viewCode, model);
            Assert.Equal(expectedResult, result);
        }

        public class TestModel
        {
            public string String { get; set; }

            public IEnumerable<string> List { get; set; }
        }
    }
}
