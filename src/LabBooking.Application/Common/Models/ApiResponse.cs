namespace LabBooking.Application.Common.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }

    public ApiResponse(int statusCode, string message, T? data = default, object? errors = null)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
        Errors = errors;
    }
}
