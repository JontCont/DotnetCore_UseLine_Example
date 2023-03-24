using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class B10LineMessageType
    {
        public string TypeId { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string? TypeMemo { get; set; }
    }
}
