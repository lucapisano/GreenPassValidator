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
    internal class CertificateExpiredException : Exception
    {
        public CertificateExpiredException()
        {
        }

        public CertificateExpiredException(string message) : base(message)
        {
        }

        public CertificateExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CertificateExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}