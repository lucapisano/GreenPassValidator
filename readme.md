# GreenPass
## By [Luca Pisano](https://lucapisano.it)

This package enables Green Pass QR code reading and reading

## Features

- Decode QR Code string into readable JSON
- Verify GreenPass using official Trust List servers
- Validation certificates caching for configurable TimeSpan to improve performance (default is 24h)
- Support for custom Trust List servers is provided using appsettings.json

Current implementation does not check for certificate revocation lists, if any jurisdiction uses them.

```csharp
//In ASP.NET Core projects, call .AddGreenPassValidator() in ConfigureServices() method
public void ConfigureServices(IServiceCollection services)
        {
            services.AddGreenPassValidator(Configuration)
            ;            
        }
```

This is an example of how you can configure it
```json
{
  "ValidatorOptions": {
    "CertificateListProviderUrl": "https://dgcg.covidbevis.se/tp/trust-list"    
    }
}
```
Special thanks to the authors of DGCValidator https://github.com/ehn-dcc-development/DGCValidator

Any artifact present in this repository, code included, is made under pure personal terms and is not related to my job.