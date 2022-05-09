namespace MySpot.Services.Users.Core.Services;

public interface IPasswordManager
{
    string Secure(string password);
    bool IsValid(string password, string securedPassword);
}