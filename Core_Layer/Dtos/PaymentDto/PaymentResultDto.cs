namespace Busines_Layer.Dtos.PaymentDto;

    public record PaymentResultDto(bool Success, string? RefId, string Message);
