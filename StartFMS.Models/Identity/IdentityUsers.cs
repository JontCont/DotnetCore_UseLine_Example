using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StartFMS.Models.Identity;

public class IdentityUsers {
    [JsonPropertyName("users")]
    public string Users { get; set; }
    [JsonPropertyName("pwd")]
    public string Password { get; set; }
    [JsonPropertyName("checked")]
    public bool CheckMe { get; set; }
}
