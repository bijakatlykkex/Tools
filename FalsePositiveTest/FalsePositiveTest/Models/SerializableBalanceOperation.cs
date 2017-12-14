using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalsePositiveTest.Models
{
    [Serializable]
    public class SerializableBalanceOperation
    {
        public SerializableMoney Amount { get; set; }
        public Serializableuint256 BlockId { get; set; }
        public int Confirmations { get; set; }
        public DateTimeOffset FirstSeen { get; set; }
        public int Height { get; set; }
        public List<SerializableCoin> ReceivedCoins { get; set; }
        public List<SerializableCoin> SpentCoins { get; set; }
        public Serializableuint256 TransactionId { get; set; }
    }
}
