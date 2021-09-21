using DGCValidator.Services;
using GreenPass;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestClass]
    public class Tests
    {
        private readonly ServiceProvider _sp;
        private readonly CertificateManager _certManager;

        public Tests()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
            _certManager = _sp.GetRequiredService<CertificateManager>();
        }
        [TestMethod]
        public async Task Test()
        {
            //insert a valid green pass data here, it can be obtained scanning the QR Code
            var scanResult = "HC1:...";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);
            Assert.IsFalse(res.IsInvalid);
        }
        [TestMethod]
        public async Task ValidateIssuingCertificate()
        {
            var s = "MIICLTCCAdOgAwIBAgIIXf7//TpWDVgwCgYIKoZIzj0EAwIwWTELMAkGA1UEBhMCQkUxGjAYBgNVBAoMEWVIZWFsdGggLSBCZWxnaXVtMS4wLAYDVQQDDCVCZWxnaXVtIENvdmlkMTkgQ291bnRyeSBTaWduaW5nIENBIDAxMB4XDTIxMDUyNzEwMTI0N1oXDTIzMDUyNzEwMTI0N1owSjELMAkGA1UEBhMCQkUxGjAYBgNVBAoMEWVIZWFsdGggLSBCZWxnaXVtMR8wHQYDVQQDDBZCZWxnaXVtIENvdmlkMTkgRFNDIDAxMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEU/f/KsmP3NasU/jZo7aulTrd9GHoznfnwWvX8xmHtK49EoobMAG7LhXnpLQ+aRwmmnSMcIWy8wPxM8QDMBUtyKOBkzCBkDAdBgNVHQ4EFgQUr/AjSs5HKJsXQVr617Z6OO2Z9h8wHwYDVR0jBBgwFoAUMc4oJrfby5Fk9eLZSMutpWhvX9UwPgYDVR0fBDcwNTAzoDGgL4YtaHR0cDovL2NlcnQtYXBwLmJlL3Jldm9rZWRMaXN0L2RnY19jc2NhMDEuY3JsMA4GA1UdDwEB/wQEAwIHgDAKBggqhkjOPQQDAgNIADBFAiA56koPekERN3iWtlXwuD8rwBgbsTkZj2Yqe8kL2doIbwIhAKYOJyyIeR4Po523PY5rniN4jaaSkgefulusXXKiEthU";
            var x509 = new X509Certificate2(Convert.FromBase64String(s));
        }
    }
}
