using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Models
{
    public class Security
    {
        public int Id { get; set; }

        public bool ApiAccessDisabled { get; set; }

        public int FailedGetTokenAttempts { get; set; }
    }
}
