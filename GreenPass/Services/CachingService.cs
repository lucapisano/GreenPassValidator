using GreenPass.Models;
using GreenPass.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenPass.Services
{
    public class CachingService
    {
        private readonly IServiceProvider _sp;
        private readonly IOptions<ValidatorOptions> _opt;

        public CachingService(IServiceProvider sp)
        {
            _sp = sp;
            _opt = _sp.GetRequiredService<IOptions<ValidatorOptions>>();
        }

        List<RemoteRule> rules { get; set; } = new List<RemoteRule>();
        public async Task<List<RemoteRule>> GetRules()
        {
            if (!rules?.Any() ?? false)
            {
                var req = new RestRequest(_opt.Value.RulesProviderUrl);
                var res = new RestClient().Get(req);
                if (res.IsSuccessful)
                    rules = JsonConvert.DeserializeObject<List<RemoteRule>>(res.Content);
            }
            return rules;
        }
    }
}
