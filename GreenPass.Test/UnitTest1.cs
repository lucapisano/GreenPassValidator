using DGCValidator.Services;
using GreenPass;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ServiceProvider _sp;
        private readonly CertificateManager _certManager;

        public UnitTest1()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
            _certManager = _sp.GetRequiredService<CertificateManager>();
        }
        [TestMethod]
        public async Task TestMethod1()
        {
            var scanResult = "HC1:6BFOXN%...D4";//insert a valid green pass data here, it can be obtained scanning the QR Code
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);
            Assert.IsTrue(res.ExpirationDate > DateTime.Now);
        }
    }
}
