using LabBooking.Application.Common.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LabBooking.API.Filters;

public class UnifiedResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            // Kiểm tra xem dữ liệu đã là ApiResponse chưa (tránh bọc 2 lần)
            var resultType = objectResult.Value?.GetType();
            if (resultType != null && resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                await next();
                return;
            }

            // Bọc dữ liệu lại
            var response = new ApiResponse<object>(
                objectResult.StatusCode ?? 200,
                "Success", // Hoặc lấy message tùy chỉnh
                objectResult.Value
            );

            context.Result = new ObjectResult(response)
            {
                StatusCode = objectResult.StatusCode
            };
        }
        // Nếu kết quả là Empty (ví dụ: return NoContent() hoặc void)
        else if (context.Result is StatusCodeResult statusCodeResult || context.Result is EmptyResult)
        {
            // Thường NoContent là 204 và không có body. 
            // Nhưng nếu bạn muốn trả về JSON chuẩn ngay cả khi update/delete thành công:
            var status = (context.Result as StatusCodeResult)?.StatusCode ?? 200;

            // Chuyển 204 thành 200 để có thể trả về body JSON (tuỳ chọn của bạn)
            var finalStatus = status == 204 ? 200 : status;

            var response = new ApiResponse<object>(
               finalStatus,
               "Success",
               null
           );

            context.Result = new ObjectResult(response)
            {
                StatusCode = finalStatus
            };
        }

        await next();
    }
}
