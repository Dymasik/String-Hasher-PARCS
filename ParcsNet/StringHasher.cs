using Parcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParcsNet
{
    class StringHasher : IModule
    {
        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            var textOptions = (HashStringOptions)info.Parent.ReadObject(typeof(HashStringOptions));
            textOptions.SetHash(token);
            info.Parent.WriteObject(textOptions.Hash);
        }
    }
}
