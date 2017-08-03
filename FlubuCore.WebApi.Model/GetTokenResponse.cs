using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class GetTokenResponse
    {
        public string Token { get; set; }

        public int ExpiresIn { get; set; }
    }
}
