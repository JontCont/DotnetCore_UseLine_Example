using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFMS.Models.Line {
    internal class Root {
        public string type { get; set; }
        public string text { get; set; }
        public QuickReply quickReply { get; set; }
    }
}
