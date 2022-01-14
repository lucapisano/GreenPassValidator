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
            var scanResult = "HC1:";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);
            Assert.IsFalse(res.IsInvalid);
        }

        [TestMethod]
        public async Task ValidTwoGValidationTest()
        {
            //insert a valid green pass data here, it can be obtained scanning the QR Code.
            var scanResult = "HC1:";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult, ValidationService.ValidationType.TwoG);
            Assert.IsFalse(res.IsInvalid);
        }
        
        [TestMethod]
        public async Task InvalidTwoGValidationTest()
        {
            // to further test 2G validation, please provide a DGC of a valid Covid-19 test 
            var scanResult = "HC1:";
            var resTest = await _sp.GetRequiredService<ValidationService>().Validate(scanResult, ValidationService.ValidationType.TwoG);

            // The validation result should be invalid, since is a 2G type which allows only recoveries and vaccinations
            Assert.IsTrue(resTest.IsInvalid);
        }

        [TestMethod]
        public async Task ValidBoosterValidationTest()
        {
            //insert a valid green pass here, one with a completed vaccination cycle + a booster dose. It can be obtained by scanning the QR Code
            var scanResult = "HC1:";
            
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult, ValidationService.ValidationType.Booster);
            
            Assert.IsFalse(res.IsInvalid);
            Assert.IsFalse(res.TestNeeded);
        }

        [TestMethod]
        public async Task InvalidBoosterValidationTest()
        {
            //insert a valid green pass here, but invalid for a booster validation (i.e. one for a Covid-19 test). It can be obtained by scanning the QR Code
            var scanResult = "HC1:";
            
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult, ValidationService.ValidationType.Booster);
            
            Assert.IsTrue(res.IsInvalid);
            Assert.IsFalse(res.TestNeeded);
        }

        [TestMethod]
        public async Task NeedTestBoosterValidationTest()
        {
            //insert a valid green pass here, one with a completed vaccination cycle but without a booster dose. It can be obtained by scanning the QR Code
            var scanResult = "HC1:";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult, ValidationService.ValidationType.Booster);
            
            //expected result : valid Dgc, but needs Covid-19 Test
            Assert.IsFalse(res.IsInvalid);
            Assert.IsTrue(res.TestNeeded);
        }

        [TestMethod]
        public async Task TestInvalid_ADOLF_HITLER()
        {
            // ADOLF HITLER black listed
            var scanResult = "HC1:6BFOXN%TSMAHN-H3YS1IK47ES6IXJR4E47X5*T917VF+UOGIS1RYZV:X9:IMJZTCV4*XUA2PSGH.+H$NI4L6HUC%UG/YL WO*Z7ON13:LHNG7H8H%BFP8FG4T 9OKGUXI$NIUZUK*RIMI4UUIMI.J9WVHWVH+ZEOV1AT1HRI2UHD4TR/S09T./08H0AT1EYHEQMIE9WT0K3M9UVZSVV*001HW%8UE9.955B9-NT0 2$$0X4PCY0+-CVYCRMTB*05*9O%0HJP7NVDEBO584DKH78$ZJ*DJWP42W5P0QMO6C8PL353X7H1RU0P48PCA7T5MCH5:ZJ::AKU2UM97H98$QP3R8BH9LV3*O-+DV8QJHHY4I4GWU-LU7T9.V+ T%UNUWUG+M.1KG%VWE94%ALU47$71MFZJU*HFW.6$X50*MSYOJT1MR96/1Z%FV3O-0RW/Q.GMCQS%NE";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);

            Assert.AreEqual(res.Dgc.Nam.Gn, "ADOLF");
            Assert.AreEqual(res.Dgc.Nam.Fn, "HITLER");

            Assert.IsTrue(res.IsInvalid);
        }

        [TestMethod]
        public async Task TestInvalid_MICKEY_MOUSE()
        {
            // MICKEY MOUSE black listed
            var scanResult = "HC1:6BFOXN%TSMAHN-H3YS1IK47ES6IXJR4E47X5*T917VF+UOGIS1RYZV:X9RLMSV9 NI4EFSYS:%OD3PYE9*FJ9QMQC8$.AIGCY0K5$0V-AVB85PSHDCR.9K%47IG$+9OPPYE97NVA.D9B92FF9B9LW4G%89-85QNC%05$0VD9%.OMRE/IE%TE6UGYGGCY0$2P0GB*$K8KG+9RR$F+ F%J00N89M40%KLR2A KZ*U0I1-I0*OC6H0/VMNPM/UESJ0A5L5M0G+SI*VSDKPZ0CN62XEAW1 WUQRELS4J1TZWV63HUTN /K9:KFKF+SF3*86AL3*IC%OYZQ5I9 LG/HLIJLKNF8JF172QDRB2C3OUW3IQ6RYMKHDV4*F -IMBCJIO%OA8EV/G3L-NG:2EQB*:C8FFIVT:1QI 8NIMW:BW$BY$M/+8%RFV8C3LVZ:2T+8IQ9LF8I66WWD";
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);

            Assert.AreEqual(res.Dgc.Nam.Gn, "MICKEY");
            Assert.AreEqual(res.Dgc.Nam.Fn, "MOUSE");

            Assert.IsTrue(res.IsInvalid);
        }

        [TestMethod]
        public async Task ValidateIssuingCertificate()
        {
            var s = "MIICLTCCAdOgAwIBAgIIXf7//TpWDVgwCgYIKoZIzj0EAwIwWTELMAkGA1UEBhMCQkUxGjAYBgNVBAoMEWVIZWFsdGggLSBCZWxnaXVtMS4wLAYDVQQDDCVCZWxnaXVtIENvdmlkMTkgQ291bnRyeSBTaWduaW5nIENBIDAxMB4XDTIxMDUyNzEwMTI0N1oXDTIzMDUyNzEwMTI0N1owSjELMAkGA1UEBhMCQkUxGjAYBgNVBAoMEWVIZWFsdGggLSBCZWxnaXVtMR8wHQYDVQQDDBZCZWxnaXVtIENvdmlkMTkgRFNDIDAxMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEU/f/KsmP3NasU/jZo7aulTrd9GHoznfnwWvX8xmHtK49EoobMAG7LhXnpLQ+aRwmmnSMcIWy8wPxM8QDMBUtyKOBkzCBkDAdBgNVHQ4EFgQUr/AjSs5HKJsXQVr617Z6OO2Z9h8wHwYDVR0jBBgwFoAUMc4oJrfby5Fk9eLZSMutpWhvX9UwPgYDVR0fBDcwNTAzoDGgL4YtaHR0cDovL2NlcnQtYXBwLmJlL3Jldm9rZWRMaXN0L2RnY19jc2NhMDEuY3JsMA4GA1UdDwEB/wQEAwIHgDAKBggqhkjOPQQDAgNIADBFAiA56koPekERN3iWtlXwuD8rwBgbsTkZj2Yqe8kL2doIbwIhAKYOJyyIeR4Po523PY5rniN4jaaSkgefulusXXKiEthU";
            var x509 = new X509Certificate2(Convert.FromBase64String(s));
        }
    }
}
