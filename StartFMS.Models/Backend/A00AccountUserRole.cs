using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class A00AccountUserRole
    {
        public string UserId { get; set; } = null!;
        public string RoleId { get; set; } = null!;

        public virtual A01AccountRole Role { get; set; } = null!;
    }
}
