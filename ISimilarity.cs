using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallel_Rabin_Karp_Application
{
    public interface ISimilarity
    {
        public float similarityMeasure(int lengthText, int lengthPatt, int stringFound);
    }
}
