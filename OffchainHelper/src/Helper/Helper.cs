using NBitcoin;
using NBitcoin.BouncyCastle.Math;
using OffchainHelper.Entities;
using OffchainHelper.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffchainHelper.Helper
{
    public static class Helper
    {
        public static async Task<IDictionary<string, uint>> ReadWordList()
        {
            var retDict = new Dictionary<string, uint>();

            using (StreamReader reader = new StreamReader("data\\wordlist.txt"))
            {
                var wordlist = await reader.ReadToEndAsync();
                var words = wordlist.Split(new char[] { '\r', '\n', ' ' }).Where(w => !string.IsNullOrEmpty(w)).ToArray();
                
                for (uint i = 0; i < words.Count(); i++)
                {
                    retDict.Add(words[i], i);
                }
            }

            return retDict;
        }

        public static Key GenerateKeyFrom12Words(string[] splittedWordList, IDictionary<string, uint> dictionary)
        {
            uint[] indexes = new uint[12];

            BigInteger binaryKey = new BigInteger(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            for (int i = 0; i < 12; i++)
            {
                indexes[i] = dictionary[splittedWordList[i]];
                binaryKey = binaryKey.ShiftLeft(11);
                binaryKey = binaryKey.Add(BigInteger.ValueOf(indexes[i]));
            }

            binaryKey = binaryKey.ShiftRight(4);
            var nonZeroArray = binaryKey.ToByteArrayUnsigned();
            Key key = new Key((new byte[32 - nonZeroArray.Length]).Concat(nonZeroArray).ToArray());

            return key;
        }

        public static async Task<string> SignTransactionWorker(TransactionSignRequest signRequest, SigHash sigHash = SigHash.All)
        {
            BlockchainExplorerHelper daemonHelper = new BlockchainExplorerHelper();
            var settings = Settings.ReadAppSettings();

            Transaction tx = new Transaction(signRequest.TransactionToSign);
            Transaction outputTx = new Transaction(signRequest.TransactionToSign);
            var secret = new BitcoinSecret(signRequest.PrivateKey);

            TransactionBuilder builder = new TransactionBuilder();
            builder.ContinueToBuild(tx);

            Transaction[] previousTransactions = new Transaction[tx.Inputs.Count];
            for (int i = 0; i < previousTransactions.Count(); i++)
            {
                var txResponse = await daemonHelper.GetTransactionHex(settings, tx.Inputs[i].PrevOut.Hash.ToString());

                if (txResponse.HasErrorOccurred)
                {
                    throw new Exception(string.Format("Error while retrieving transaction {0}, error is: {1}",
                        tx.Inputs[i].PrevOut.Hash.ToString(), txResponse.Error));
                }

                previousTransactions[i] = new Transaction(txResponse.TransactionHex);

                builder.AddCoins(new Coin(previousTransactions[i], tx.Inputs[i].PrevOut.N));
            }
            tx = builder.AddKeys(new BitcoinSecret[] { secret }).SignTransaction(tx, sigHash);

            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                var input = tx.Inputs[i];

                var prevTransaction = previousTransactions[i];
                var output = prevTransaction.Outputs[input.PrevOut.N];

                Coin c = new Coin(prevTransaction, input.PrevOut.N);
                builder.AddCoins(c);

                if (PayToScriptHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    var redeemScript = PayToScriptHashTemplate.Instance.ExtractScriptSigParameters(input.ScriptSig).RedeemScript;
                    if (PayToMultiSigTemplate.Instance.CheckScriptPubKey(redeemScript))
                    {
                        var pubkeys = PayToMultiSigTemplate.Instance.ExtractScriptPubKeyParameters(redeemScript).PubKeys;
                        for (int j = 0; j < pubkeys.Length; j++)
                        {
                            if (secret.PubKey.ToHex() == pubkeys[j].ToHex())
                            {
                                var scriptParams = PayToScriptHashTemplate.Instance.ExtractScriptSigParameters(input.ScriptSig);
                                var hash = Script.SignatureHash(scriptParams.RedeemScript, tx, i, sigHash);
                                var signature = secret.PrivateKey.Sign(hash, sigHash);
                                scriptParams.Pushes[j + 1] = signature.Signature.ToDER().Concat(new byte[] { (byte)sigHash }).ToArray();
                                outputTx.Inputs[i].ScriptSig = PayToScriptHashTemplate.Instance.GenerateScriptSig(scriptParams);
                            }
                        }
                    }
                    continue;
                }

                if (PayToPubkeyHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey))
                {
                    var address = PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(output.ScriptPubKey).GetAddress(settings.Network).ToWif();
                    if (address == secret.GetAddress().ToWif())
                    {
                        var hash = Script.SignatureHash(output.ScriptPubKey, tx, i, sigHash);
                        var signature = secret.PrivateKey.Sign(hash, sigHash);

                        outputTx.Inputs[i].ScriptSig = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(signature, secret.PubKey);
                    }

                    continue;
                }
            }

            return outputTx.ToHex();
        }
    }
}
