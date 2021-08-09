namespace FlubuCore.LiteDb
{
    public interface IHashService
    {
        string Hash(string valueToHash);

        bool Validate(string valueToValidate, string correctHash);
    }
}
