using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallel_Rabin_Karp_Application
{
    public interface IVerify
    {
        public bool isMeetReq(string testDoc, string trainDoc);
    }
}
