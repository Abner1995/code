namespace Todo.Core;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // 成功响应
    public static ApiResponse<T> Success(T data, string message = "成功")
        => new() { Code = 200, Message = message, Data = data };

    // 失败响应
    public static ApiResponse<T> Fail(int code, string message, T? data = default)
        => new() { Code = code, Message = message, Data = data };
}
