using System;
using DGCValidator.Services.DGC;
using DGCValidator.Services.DGC.V1;

namespace DGCValidator.Services
{
    /**
    * A DGC representation class.
    *
    * @author Henrik Bengtsson (henrik@sondaica.se)
    * @author Martin Lindström (martin@idsec.se)
    * @author Henric Norlander (extern.henric.norlander@digg.se)
    */
    public class SignedDGC
    {
        public EU_DGC Dgc { get; set; }
        public string IssuingCountry { get; set; }
        public DateTime CertificateExpirationDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsInvalid { get; set; }
        public SignedDGC()
        {
        }
    }
}
