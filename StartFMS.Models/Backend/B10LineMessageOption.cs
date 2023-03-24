using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class B10LineMessageOption
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string IsUse { get; set; } = null!;
    }
}
