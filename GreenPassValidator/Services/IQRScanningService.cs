using System;
using System.Threading.Tasks;

namespace DGCValidator.Services
{
    /**
    * Interface for scanning.
    *
    * @author Henrik Bengtsson (henrik@sondaica.se)
    * @author Martin Lindström (martin@idsec.se)
    * @author Henric Norlander (extern.henric.norlander@digg.se)
    */
    public interface IQRScanningService
    {
        Task<String> ScanAsync();
    }
}
