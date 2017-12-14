using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalsePositiveTest.Models
{
    [Serializable]
    public class SerializableCoin
    {
        public uint OutputNumber
        {
            get;
            set;
        }

        public Serializableuint256 TransactionId
        {
            get;
            set;
        }
    }
}
