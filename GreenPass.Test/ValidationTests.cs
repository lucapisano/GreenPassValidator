using DGCValidator.Services;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenPass.Test
{
    [TestClass]
    public partial class ValidationTests
    {
        private readonly ServiceProvider _sp;
        private readonly CertificateManager _certManager;
        // WARNING: in order for the tests to work, you should provide your own partial class implementation.
        // Please see ValidationTests_Stub.cs for an example.
        

        public ValidationTests()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
            _certManager = _sp.GetRequiredService<CertificateManager>();
        }

        #region PfizerTests
       
        [TestMethod]
        public async Task ValidPfizerValidationTests()
        {
            // 3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_VALID_3G);
            Assert.IsFalse(res.IsInvalid);

            // 2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_VALID_2G, ValidationService.ValidationType.TwoG);
            Assert.IsFalse(res2G.IsInvalid);

            // Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_VALID_BOOSTER, ValidationService.ValidationType.Booster);
            
            Assert.IsFalse(resBooster.TestNeeded);
            Assert.IsFalse(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task InvalidPfizerValidationTests()
        {
            // 3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_INVALID);
            Assert.IsTrue(res.IsInvalid);

            // 2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_INVALID, ValidationService.ValidationType.TwoG);
            Assert.IsTrue(res2G.IsInvalid);

            // Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_INVALID, ValidationService.ValidationType.Booster);
            Assert.IsTrue(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task NeedsTestPfizerBoosterValidationTest()
        {
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Pfizer_NEEDSTEST, ValidationService.ValidationType.Booster);
            Assert.IsTrue(res.TestNeeded);
            Assert.IsFalse(res.IsInvalid);
        }

        #endregion

        #region ModernaTests

        [TestMethod]
        public async Task ValidModernaValidationTests()
        {
            //3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_VALID_3G);
            Assert.IsFalse(res.IsInvalid);

            //2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_VALID_2G, ValidationService.ValidationType.TwoG);
            Assert.IsFalse(res2G.IsInvalid);

            //Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_VALID_BOOSTER, ValidationService.ValidationType.Booster);
            Assert.IsFalse(resBooster.TestNeeded);
            Assert.IsFalse(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task InvalidModernaValidationTests()
        {
            // 3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_INVALID);
            Assert.IsTrue(res.IsInvalid);

            // 2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_INVALID, ValidationService.ValidationType.TwoG);
            Assert.IsTrue(res2G.IsInvalid);

            // Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_INVALID, ValidationService.ValidationType.Booster);
            Assert.IsTrue(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task NeedsTestModernaBoosterValidationTest()
        {
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Moderna_NEEDSTEST, ValidationService.ValidationType.Booster);
            Assert.IsTrue(res.TestNeeded);
            Assert.IsFalse(res.IsInvalid);
        }

        #endregion

        #region JohnsonTests
        
        [TestMethod]
        public async Task ValidJohnsonValidationTests()
        {
            //3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_VALID_3G);
            Assert.IsFalse(res.IsInvalid);

            //2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_VALID_2G, ValidationService.ValidationType.TwoG);
            Assert.IsFalse(res2G.IsInvalid);

            //Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_VALID_BOOSTER, ValidationService.ValidationType.Booster);
            Assert.IsFalse(resBooster.TestNeeded);
            Assert.IsFalse(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task InvalidJohnsonValidationTests()
        {
            // 3G Validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_INVALID);
            Assert.IsTrue(res.IsInvalid);

            // 2G Validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_INVALID, ValidationService.ValidationType.TwoG);
            Assert.IsTrue(res2G.IsInvalid);

            // Booster Validation
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_INVALID, ValidationService.ValidationType.Booster);
            Assert.IsTrue(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task NeedsTestJohnsonBoosterValidationTest()
        {
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Johnson_NEEDSTEST, ValidationService.ValidationType.Booster);
            Assert.IsTrue(res.TestNeeded);
            Assert.IsFalse(res.IsInvalid);
        }

        #endregion


        #region CovidSwabTests 
        // note: CovidSwab = covid test. I used swab for readability

        [TestMethod]
        public async Task ValidCovidSwabTest()
        {
            // Since tests are valid only for 3G validation, there are no other tests to be made
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_CovidTest_VALID_3G);
            Assert.IsFalse(res.IsInvalid);
        }

        [TestMethod]
        public async Task InvalidCovidSwabTest()
        {
            //3G validation
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_CovidTest_INVALID_3G);
            Assert.IsTrue(res.IsInvalid);

            // 2G validation: invalid even if we use a valid covid-19 test
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_CovidTest_VALID_3G, ValidationService.ValidationType.TwoG);
            Assert.IsTrue(res2G.IsInvalid);

            // Booster validation: invalid even if we use a valid covid-19 test
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_CovidTest_VALID_3G, ValidationService.ValidationType.Booster);
            Assert.IsTrue(resBooster.IsInvalid);
        }

        #endregion

        #region RecoveryTests
        
        [TestMethod]
        public async Task ValidRecoveryTests()
        {
            // 3G
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_VALID_3G);
            Assert.IsFalse(res.IsInvalid);

            // 2G
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_VALID_2G, ValidationService.ValidationType.TwoG);
            Assert.IsFalse(res2G.IsInvalid);
        }

        [TestMethod]
        public async Task InvalidRecoveryTests()
        {
            // 3G
            var res = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_INVALID);
            Assert.IsTrue(res.IsInvalid);

            // 2G
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_INVALID, ValidationService.ValidationType.TwoG);
            Assert.IsTrue(res2G.IsInvalid);

            // Booster
            var resBooster = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_INVALID, ValidationService.ValidationType.Booster);
            Assert.IsTrue(resBooster.IsInvalid);
        }

        [TestMethod]
        public async Task NeedsTestRecoveryTest()
        {
            // Use a valid 3G test Recovery DGC, it will yield a NEEDS_TEST result using a Booster validation
            var res2G = await _sp.GetRequiredService<ValidationService>().Validate(DGC_Recovery_VALID_3G, ValidationService.ValidationType.Booster);
            Assert.IsTrue(res2G.TestNeeded);
            Assert.IsFalse(res2G.IsInvalid);
        }
        #endregion
    }
}
