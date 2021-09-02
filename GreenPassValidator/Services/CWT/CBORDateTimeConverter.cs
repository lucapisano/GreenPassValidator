using System;
using PeterO.Cbor;
/**
 * Converter for representation of NumericDate using DateTime.
 * 
 * @author Henrik Bengtsson (henrik@sondaica.se)
 * @author Martin Lindström (martin@idsec.se)
 * @author Henric Norlander (extern.henric.norlander@digg.se)
 */
namespace DGCValidator.Services.CWT
{
    public class CBORDateTimeConverter
    {

        public CBORDateTimeConverter()
        {
        }

        public DateTime? FromCBORObject(CBORObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            CBORObject untaggedObject = obj;

            if (obj.HasMostOuterTag(0) || obj.HasMostOuterTag(1))
            {
                untaggedObject = obj.UntagOne();
            }

            if (!untaggedObject.IsNumber)
            {
                throw new CBORException("Expected number for representation of date");
            }
            CBORNumber num = untaggedObject.AsNumber();
            if (!num.IsFinite())
            {
                throw new CBORException("Not a finite number");
            }
            if (num.CompareTo(Int64.MinValue) < 0 || num.CompareTo(Int64.MaxValue) > 0)
            {
                throw new CBORException("Date can not be represented as Instant (too small or large)");
            }
            // Section 2.4.1 of RFC7049 states:
            // Tag value 1 is for numerical representation of seconds relative to
            // 1970-01-01T00:00Z in UTC time. (For the non-negative values that the
            // Portable Operating System Interface (POSIX) defines, the number of
            // seconds is counted in the same way as for POSIX "seconds since the
            // epoch" [TIME_T].) The tagged item can be a positive or negative
            // integer (major types 0 and 1), or a floating-point number (major type
            // 7 with additional information 25, 26, or 27). Note that the number
            // can be negative (time before 1970-01-01T00:00Z) and, if a floating-
            // point number, indicate fractional seconds.
            //

            // We only support a positive integer ...
            //
            //if ( !num.IsInteger() )
            //{
            //    throw new CBORException(
            //      String.Format("Date is represented as {0} - Only {1} is supported", num.GetType(), CBORNumber.NumberKind.Integer));
            //}
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(num.ToInt64Unchecked()).ToUniversalTime();

            return dtDateTime;
        }
    }
}
