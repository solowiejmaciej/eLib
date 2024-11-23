using eLib.Common.Enums;

namespace eLib.Common.Dtos;

public class TwoStepCodeDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public ECodeType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
}