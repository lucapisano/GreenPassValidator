using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DGCValidator.Services.CWT.Certificates;
using DGCValidator.Services.DGC.ValueSet;
using GreenPass.Extensions;
using GreenPass.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace DGCValidator.Services
{
    /**
     * An implementation for finding certificates that may be used to verify a HCERT (see {@link HCertVerifier}).
     * 
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class CertificateManager : ICertificateProvider
    {
        private readonly IServiceProvider _sp;
        private readonly IOptions<ValidatorOptions> _opt;
        private readonly IRestService _restService;
        public Dictionary<string, ValueSet> ValueSets { get; private set; }
        public DSC_TL TrustList { get; private set; }
        private readonly string ValueSetPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public CertificateManager(IServiceProvider sp, IRestService service)
        {
            _sp = sp;
            _opt = _sp.GetRequiredService<IOptions<ValidatorOptions>>();
            _restService = service;
        }
        public async Task RefreshTrustListAsync()
        {
            DSC_TL trustList = await _restService.RefreshTrustListAsync();
            if (trustList != null && trustList.DscTrustList != null && trustList.DscTrustList.Count > 0 && trustList.Exp > GetSecondsFromEpoc())
            {
                TrustList = trustList;
                try
                {
                    File.WriteAllText(_opt.Value.CacheTrustListFileName, DSC_TLSerialize.ToJson(trustList));
                }
                catch(Exception e)
                {
                    //_logger?.LogWarning($"unable to cache trust list file");
                }
            }
        }

        public async Task LoadCertificates()
        {
            if (TrustList == null && File.Exists(_opt.Value.CacheTrustListFileName))
            {
                DSC_TL trustList = DSC_TL.FromJson(File.ReadAllText(_opt.Value.CacheTrustListFileName));
                // If trustlist hasn´t expired
                if (trustList.Exp > GetSecondsFromEpoc())
                {
                    TrustList = trustList;
                }
            }
            // If trustlist is not set or it´s older than 24 hours refresh it
            if (TrustList == null || (TrustList.Iat + _opt.Value.CacheInterval.TotalSeconds) < GetSecondsFromEpoc())
            {
                await RefreshTrustListAsync();
            }
        }

        private long GetSecondsFromEpoc()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public async Task<List<AsymmetricKeyParameter>> GetCertificates(string country, byte[] kid)
        {
            await LoadCertificates();
            List<AsymmetricKeyParameter> publicKeys = new List<AsymmetricKeyParameter>();

            // No TrustList means no keys to match with
            if( TrustList == null)
            {
                return publicKeys;
            }

            List<DscTrust> trusts=new List<DscTrust>();
            if( country != null && country.Length > 0 && TrustList.DscTrustList.ContainsKey(country) )
            {
                DscTrust dscTrust = TrustList.DscTrustList.GetValueOrDefault(country);
                if( dscTrust != null)
                {
                    trusts.Add(dscTrust);
                }
            }
            else
            {
                trusts.AddRange(TrustList.DscTrustList.Values);
            }

            foreach( DscTrust trust in trusts)
            {
                foreach (Key key in trust.Keys)
                {
                    string kidStr = Convert.ToBase64String(kid);
                        //.Replace('+', '-')
                        //.Replace('/', '_');
                    if (kid == null || key.Kid == null || key.Kid.Equals(kidStr))
                    {
                        if( key.Kty.Equals("EC"))
                        {
                            X9ECParameters x9 = ECNamedCurveTable.GetByName(key.Crv);
                            ECPoint point = x9.Curve.CreatePoint(Base64UrlDecodeToBigInt(key.X), Base64UrlDecodeToBigInt(key.Y));

                            ECDomainParameters dParams = new ECDomainParameters(x9);
                            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(point, dParams);
                            publicKeys.Add(pubKey);
                        }
                        else if( key.Kty.Equals("RSA"))
                        {
                            RsaKeyParameters pubKey = new RsaKeyParameters(false, Base64UrlDecodeToBigInt(key.N), Base64UrlDecodeToBigInt(key.E));
                            publicKeys.Add(pubKey);
                        }
                    }
                }
            }
            return publicKeys;
        }

        private BigInteger Base64UrlDecodeToBigInt(String value)
        {
            value = value.Replace('-', '+');
            value = value.Replace('_', '/');
            switch (value.Length % 4)
            {
                case 0: break;
                case 2: value += "=="; break;
                case 3: value += "="; break;
                default:
                    throw new Exception("Illegal base64url string!");
            }
            return new BigInteger(1,Convert.FromBase64String(value));
        }
    }
}
