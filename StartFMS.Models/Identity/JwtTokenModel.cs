using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFMS.Models.Identity;
public partial class JwtTokenModel {
    public string sub { get; set; }
    public string jti { get; set; }
    public string roles { get; set; }
    public string grp_code { get; set; }
    public string token { get; set; }
    public string org { get; set; }
    public string nbf { get; set; }
    public string exp { get; set; }
    public string iat { get; set; }
    public string iss { get; set; }
}
