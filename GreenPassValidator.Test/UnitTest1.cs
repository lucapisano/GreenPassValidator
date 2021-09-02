using DGCValidator.Services;
using GreenPassValidator.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void TestMethod1()
        {
            var scanResult = "HC1:6BFOXN%TS3DHPVO13J /G-/2YRVA.QKW8SFBXG4CH23IRM*48KU/19AD6MO6 NI4EFSYS:%OD3P9B9LGFIE9MIHJ6W48UK.GCY0O1P9-8:0LVYCDEBD0HX2JN*4CY035T395*CBB686YBDQEQL02%K:XFE70*LP$V25$0Q:JB2P 5PG0EF.DI2IC-DS%4P+58:IC4ME/HL+9+0E23L-2J8:IPSPIE9WT0K3M9UVZSVV*001HW%8VD9/.OD4OYGFO-O/HLYLV0C5OA7REH0LO.C7BY4UE9GB55B9-NT0 2$$0$2PFVJ:PI/E2$4J6ALD-I5V0KZ0YWVO059:SZFB9NTIV4D-735QLF9MFF3J7FT5D75W9AV88E34+V4YC5/HQ9PQNFTPZBLRKC6HY%BE7LH:T4.2A6B+ SOZFU-V$O8G*GK0LKT0RTJBVHX*VT*1UOBYXV-%7UO7%RRK.P95DZY2R6W705%EPN1K6NDL*M:NB3Y675WP13W20KULD4";
            var res = _sp.GetRequiredService<VerificationService>().VerifyData(scanResult, _certManager);
            Assert.IsTrue(res.ExpirationDate > DateTime.Now);
        }
    }
}
