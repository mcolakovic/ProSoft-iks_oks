using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class Response
    {
        public bool IsSuccessful { get; set; }
        public string MessageText { get; set; }
        public Object ResponseObject { get; set; }
    }
}
