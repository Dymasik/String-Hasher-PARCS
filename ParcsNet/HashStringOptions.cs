using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParcsNet
{
    [Serializable]
    class HashStringOptions
    {
        private readonly BigInteger PRIME = 31;

        public string Text { get; set; }
        public int StartPosition { get; set; }
        public BigInteger Hash { get; set; }

        public void SetHash(CancellationToken token) {
            try
            {
                token.ThrowIfCancellationRequested();
                for (int i = 0; i < Text.Length; i++) {
                    Hash += BigInteger.Pow(PRIME, StartPosition + i) * (Text[i] - 'a' + 1);
                }
            }
            catch (Exception) { 
            
            }
        }
    }
}
