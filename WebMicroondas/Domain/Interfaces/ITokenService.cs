namespace WebMicroondas.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string username);
    }
}
