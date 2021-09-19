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
            var scanResult = "HC1:6BFOXN%TS3DHPVO13J /G-/2YRVA.QKW8SFBXG4CH23IRM*48KU/19AD6MO6 NI4EFSYS:%OD3P9B9LGFIE9MIHJ6W48UK.GCY0O1P9-8:0LVYCDEBD0HX2JN*4CY035T395*CBB686YBDQEQL02%K:XFE70*LP$V25$0Q:JB2P 5PG0EF.DI2IC-DS%4P+58:IC4ME/HL+9+0E23L-2J8:IPSPIE9WT0K3M9UVZSVV*001HW%8VD9/.OD4OYGFO-O/HLYLV0C5OA7REH0LO.C7BY4UE9GB55B9-NT0 2$$0$2PFVJ:PI/E2$4J6ALD-I5V0KZ0YWVO059:SZFB9NTIV4D-735QLF9MFF3J7FT5D75W9AV88E34+V4YC5/HQ9PQNFTPZBLRKC6HY%BE7LH:T4.2A6B+ SOZFU-V$O8G*GK0LKT0RTJBVHX*VT*1UOBYXV-%7UO7%RRK.P95DZY2R6W705%EPN1K6NDL*M:NB3Y675WP13W20KULD4";//insert a valid green pass data here, it can be obtained scanning the QR Code
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
