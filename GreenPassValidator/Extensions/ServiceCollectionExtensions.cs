using DGCValidator.Services;
using GreenPassValidator.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenPassValidator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGreenPassValidator(this IServiceCollection sc, IConfiguration config)
        {
            sc.AddOptions()
                .Configure<ValidatorOptions>(config.GetSection(nameof(ValidatorOptions)))
                ;
            sc
                .AddTransient<VerificationService>()
                .AddTransient<CertificateManager>()
                .AddTransient<IRestService, RestService>()

                ;
            return sc;
        }
    }
}
