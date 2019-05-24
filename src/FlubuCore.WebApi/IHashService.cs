namespace FlubuCore.WebApi
{
    public interface IHashService
    {
        string Hash(string valueToHash);

        bool Validate(string valueToValidate, string correctHash);
    }
}
