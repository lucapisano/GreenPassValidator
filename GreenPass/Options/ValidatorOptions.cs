using System;
using System.Collections.Generic;
using System.Text;

namespace GreenPass.Options
{
    public class ValidatorOptions
    {
        //take it from configuration, with default provided here. https://github.com/lovasoa/sanipasse/blob/master/src/assets/Digital_Green_Certificate_Signing_Keys.json
        public string CertificateListProviderUrl { get; set; } = "https://dgcg.covidbevis.se/tp/trust-list";
        public string CacheTrustListFileName { get; set; } = "cached_trust_list.json";
        public TimeSpan CacheInterval { get; set; } = TimeSpan.FromDays(1);
    }
}
