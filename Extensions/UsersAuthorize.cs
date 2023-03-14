using Microsoft.Data.SqlClient;
using StartFMS.Models.Identity;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
namespace StartFMS_BackendAPI.Extensions;

internal class UsersAuthorize {
    public JwtTokenModel Users { get; set; }
    public string User => Users.sub;
    public string Organize => Users.org;
    public string ConnectString { get; set; }

    public UsersAuthorize(HttpRequest request) {
        string authHeader =
            (request.Headers.Where(x => x.Key == "Authorization").Any())
            ? request.Headers["Authorization"]
            : request.Cookies["x-access-token"];
        authHeader = authHeader.Replace("Bearer ", "");

        JwtSecurityTokenHandler handler = new();
        var securityToken = handler.ReadToken(authHeader) as JwtSecurityToken;
        Users = JwtHelpers.GetPayLoadData(securityToken);
    }
    public UsersAuthorize(string cookieKey, HttpRequest request) {
        string authHeader = request.Cookies[cookieKey];
        authHeader = authHeader.Replace("Bearer ", "");

        JwtSecurityTokenHandler handler = new();
        var securityToken = handler.ReadToken(authHeader) as JwtSecurityToken;
        Users = JwtHelpers.GetPayLoadData(securityToken);
    }

    /// <summary>
    /// 傳入一個SQL語法，回傳一個DataTable
    /// </summary>
    /// <param name="pSql">Select語法</param>
    /// <returns></returns>
    public DataTable GetDataTable(string pSql, string Connection = "") {
        DataTable datatable = new DataTable();
        try {
            if (pSql.Length > 0) {
                using SqlConnection con_db = new(string.IsNullOrEmpty(Connection) ? ConnectString : Connection);
                con_db.Open();
                SqlDataAdapter Adapter = new(pSql, con_db);
                Adapter.Fill(datatable);
                con_db.Close();
            }
            return datatable;
        }
        catch (Exception) { throw; }
    }


    /// <summary>
    /// 傳入SQL語法和自定義的SQL參數，回傳DataTable
    /// </summary>
    /// <param name="pSql">sql指令</param>
    /// <param name="pSqlParams">sql參數，如果值是null傳空字串</param>
    /// <returns></returns>
    public DataTable GetDataTable(string pSql, Dictionary<string, object> pSqlParams, string Connection = "") {
        DataTable datatable = new DataTable();
        try {
            if (pSql.Length > 0) {
                using SqlConnection con_db = new(string.IsNullOrEmpty(Connection) ? ConnectString : Connection);
                SqlCommand sqlCommand = new(pSql);
                sqlCommand.Connection = con_db;
                foreach (string key in pSqlParams.Keys) {
                    string paramName = key;
                    object paramValue = pSqlParams[key];
                    if (!string.IsNullOrEmpty(paramName) && paramName[0] != '@') {
                        paramName = "@" + paramName;
                    }
                    // 傳null會出錯
                    if (paramValue == null) {
                        paramValue = "";
                    }

                    sqlCommand.Parameters.Add(new SqlParameter(paramName, paramValue));
                }
                SqlDataReader reader = sqlCommand.ExecuteReader();
                datatable.Load(reader);
            }
            return datatable;
        }
        catch (Exception) { throw; }
    }

    /// <summary>
    /// 傳入SQL語法和自定義的SQL參數，回傳DataTable
    /// </summary>
    /// <param name="pSql">SQL</param>
    /// <param name="pSqlParams">自定義 Object</param>
    /// <returns></returns>
    public DataTable GetDataTable(string pSql, object pSqlParams, string Connection = "") {
        DataTable datatable = new DataTable();
        try {
            if (pSql.Length > 0) {
                using SqlConnection con_db = new(string.IsNullOrEmpty(Connection) ? ConnectString : Connection);
                SqlCommand sqlCommand = new(pSql);
                sqlCommand.Connection = con_db;
                var props = pSqlParams.GetType().GetProperties();
                foreach (var prop in props) {
                    string paramName = prop.Name;
                    object paramValue = prop.GetValue(pSqlParams);
                    if (paramValue == null) { paramValue = ""; }
                    sqlCommand.Parameters.Add(new SqlParameter(paramName, paramValue));
                }

                SqlDataReader reader = sqlCommand.ExecuteReader();
                datatable.Load(reader);
            }
            return datatable;
        }
        catch (Exception) { throw; }
    }
}
