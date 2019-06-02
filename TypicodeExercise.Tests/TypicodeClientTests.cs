using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypicodeExercise.Clients;

namespace TypicodeExercise.Tests
{
    [TestClass]
    public class TypicodeClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ApiBaseUrlNotConfigured_ThrowsException()
        {
            new TypicodeClient();
        }

        [TestMethod]
        public void ApiBaseUrlConfigured_NoException()
        {
            ConfigurationManager.AppSettings["TypicodeApi.BaseUrl"] = "http://api.com";
            new TypicodeClient();
        }


    }
}
