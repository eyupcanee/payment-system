namespace Common.Contracts.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<ErrorDetail> Errors { get; set; }
    
    public static ApiResponse<T> SuccessResponse(T data,int statusCode = 200,string message = "") => new()
    {
        Success = true,
        StatusCode = statusCode,
        Message = message,
        Data = data
    };
    
    public static ApiResponse<T> FailResponse(string message,int statusCode = 400, List<ErrorDetail> errors = null) =>
    new()
    {
        Success = false,
        StatusCode = statusCode,
        Message = message,
        Errors = errors 
    };
    
}