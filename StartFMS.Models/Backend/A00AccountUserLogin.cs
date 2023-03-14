using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class A00AccountUserLogin
    {
        public string LoginProvider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public string? ProviderDisplayName { get; set; }
        public string UserId { get; set; } = null!;

        public virtual A00AccountUser User { get; set; } = null!;
    }
}
