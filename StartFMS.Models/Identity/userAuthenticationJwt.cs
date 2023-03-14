using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFMS.Models.Identity;
public partial class userAuthenticationJwt {
    public string Org { get; set; }
    public string User { get; set; }
    public string Token { get; set; }
    public string Roles { get; set; }  // for bdp090A 權限
    public string SysCode { get; set; }
}
