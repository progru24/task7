using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task7.Contracts;

namespace Task7.Models
{
    internal class ResponseInfo
    {
        public IResponse? Response { get; set; }
        public int RetriesCount { get; set; }
    }
}
