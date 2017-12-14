using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Configuration;

namespace TransactionDownloader
{
    public class Settings
    {

        public string ZCashAddress
        {
            get;
            set;
        }

        public string OutputDir
        {
            get;
            set;
        }

        public string DesiredSourceAddress
        {
            get;
            set;
        }

        public Settings()
        {
            ZCashAddress = ConfigurationManager.AppSettings["ZCashAddress"];
            OutputDir = ConfigurationManager.AppSettings["OutputDir"];
            DesiredSourceAddress = ConfigurationManager.AppSettings["DesiredSourceAddress"];
        }
    }
}