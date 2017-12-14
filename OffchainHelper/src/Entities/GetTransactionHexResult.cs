using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffchainHelper.Entities
{
    public class GetTransactionHexResult
    {
        public string TransactionHex
        {
            get;
            set;
        }

        public bool HasErrorOccurred
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }
    }
}
