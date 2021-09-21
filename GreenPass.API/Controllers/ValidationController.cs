using DGCValidator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GreenPass.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly ILogger<ValidationController> _logger;
        private readonly ValidationService _svc;

        public ValidationController(IServiceProvider sp, ILogger<ValidationController> logger)
        {
            _logger = logger;
            _svc = sp.GetRequiredService<ValidationService>();
        }
        /// <summary>
        /// Validates the given GreenPass and returns the result in boolean
        /// </summary>
        /// <param name="input">GreenPass raw string e.g. HC1:... </param>
        /// <returns></returns>
        [HttpPost("bool")]
        [Description("Validates the given GreenPass and returns the result in JSON format. Input example: HC1:...")]
        public async Task<bool> Validate(string input)
        {
            return !(await _svc.Validate(input)).IsInvalid;
        }
        /// <summary>
        /// Validates the given GreenPass and returns the result in JSON format
        /// </summary>
        /// <param name="input">GreenPass raw string e.g. HC1:... </param>
        /// <returns></returns>
        [HttpPost()]
        [HttpPost("data")]
        [Description("Validates the given GreenPass and returns the result in JSON format. Input example: HC1:...")]
        public async Task<SignedDGC> GetData(string input)
        {
            return (await _svc.Validate(input));
        }
    }
}
