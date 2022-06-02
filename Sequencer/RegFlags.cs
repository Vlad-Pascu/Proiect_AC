using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sequencer
{
    public class RegFlags
    {
        //R
        public static short[] RG = new short[16];
        public static short SP;
        public static short T;
        public static short PC;
        public static short IVR;
        public static short ADR;
        public static short MDR;
        public static short IR;
        public static short FLAGS;

        // Flaguri
        public static short Z;
        public static short S;
        public static short C;
        public static short V;
        public static short BE0, BE1;
        public static short BVI, BPO,BI;
        public static short CIL, ACLOW;
    }
}
