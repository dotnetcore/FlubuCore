using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class GetTokenResponse
    {
        /// <summary>
        /// Authentication token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Time in seconds authentication token expires in.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
