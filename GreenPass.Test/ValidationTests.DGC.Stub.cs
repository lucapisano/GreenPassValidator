using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenPass.Test
{
    /*
     *     _____ __  __ _____   ____  _____ _______       _   _ _______ 
     *    |_   _|  \/  |  __ \ / __ \|  __ \__   __|/\   | \ | |__   __|
     *      | | | \  / | |__) | |  | | |__) | | |  /  \  |  \| |  | |   
     *      | | | |\/| |  ___/| |  | |  _  /  | | / /\ \ | . ` |  | |   
     *     _| |_| |  | | |    | |__| | | \ \  | |/ ____ \| |\  |  | |   
     *    |_____|_|  |_|_|     \____/|_|  \_\ |_/_/    \_\_| \_|  |_|   
     *                                                          
     *                                                        
     * 
     *      This is a stub. Please copy this file to ValidationTests.DGC.cs and fill in your own DGCs for testing
     * 
     */
    partial class ValidationTests
    {
        // 3G VALIDATION
        static string DGC_Pfizer_VALID_3G = string.Empty;
        static string DGC_Moderna_VALID_3G = string.Empty;
        static string DGC_Johnson_VALID_3G = string.Empty;
        static string DGC_CovidTest_VALID_3G = string.Empty;
        static string DGC_Recovery_VALID_3G = string.Empty;

        // DGC for first dose should be invalid if enough time has passed.
        static string DGC_Pfizer_INVALID = string.Empty;
        static string DGC_Moderna_INVALID = string.Empty;
        static string DGC_Johnson_INVALID = string.Empty;
        static string DGC_CovidTest_INVALID_3G = string.Empty;
        static string DGC_Recovery_INVALID = string.Empty;

        // 2G VALIDATION
        static string DGC_Pfizer_VALID_2G = string.Empty;
        static string DGC_Moderna_VALID_2G = string.Empty;
        static string DGC_Johnson_VALID_2G = string.Empty;
        static string DGC_Recovery_VALID_2G = string.Empty;

        // BOOSTER VALIDATION
        static string DGC_Pfizer_VALID_BOOSTER = string.Empty;
        static string DGC_Pfizer_NEEDSTEST = string.Empty;
        static string DGC_Moderna_VALID_BOOSTER = string.Empty;
        static string DGC_Moderna_NEEDSTEST = string.Empty;
        static string DGC_Johnson_VALID_BOOSTER = string.Empty;
        static string DGC_Johnson_NEEDSTEST = string.Empty;

    }
}
