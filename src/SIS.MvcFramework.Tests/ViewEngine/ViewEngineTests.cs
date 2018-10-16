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
            var result = viewEngine.GetHtml(viewCode);
            Assert.Equal(expectedResult, result);
        }
    }
}
