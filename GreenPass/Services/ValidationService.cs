using System;
using System.IO;
using System.Text;
using DGCValidator.Services.CWT;
using DGCValidator.Services;
using DGCValidator.Services.DGC;
using DGCValidator.Services.DGC.V1;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PeterO.Cbor;

namespace GreenPass
{
    /**
     * A Crypto support class for the reading of the European Digital Green Certificate.
     *
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class ValidationService
    {
        private readonly CertificateManager _certManager;

        public ValidationService(CertificateManager certificateManager)
        {
            _certManager = certificateManager;
        }

        public SignedDGC Validate(String codeData)
        {
            try {
                // The base45 encoded data shoudl begin with HC1
                if( codeData.StartsWith("HC1:"))
                {
                    string base45CodedData = codeData.Substring(4);

                    // Base 45 decode data
                    byte[] base45DecodedData = Base45Decoding(Encoding.GetEncoding("UTF-8").GetBytes(base45CodedData));

                    // zlib decompression
                    byte[] uncompressedData = ZlibDecompression(base45DecodedData);

                    SignedDGC vacProof = new SignedDGC();
                    // Sign and encrypt data
                    byte[] signedData = VerifySignedData(uncompressedData, vacProof, _certManager);

                    // Get json from CBOR representation of ProofCode
                    EU_DGC eU_DGC = GetVaccinationProofFromCbor(signedData);
                    vacProof.Dgc = eU_DGC;
                    return vacProof;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
            return null;
        }

		protected static byte[] ZlibDecompression(byte[] compressedData)
        {
            if( compressedData[0] == 0x78 )
            {
                var outputStream = new MemoryStream();
                using (var compressedStream = new MemoryStream(compressedData))
                using (var inputStream = new InflaterInputStream(compressedStream))
                {
                    inputStream.CopyTo(outputStream);
                    outputStream.Position = 0;
                    return outputStream.ToArray();
                }
            }
            else
            {
                // The data is not compressed
                return compressedData;
            }
        }

		protected static byte[] VerifySignedData(byte[] signedData, SignedDGC vacProof, CertificateManager certificateManager)
        {
            DGCVerifier verifier = new DGCVerifier(certificateManager);
            return verifier.Verify(signedData, vacProof);
        }

        protected static byte[] Base45Decoding(byte[] encodedData)
        {
            byte[] uncodedData = Base45.Decode(encodedData);
			return uncodedData;
        }

        protected static EU_DGC GetVaccinationProofFromCbor(byte[] cborData)
		{
            CBORObject cbor = CBORObject.DecodeFromBytes(cborData, CBOREncodeOptions.Default);
            string json = cbor.ToJSONString();
            EU_DGC vacProof = EU_DGC.FromJson(cbor.ToJSONString());
            return vacProof;
        }


	}
}
