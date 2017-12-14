using FalsePositiveTest.Models;
using NBitcoin;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalsePositiveTest
{
    public static class Extensions
    {
        public static SerializableBalanceOperation GetSerializable(this BalanceOperation bo)
        {
            return new SerializableBalanceOperation
            {
                Amount = new SerializableMoney { Satoshi = bo.Amount },
                BlockId = new Serializableuint256 { Value = bo.BlockId.ToString() },
                Confirmations = bo.Confirmations,
                FirstSeen = bo.FirstSeen,
                Height = bo.Height,
                ReceivedCoins = bo.ReceivedCoins.Select(c => c.GetSerializable()).ToList(),
                SpentCoins = bo.SpentCoins.Select(c => c.GetSerializable()).ToList(),
                TransactionId = new Serializableuint256 { Value = bo.TransactionId.ToString() }
            };
        }

        public static SerializableCoin GetSerializable(this ICoin coin)
        {
            return new SerializableCoin
            {
                TransactionId = new Serializableuint256 { Value = coin.Outpoint.Hash.ToString() },
                OutputNumber = coin.Outpoint.N
            };
        }

        public static SerializableBalanceModel GetSerializable(this BalanceModel balanceModel)
        {
            return new SerializableBalanceModel
            {
                ConflictedOperations = balanceModel.ConflictedOperations.Select(bo => bo.GetSerializable()).ToList(),
                Continuation = balanceModel.Continuation,
                Operations = balanceModel.Operations.Select(bo => bo.GetSerializable()).ToList()
            };
        }
    }
}
