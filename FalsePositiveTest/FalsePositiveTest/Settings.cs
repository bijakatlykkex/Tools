using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NBitcoin;

namespace FalsePositiveTest
{
    public class Settings
    {

        public Network Network
        {
            get;
            set;
        }

        public string BaseUrl
        {
            get;
            set;
        }

        public BitcoinAddress Address
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

        // Some unspent coins may get spent while getting the unspent in some retries
        public int RequiredConfirmations
        {
            get;
            set;
        }

        public Settings()
        {
            Network = ConfigurationManager.AppSettings["Network"].ToLower() == "main" ?
                Network.Main : Network.TestNet;
            BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            Address = BitcoinAddress.GetFromBase58Data(ConfigurationManager.AppSettings["Address"]) as BitcoinAddress;
            NumberOfTries = Int32.Parse(ConfigurationManager.AppSettings["NumberOfTries"]);
            OutputDir = ConfigurationManager.AppSettings["OutputDir"];
            RequiredConfirmations = Int32.Parse(ConfigurationManager.AppSettings["RequiredConfirmations"]);
        }
    }
}
