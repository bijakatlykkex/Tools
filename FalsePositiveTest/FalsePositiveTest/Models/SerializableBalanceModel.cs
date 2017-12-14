using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalsePositiveTest.Models
{
    [Serializable]
    public class SerializableBalanceModel
    {
        public List<SerializableBalanceOperation> ConflictedOperations { get; set; }
        public string Continuation { get; set; }
        public List<SerializableBalanceOperation> Operations { get; set; }
    }
}
