using Newtonsoft.Json;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using NBitcoin;
using FalsePositiveTest.Models;
using System.Collections.Concurrent;

namespace FalsePositiveTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DoWork().Wait();
        }

        public static void SaveStringToFile(string str, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(str);
            }
        }

        public static void SaveObjectToFile<T>(T obj, string filename)
        {
            byte[] objBytes = null;
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                objBytes = ms.ToArray();
            }

            using (var file = new FileStream(filename, FileMode.Create))
            {
                using (var stream = new BinaryWriter(file))
                {
                    stream.Write(objBytes);
                }
            }
        }

        private static async Task GetUnspentOnce(Settings settings, int count)
        {
            QBitNinja.Client.QBitNinjaClient client = new QBitNinja.Client.QBitNinjaClient
                    (settings.BaseUrl, settings.Network);

            var balanceOutput = await client.GetBalance(settings.Address, true);
            var serializableBalanceOutput = balanceOutput.GetSerializable();

            var serializedBalance = JsonConvert.SerializeObject(serializableBalanceOutput);
            var filename = String.Format("{0}\\log{1}.txt", settings.OutputDir, count);
            SaveStringToFile(serializedBalance, filename);
        }

        private static async Task<SerializableBalanceModel> ReadUpspentFromFile(Settings settings, int count)
        {
            var filename = String.Format("{0}\\log{1}.txt", settings.OutputDir, count);
            string balanceModel = null;

            using (StreamReader reader = new StreamReader(filename))
            {
                balanceModel = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<SerializableBalanceModel>(balanceModel);
            }
        }

        public static async Task<IList<SerializableCoin>> GetSpentCoins(Settings settings)
        {
            ConcurrentBag<SerializableCoin> coinCollection = new ConcurrentBag<SerializableCoin>();
            QBitNinja.Client.QBitNinjaClient client = new QBitNinja.Client.QBitNinjaClient
                    (settings.BaseUrl, settings.Network);

            var balanceOutput = await client.GetBalance(settings.Address, false);
            var serializableBalanceOutput = balanceOutput.GetSerializable();
            serializableBalanceOutput.Operations
                .Where(o => o.Confirmations >= settings.RequiredConfirmations)
                .Select(o => o.SpentCoins).ToList().ForEach(u => u.ForEach(uItem => coinCollection.Add(uItem)));

            return coinCollection.ToList();
        }

        public static async Task DoWork()
        {
            Settings settings = new Settings();

            Console.WriteLine(string.Format("Getting the address unspent coins for {0} times ...",
                settings.NumberOfTries));
            for (int i = 0; i < settings.NumberOfTries; i++)
            {
                Console.WriteLine(i);
                try
                {
                    await GetUnspentOnce(settings, i);
                    Thread.Sleep(5000);
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());
                }
            }

            Console.WriteLine(string.Format("Getting the address spent coins"));
            var spentCoins = (await GetSpentCoins(settings)).ToList();

            Console.WriteLine(string.Format("Checking the unspents confirm to rules."));
            for (int i = 0; i < settings.NumberOfTries; i++)
            {
                ConcurrentBag<SerializableCoin> unspentCoins = new ConcurrentBag<SerializableCoin>();

                Console.WriteLine(i);
                var balanceModel = await ReadUpspentFromFile(settings, i);
                balanceModel.Operations
                .Select(o => o.ReceivedCoins).ToList().ForEach(u => u.ForEach(uItem => unspentCoins.Add(uItem)));

                unspentCoins.ToList().ForEach(u =>
                {
                    spentCoins.ForEach(s => 
                    {
                        if (s.TransactionId.Value == u.TransactionId.Value && s.OutputNumber == u.OutputNumber)
                        {
                            Console.WriteLine(
                              string.Format("Try number {0} has maked transaction {1}, output number {2} as spent.",
                              i,
                              u.TransactionId.Value,
                              u.OutputNumber));
                        }
                    });
                });
            }
        }
    }
}
