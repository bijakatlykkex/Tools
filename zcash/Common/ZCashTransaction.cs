using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ScriptPubKey
    {
        public List<string> addresses { get; set; }
        public string asm { get; set; }
        public string hex { get; set; }
        public int reqSigs { get; set; }
        public string type { get; set; }
    }

    public class RetrievedVout
    {
        public int n { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
        public double value { get; set; }
    }

    public class ScriptSig
    {
        public string asm { get; set; }
        public string hex { get; set; }
    }

    public class Vin
    {
        public RetrievedVout retrievedVout { get; set; }
        public ScriptSig scriptSig { get; set; }
        public double sequence { get; set; }
        public string txid { get; set; }
        public int vout { get; set; }
    }

    public class ScriptPubKey2
    {
        public List<string> addresses { get; set; }
        public string asm { get; set; }
        public string hex { get; set; }
        public int reqSigs { get; set; }
        public string type { get; set; }
    }

    public class Vout
    {
        public int n { get; set; }
        public ScriptPubKey2 scriptPubKey { get; set; }
        public double value { get; set; }
    }

    public class Vjoinsplit
    {
        public string anchor { get; set; }
        public List<string> ciphertexts { get; set; }
        public List<string> commitments { get; set; }
        public List<string> macs { get; set; }
        public List<string> nullifiers { get; set; }
        public string onetimePubKey { get; set; }
        public string proof { get; set; }
        public string randomSeed { get; set; }
        public double vpub_new { get; set; }
        public int vpub_old { get; set; }
    }

    public class ZCashTransaction
    {
        public string hash { get; set; }
        public bool mainChain { get; set; }
        public double fee { get; set; }
        public string type { get; set; }
        public bool shielded { get; set; }
        public int index { get; set; }
        public string blockHash { get; set; }
        public int blockHeight { get; set; }
        public int version { get; set; }
        public int lockTime { get; set; }
        public int timestamp { get; set; }
        public int time { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public List<Vjoinsplit> vjoinsplit { get; set; }
        public double value { get; set; }
        public double outputValue { get; set; }
        public double shieldedValue { get; set; }
    }
}
