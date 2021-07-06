namespace FlubuCore.LiteDb.Models
{
    public class Security
    {
        public int Id { get; set; }

        public bool ApiAccessDisabled { get; set; }

        public int FailedGetTokenAttempts { get; set; }
    }
}
