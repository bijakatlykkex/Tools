using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TransactionDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            DoWork().Wait();

            System.Console.WriteLine("Press any key to exit ...");
            System.Console.ReadLine();
        }


        private static async Task<List<ZCashTransaction>> GetZCashTransactionList(Settings settings)
        {
            int count = 0;

            List<ZCashTransaction> txList = new List<ZCashTransaction>();

            while (true)
            {
                System.Console.WriteLine(string.Format("Count: {0}, Interval: {1}", count, settings.GetInterval));

                var url = string.Format("{0}/accounts/{1}/recv?limit={2}&offset={3}",
                        settings.ExplorerUrl, settings.ZCashAddress, settings.GetInterval, settings.GetInterval * count++);
                
                var errorCount = 0;

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage result = await client.GetAsync(url);
                    if (!result.IsSuccessStatusCode)
                    {
                        if (errorCount > settings.NumberOfTries)
                        {
                            throw new Exception(string.Format("An error occurred while retrieving url {0}. Error code is {1}.",
                                url, settings.NumberOfTries));
                        }
                    }
                    else
                    {
                        var webResponse = await result.Content.ReadAsStringAsync();

                        var tempTxList = Newtonsoft.Json.JsonConvert.DeserializeObject<ZCashTransaction[]>(webResponse);

                        if (tempTxList.Count() == 0)
                        {
                            break;
                        }
                        else
                        {
                            txList.AddRange(tempTxList);
                        }
                    }
                }
            }

            return txList;
        }

        public static async Task DoWork()
        {
            Settings settings = new Settings();

            StringBuilder builder = new StringBuilder();

            var txList = await GetZCashTransactionList(settings);

            using (StreamWriter writer = new StreamWriter(string.Format("{0}\\log.txt", settings.OutputDir)))
            {
                string json = JsonConvert.SerializeObject(txList);

                writer.WriteLine(json);
            }
        }
    }
}
