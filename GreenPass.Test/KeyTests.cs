using Microsoft.VisualStudio.TestTools.UnitTesting;
using DGCValidator.Services.CWT.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCValidator.Services.CWT.Certificates.Tests
{
    [TestClass()]
    public class KeyTests
    {
        [TestMethod()]
        public void LoadFromX509Test()
        {
            var rawData1 = "MIIEDzCCAfegAwIBAgIURldu5rsfrDeZtDBxrJ+SujMr2IswDQYJKoZIhvcNAQELBQAwSTELMAkGA1UEBhMCSVQxHzAdBgNVBAoMFk1pbmlzdGVybyBkZWxsYSBTYWx1dGUxGTAXBgNVBAMMEEl0YWx5IERHQyBDU0NBIDEwHhcNMjEwNTEyMDgxODE3WhcNMjMwNTEyMDgxMTU5WjBIMQswCQYDVQQGEwJJVDEfMB0GA1UECgwWTWluaXN0ZXJvIGRlbGxhIFNhbHV0ZTEYMBYGA1UEAwwPSXRhbHkgREdDIERTQyAxMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEnL9+WnIp9fvbcocZSGUFlSw9ffW/jbMONzcvm1X4c+pXOPEs7C4/83+PxS8Swea2hgm/tKt4PI0z8wgnIehoj6OBujCBtzAfBgNVHSMEGDAWgBS+VOVpXmeSQImXYEEAB/pLRVCw/zBlBgNVHR8EXjBcMFqgWKBWhlRsZGFwOi8vY2Fkcy5kZ2MuZ292Lml0L0NOPUl0YWx5JTIwREdDJTIwQ1NDQSUyMHhcMSxPPU1pbmlzdGVybyUyMGRlbGxhJTIwU2FsdXRlLEM9SVQwHQYDVR0OBBYEFC4bAbCvpArrgZ0E+RrqS8V7TNNIMA4GA1UdDwEB/wQEAwIHgDANBgkqhkiG9w0BAQsFAAOCAgEAjxTeF7yhKz/3PKZ9+WfgZPaIzZvnO/nmuUartgVd3xuTPNtd5tuYRNS/1B78HNNk7fXiq5hH2q8xHF9yxYxExov2qFrfUMD5HOZzYKHZcjcWFNHvH6jx7qDCtb5PrOgSK5QUQzycR7MgWIFinoWwsWIrA1AJOwfUoi7v1aoWNMK1eHZmR3Y9LQ84qeE2yDk3jqEGjlJVCbgBp7O8emzy2KhWv3JyRZgTmFz7p6eRXDzUYHtJaufveIhkNM/U8p3S7egQegliIFMmufvEyZemD2BMvb97H9PQpuzeMwB8zcFbuZmNl42AFMQ2PhQe27pU0wFsDEqLe0ETb5eR3T9L6zdSrWldw6UuXoYV0/5fvjA55qCjAaLJ0qi16Ca/jt6iKuws/KKh9yr+FqZMnZUH2D2j2i8LBA67Ie0JoZPSojr8cwSTxQBdJFI722uczCj/Rt69Y4sLdV3hNQ2A9hHrXesyQslr0ez3UHHzDRFMVlOXWCayj3LIgvtfTjKrT1J+/3Vu9fvs1+CCJELuC9gtVLxMsdRc/A6/bvW4mAsyY78ROX27Bi8CxPN5IZbtiyjpmdfr2bufDcwhwzdwsdQQDoSiIF1LZqCn7sHBmUhzoPcBJdXFET58EKow0BWcerZzpvsVHcMTE2uuAUr/JUh1SBpoJCiMIRSl+XPoEA2qqYU=";

            var actualJWK = Key.LoadFromX509(Encoding.ASCII.GetBytes(rawData1));

            Assert.AreEqual("P-256", actualJWK.Crv);
            Assert.AreEqual("NJpCsMLQco4=", actualJWK.Kid);
            Assert.AreEqual("EC", actualJWK.Kty);
            Assert.AreEqual("nL9-WnIp9fvbcocZSGUFlSw9ffW_jbMONzcvm1X4c-o",actualJWK.X);
            Assert.AreEqual(null, actualJWK.X5A);
            Assert.AreEqual("NJpCsMLQco5pJbcmDRgT7bJxxUQKoPU8f92i_qiXabs", actualJWK.X5TS256);
            Assert.AreEqual("VzjxLOwuP_N_j8UvEsHmtoYJv7SreDyNM_MIJyHoaI8", actualJWK.Y);
            Assert.AreEqual(null, actualJWK.N);
            Assert.AreEqual(null, actualJWK.E);

            var rawData2 = "MIICwDCCAmagAwIBAgIIPR9jkXY7CPEwCgYIKoZIzj0EAwIwPTELMAkGA1UEBhMCSFIxEzARBgNVBAoMCkFLRCBkLm8uby4xGTAXBgNVBAMMEENyb2F0aWEgREdDIENTQ0EwHhcNMjEwNTIwMTMxNzQ2WhcNMjMwNTIwMTMxNzQ1WjA/MQswCQYDVQQGEwJIUjETMBEGA1UECgwKQUtEIGQuby5vLjEbMBkGA1UEAwwSQ3JvYXRpYSBER0MgRFMgMDAxMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEt5hwD0cJUB5TeQIAaE7nLjeef0vV5mamR30kjErGOcReGe37dDrmFAeOqILajQTiBXzcnPaMxWUd9SK9ZRexzaOCAUwwggFIMB8GA1UdIwQYMBaAFDErHKPIgGXhH70EktAlPHyGj1LRMC8GA1UdEgQoMCaBEkNyb2F0aWEuREdDQGRnYy5ocqQQMA4xDDAKBgNVBAcMA0hSVjAvBgNVHREEKDAmgRJDcm9hdGlhLkRHQ0BkZ2MuaHKkEDAOMQwwCgYDVQQHDANIUlYwZwYDVR0fBGAwXjAtoCugKYYnaHR0cDovL2RnYzEuZGdjLmhyL2Nyb2F0aWEtZGdjLWNzY2EuY3JsMC2gK6AphidodHRwOi8vZGdjMi5kZ2MuaHIvY3JvYXRpYS1kZ2MtY3NjYS5jcmwwHQYDVR0OBBYEFB55yLnz+T3ShQFs345mxQEJZb7TMCsGA1UdEAQkMCKADzIwMjEwNTIwMTMxNzQ2WoEPMjAyMTExMTYxMzE3NDZaMA4GA1UdDwEB/wQEAwIHgDAKBggqhkjOPQQDAgNIADBFAiANYlqMzCo7P6/FbwxS88MCB43CIBgfpJDmQ+D120Ov0gIhALJNQbk8HdHnkd31GV88U1N4YghHSZslLY8eZX8wSYR/";

            var actualJWK2 = Key.LoadFromX509(Encoding.ASCII.GetBytes(rawData2));

            Assert.AreEqual("P-256", actualJWK2.Crv);
            Assert.AreEqual("25QCxBrBJvA=", actualJWK2.Kid);
            Assert.AreEqual("EC", actualJWK2.Kty);
            Assert.AreEqual("t5hwD0cJUB5TeQIAaE7nLjeef0vV5mamR30kjErGOcQ", actualJWK2.X);
            Assert.AreEqual(null, actualJWK2.X5A);
            Assert.AreEqual("25QCxBrBJvBBALjD4A0vE9it6S_EOnuAU4l0vQbPwmU", actualJWK2.X5TS256);
            Assert.AreEqual("Xhnt-3Q65hQHjqiC2o0E4gV83Jz2jMVlHfUivWUXsc0", actualJWK2.Y);
            Assert.AreEqual(null, actualJWK2.N);
            Assert.AreEqual(null, actualJWK2.E);

            var rawData3 = "MIIFnDCCA4QCAQEwDQYJKoZIhvcNAQELBQAwgZkxCzAJBgNVBAYTAk1LMRgwFgYDVQQIDA9Ob3J0aCBNYWNlZG9uaWExDzANBgNVBAcMBlNrb3BqZTEbMBkGA1UECgwSTWluaXN0cnkgb2YgSGVhbHRoMRkwFwYDVQQDDBB6ZHJhdnN0dm8uZ292Lm1rMScwJQYJKoZIhvcNAQkBFhhjb250YWN0QHpkcmF2c3R2by5nb3YubWswHhcNMjEwOTE1MTMyMDUzWhcNMjIwOTE1MTMyMDUzWjCBjTELMAkGA1UEBhMCTUsxGDAWBgNVBAgMD05vcnRoIE1hY2Vkb25pYTEPMA0GA1UEBwwGU2tvcGplMRswGQYDVQQKDBJNaW5pc3RyeSBvZiBIZWFsdGgxGzAZBgNVBAsMEk1pbmlzdHJ5IG9mIEhlYWx0aDEZMBcGA1UEAwwQemRyYXZzdHZvLmdvdi5tazCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAMuGeVPEG+XxhagXRS8lIRj+GZcHmCiyANAVF7LjQdYeBJDZhfbX3BrGgwwcJh+X3hlpcWoj5h4KZvHsCm7azbrDM/bdmLmc+/H/T3ZOgo4YewNTu2m7ZqJ6RBjW1J5kK9cZqdst90TPqfpBtmM8FoZVUrZuDoyTZp7i6ZWwny4vDGWqlMUb1fIq40y7m6lArIB9cjjizZwdNDlU6IO6Kgxhg+S3rI/+x+0+6agx/gzb49QZxVTA5tQ22BjndDV2R+4K/k8It2uToLa5ul2BuuUbgztiHrSSc5jBPxcjAYGJ7Zrve3gg6Csta3aQPBlDFjpPTInNjA/Xdgy5To7Wz1FetypHrgpWWC1De+7ZdOqlCUQ+r0jSwp88UkM4av6pq/AaubdFwssJdVYmghZJz7WQU8bM4vxm6840EFBMFpe6ySmUEEy4nwIn3O2rQ2R0YUswUydiHMRKaU5ZTBLLNlspOTs4YyC2rkxpOuNKuZ269Jose8gyluVLObczL4EHHeAqdxNVEhgYyqC7Psk/Qt7JFQOU47epOLXNQdbEfhH7Vml94BLZelzNkvSWZdWDWMjo1zVuT0cvhZqVrhFQUwTfHMyp6r4uu3QmZoCyJL9QAcNNF5Vj6B+pAJyhQlfc0oQuqPRH+1F9oR3ITi3Y2cSCQmOJ07tznwuK6nstxDxhAgMBAAEwDQYJKoZIhvcNAQELBQADggIBAJJQwAZS0bnDj2AELabrd3EztNAkbaQUsspJ26gPTDku1n9fB/yWKkoCtFMHiEeNyBhSlsVbdnWOGSX4+ifVqizuDHn59snHBN+xHCAortuHFViMspvDwccdbMo+OQ5SEJhGo3bVRbAlC0tp1h6TOZvsgQvX+ztnCD/2clmAuOaLEEwpMy1lLvUn8fKclP+ulOHki+rd9qETf8jedBttgac/7ekpLm6VMUNCp3f0ETnnP3E4/0lBtodypLusM+Qa6iwM6BujsXNhv+4JG/42LeBIpJvrGKr5eFHMyUT3Nn10ZMpt3Prc60k6O2ZIr8vWq5oeXmCmuJi9zHGZni9f9nPjNyKcS8iRYCdHYyG7HOdHyX6rKvZ0SbaaOW/0Vovf5so+Jsey8Ck7jFuqsIXEdaoJyosGVrRnlA5HWLoPLaijtnghVADGq17M6PqSRWQDWntrzsqb8AyRFm2ksO1JmGHD4x/qxi+UMVOBQ2MQuJQHy4SeMsUOMnZU7ssqpOf3Ar34IQ0usITJ3AC2c8RrES2aBLo/0KkghwIWGo3HZZecNIKZYs3LNA36PliiFo5ihm7FNQMFkQ9rjXndUOwhkOwxchn77aruzWjEqL3p9aZAJKbI1Dt103nvY0m4vPHt/KAlSK/XU3GiqlWZBOuVUb7VVCKclvl78DLgCahA0hnK";

            var actualJWK3 = Key.LoadFromX509(Encoding.ASCII.GetBytes(rawData3));

            Assert.AreEqual(null, actualJWK3.Crv);
            Assert.AreEqual("KjE8h58xh7A=", actualJWK3.Kid);
            Assert.AreEqual("RSA", actualJWK3.Kty);
            Assert.AreEqual(null, actualJWK3.X);
            Assert.AreEqual(null, actualJWK3.X5A);
            Assert.AreEqual("KjE8h58xh7BpvPp3qYqbihoo7lKx5rS7ob0OD9RdfJM", actualJWK3.X5TS256);
            Assert.AreEqual(null, actualJWK3.Y);
            Assert.AreEqual("y4Z5U8Qb5fGFqBdFLyUhGP4ZlweYKLIA0BUXsuNB1h4EkNmF9tfcGsaDDBwmH5feGWlxaiPmHgpm8ewKbtrNusMz9t2YuZz78f9Pdk6Cjhh7A1O7abtmonpEGNbUnmQr1xmp2y33RM-p-kG2YzwWhlVStm4OjJNmnuLplbCfLi8MZaqUxRvV8irjTLubqUCsgH1yOOLNnB00OVTog7oqDGGD5Lesj_7H7T7pqDH-DNvj1BnFVMDm1DbYGOd0NXZH7gr-Twi3a5Ogtrm6XYG65RuDO2IetJJzmME_FyMBgYntmu97eCDoKy1rdpA8GUMWOk9Mic2MD9d2DLlOjtbPUV63KkeuClZYLUN77tl06qUJRD6vSNLCnzxSQzhq_qmr8Bq5t0XCywl1ViaCFknPtZBTxszi_GbrzjQQUEwWl7rJKZQQTLifAifc7atDZHRhSzBTJ2IcxEppTllMEss2Wyk5OzhjILauTGk640q5nbr0mix7yDKW5Us5tzMvgQcd4Cp3E1USGBjKoLs-yT9C3skVA5Tjt6k4tc1B1sR-EftWaX3gEtl6XM2S9JZl1YNYyOjXNW5PRy-FmpWuEVBTBN8czKnqvi67dCZmgLIkv1ABw00XlWPoH6kAnKFCV9zShC6o9Ef7UX2hHchOLdjZxIJCY4nTu3OfC4rqey3EPGE", actualJWK3.N);
            Assert.AreEqual("AQAB", actualJWK3.E);

            var nullData = "";
            var actualJWKNull = Key.LoadFromX509(Encoding.ASCII.GetBytes(nullData));
            Assert.IsNull(actualJWKNull);
        }
    }
}