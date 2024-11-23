using eLib.Common.Dtos;
using eLib.Common.Enums;
using eLib.Models.Results;
using eLib.Models.Results.Base;

namespace eLib.DAL.Entities;

public class TwoStepCode : Entity
{
    public TwoStepCode() { }

    public static TwoStepCode Create(
        Guid userId,
        ECodeType type)
    {
        var code = GenerateCode();
        return new TwoStepCode(userId, code, type, DateTime.UtcNow.AddMinutes(15));
    }

    private TwoStepCode(
        Guid userId,
        string code,
        ECodeType type,
        DateTime expiresAt)
    {
        UserId = userId;
        Code = code;
        Type = type;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public ECodeType Type { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }


    private static string GenerateCode()
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int codeLength = 6;

        using var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        var bytes = new byte[codeLength];

        var chars = new char[codeLength];
        for (int i = 0; i < codeLength; i++)
        {
            rng.GetBytes(bytes);
            chars[i] = allowedChars[bytes[0] % allowedChars.Length];
        }

        return new string(chars);
    }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public TwoStepCodeDto MapToDto()
    {
        return new TwoStepCodeDto
        {
            UserId = UserId,
            Code = Code,
            Type = Type,
            ExpiresAt = ExpiresAt
        };
    }

    public Error? Use(
        ECodeType desiredType,
        Guid userId
        )
    {
        if (UserId != userId)
            return TwoStepCodeErrors.CodeNotFound;

        if (Type != desiredType)
            return TwoStepCodeErrors.CodeNotFound;

        if (IsUsed)
            return TwoStepCodeErrors.CodeNotFound;

        if (ExpiresAt < DateTime.UtcNow)
            return TwoStepCodeErrors.CodeNotFound;

        IsUsed = true;

        return null;
    }
}