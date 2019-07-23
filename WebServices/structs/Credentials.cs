using Common;
using Common.EncoDeco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class Credentials
    {
        string _PasswordHash;

        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get {
                if (string.IsNullOrWhiteSpace(_PasswordHash))
                    return SHA256.Encode(Password);
                else
                    return _PasswordHash;
            } set { _PasswordHash = value; } }
        public bool RememberMe { get; set; }    
    }
}
