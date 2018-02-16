using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class GetTokenRequest
    {
         public const string WrongUsernamePassword = "WrongUsernameOrPassword";

        public string Username { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return $"Username: '{Username}'";
        }
    }
}
