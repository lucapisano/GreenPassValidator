using System;
using System.Runtime.Serialization;

/**
 * Exception for Expired certificates.
 * 
 * @author Henrik Bengtsson (henrik@sondaica.se)
 * @author Martin Lindström (martin@idsec.se)
 * @author Henric Norlander (extern.henric.norlander@digg.se)
 */
namespace DGCValidator.Services.CWT
{
    [Serializable]
    internal class CertificateValidationException : Exception
    {
        public CertificateValidationException()
        {
        }

        public CertificateValidationException(string message) : base(message)
        {
        }

        public CertificateValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CertificateValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
