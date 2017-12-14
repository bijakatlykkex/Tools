using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TransactionDownloader
{
    public class Settings
    {

        public string ExplorerUrl
        {
            get;
            set;
        }

        public string ZCashAddress
        {
            get;
            set;
        }

        public int NumberOfTries
        {
            get;
            set;
        }

        public string OutputDir
        {
            get;
            set;
        }

        public int GetInterval
        {
            get;
            set;
        }

        // Some unspent coins may get spent while getting the unspent in some retries

        public Settings()
        {
            ExplorerUrl = ConfigurationManager.AppSettings["ExplorerUrl"];
            ZCashAddress = ConfigurationManager.AppSettings["ZCashAddress"];
            NumberOfTries = Int32.Parse(ConfigurationManager.AppSettings["NumberOfTries"]);
            OutputDir = ConfigurationManager.AppSettings["OutputDir"];
            GetInterval = Int32.Parse(ConfigurationManager.AppSettings["GetInterval"]);
        }
    }
}