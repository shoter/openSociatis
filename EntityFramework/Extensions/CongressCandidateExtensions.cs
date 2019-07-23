using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CongressCandidateExtensions
    {
        public static bool Is(this CongressCandidate candiate, CongressCandidateStatusEnum status)
        {
            return candiate.CongressCandidateStatusID == (int)status;
        }
    }
}
