using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class A00AccountUserToken
    {
        public string UserId { get; set; } = null!;
        public string LoginProvider { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Value { get; set; }
    }
}
