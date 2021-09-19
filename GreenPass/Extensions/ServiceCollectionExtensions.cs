using DGCValidator.Services;
using GreenPass.Options;
using GreenPass.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenPass.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGreenPassValidator(this IServiceCollection sc, IConfiguration config)
        {
            sc.AddOptions()
                .Configure<ValidatorOptions>(config.GetSection(nameof(ValidatorOptions)))
                ;
            sc
                .AddTransient<ValidationService>()
                .AddTransient<CertificateManager>()
                .AddTransient<IRestService, RestService>()
                .AddSingleton<CachingService>()
                ;
            return sc;
        }
    }
}
