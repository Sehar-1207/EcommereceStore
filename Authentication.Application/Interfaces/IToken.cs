using Authentication.Domain.Entities;

public interface IToken
{
    Task<string> CreateAccessTokenAsync(Appuser user);
}