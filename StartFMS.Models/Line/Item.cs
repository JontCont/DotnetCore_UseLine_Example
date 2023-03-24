using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFMS.Models.Line {
    internal class Item {
        public string type { get; set; }
        public string imageUrl { get; set; }
        public Action action { get; set; }
    }
}
