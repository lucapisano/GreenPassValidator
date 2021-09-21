using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GreenPass.Lambda
{
    public class Function
    {
        private ServiceProvider _sp;
        void Init()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
        }
        public async Task<bool> BooleanResult(string input, ILambdaContext context)
        {
            Init();
            var res = await _sp.GetRequiredService<ValidationService>().Validate(input);
            return !res.IsInvalid;
        }
        public async Task<string> JsonResult(string input, ILambdaContext context)
        {
            Init();
            var res = await _sp.GetRequiredService<ValidationService>().Validate(input);
            return JsonConvert.SerializeObject(res);
        }
    }
}
