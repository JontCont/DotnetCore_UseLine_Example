using System;
using System.Collections.Generic;

namespace StartFMS.Models.Backend
{
    public partial class A01AccountRole
    {
        public A01AccountRole()
        {
            A00AccountUserRoles = new HashSet<A00AccountUserRole>();
            A01AccountRoleClaims = new HashSet<A01AccountRoleClaim>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }

        public virtual ICollection<A00AccountUserRole> A00AccountUserRoles { get; set; }
        public virtual ICollection<A01AccountRoleClaim> A01AccountRoleClaims { get; set; }
    }
}
