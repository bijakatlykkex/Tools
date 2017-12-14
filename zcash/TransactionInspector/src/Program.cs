using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionDownloader;

namespace TransactionInspector
{
    class Program
    {
        static void Main(string[] args)
        {
            DoWork().Wait();
            Console.WriteLine("Press any key to stop.");
            Console.ReadLine();
        }

        private static async Task<ZCashTransaction[]> GetTransactionsFromFile(Settings settings)
        {
            using (StreamReader reader = new StreamReader(string.Format("{0}\\log.txt", settings.OutputDir)))
            {
                return JsonConvert.DeserializeObject<ZCashTransaction[]>(await reader.ReadToEndAsync());
            }
        }

        private static async Task DoWork()
        {
            Settings settings = new Settings();
            var txList = await GetTransactionsFromFile(settings);
            foreach(var tx in txList)
            {
                if(tx.shielded == true)
                {
                    System.Console.WriteLine(string.Format("Transaction {0} is shielded.", tx.hash));
                }

                foreach(var input in tx.vin)
                {
                    var addresses = input.retrievedVout.scriptPubKey.addresses;
                    if(addresses.Count != 1)
                    {
                        Console.WriteLine(string.Format("Transaction {0} has vin with more than one.", tx.hash));
                    }
                    if(!addresses.Contains(settings.DesiredSourceAddress))
                    {
                        Console.WriteLine(string.Format("Transaction {0} has does not have the desired source address {1}.",
                            tx.hash, settings.DesiredSourceAddress));
                    }
                }

                var addrValue = tx.vout.Where(o => o.scriptPubKey.addresses[0] == settings.ZCashAddress).FirstOrDefault().value;

                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                System.Console.WriteLine("{0},{1},{2}", tx.hash, epoch.AddSeconds(tx.timestamp), addrValue);
            }
        }
    }
}
