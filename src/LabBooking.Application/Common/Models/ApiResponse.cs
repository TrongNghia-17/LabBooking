namespace LabBooking.Application.Common.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }

    // Constructor cho trường hợp thành công
    public ApiResponse(int statusCode, string message, T? data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    // Constructor cho trường hợp lỗi (Data null)
    public ApiResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
        Data = default;
    }
}
