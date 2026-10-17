
using Microsoft.AspNetCore.Identity;

namespace Marketplace_Joias.Models;

public class PasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string GerarHash(string senha)
    {
        return _hasher.HashPassword(null!, senha);
    }

    public bool VerificarSenha(string senhaHash, string senhaInformada)
    {
        var resultado = _hasher.VerifyHashedPassword(
            null!,
            senhaHash,
            senhaInformada
        );

        return resultado != PasswordVerificationResult.Failed;
    }
}