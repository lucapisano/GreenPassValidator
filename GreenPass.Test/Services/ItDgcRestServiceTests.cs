using Microsoft.VisualStudio.TestTools.UnitTesting;
using DGCValidator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GreenPass.Options;
using GreenPass;
using GreenPass.Services;

namespace DGCValidator.Services.Tests
{
    [TestClass()]
    public class ItDgcRestServiceTests
    {
        ServiceProvider sp;
        CertificateManager cm;
        public ItDgcRestServiceTests()
        {
            // load config
            var config = new ConfigurationBuilder().AddJsonFile("testconfig.json", true).Build();
            if (config.GetSection(nameof(ValidatorOptions)).Exists())
            {
                config.GetSection(nameof(ValidatorOptions)).GetChildren();
            }

            var sc = new ServiceCollection();
            sc.AddOptions()
               .Configure<ValidatorOptions>(config.GetSection(nameof(ValidatorOptions)))
               ;
            sc
                .AddTransient<ValidationService>()
                .AddTransient<CertificateManager>()
                .AddTransient<IRestService, ItDgcRestService>()
                .AddSingleton<CachingService>()
                ;
            sp = sc.BuildServiceProvider();
            cm = sp.GetRequiredService<CertificateManager>();
        }
        [TestMethod()]
        public async Task RefreshTrustListAsyncTestAsync()
        {
            // this is an outdated test certificate 
            // it raises a "No signer certificates could be found" exception
            var it = "HC1:6BFOXN%TS3DH0YOJ58S S-W5HDC *M0II5XHC9B5G2+$N IOP-IA%NFQGRJPC%OQHIZC4.OI1RM8ZA.A5:S9MKN4NN3F85QNCY0O%0VZ001HOC9JU0D0HT0HB2PL/IB*09B9LW4T*8+DCMH0LDK2%K:XFE70*LP$V25$0Q:J:4MO1P0%0L0HD+9E/HY+4J6TH48S%4K.GJ2PT3QY:GQ3TE2I+-CPHN6D7LLK*2HG%89UV-0LZ 2ZJJ524-LH/CJTK96L6SR9MU9DHGZ%P WUQRENS431T1XCNCF+47AY0-IFO0500TGPN8F5G.41Q2E4T8ALW.INSV$ 07UV5SR+BNQHNML7 /KD3TU 4V*CAT3ZGLQMI/XI%ZJNSBBXK2:UG%UJMI:TU+MMPZ5$/PMX19UE:-PSR3/$NU44CBE6DQ3D7B0FBOFX0DV2DGMB$YPF62I$60/F$Z2I6IFX21XNI-LM%3/DF/U6Z9FEOJVRLVW6K$UG+BKK57:1+D10%4K83F+1VWD1NE";

            try
            {
                var dgc = await sp.GetRequiredService<ValidationService>().Validate(it);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No signer certificates could be found", ex.Message);
            }
        }
    }
}