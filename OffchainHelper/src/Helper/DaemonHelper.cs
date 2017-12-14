using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using OffchainHelper.Entities;

namespace OffchainHelper.Helper
{
    public class BlockchainExplorerHelper
    {
        public async Task<GetTransactionHexResult> GetTransactionHex(Settings settings, string transactionId)
        {
            string transactionHex = string.Empty;
            bool errorOccured = false;
            string errorMessage = string.Empty;

            try
            {
                QBitNinja.Client.QBitNinjaClient client = new QBitNinja.Client.QBitNinjaClient(settings.QBitNinjaUrl, settings.Network);
                var qbitNinjaResponse = await client.GetTransaction(new uint256(transactionId));
                transactionHex = qbitNinjaResponse.Transaction.ToHex();
            }
            catch (Exception exp)
            {
                errorOccured = true;
                errorMessage = exp.ToString();
            }

            return new GetTransactionHexResult { HasErrorOccurred = errorOccured,
                TransactionHex = transactionHex, Error = errorMessage };
        }
    }
}
