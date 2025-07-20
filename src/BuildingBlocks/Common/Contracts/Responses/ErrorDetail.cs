namespace Common.Contracts.Responses;

public class ErrorDetail
{
    public string? Code { get; set; }
    public string? Field { get; set; }
    public string Message { get; set; }
}