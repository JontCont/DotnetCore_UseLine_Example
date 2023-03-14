using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class A00AccountUserClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual A00AccountUser User { get; set; } = null!;
    }
}
