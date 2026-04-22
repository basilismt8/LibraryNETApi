namespace Library.Api.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Error { get; init; }
        public int StatusCode { get; init; } = 200;

        public static ServiceResult<T> Ok(T data) =>
            new() { Success = true, Data = data, StatusCode = 200 };

        public static ServiceResult<T> NotFound(string error) =>
            new() { Success = false, Error = error, StatusCode = 404 };

        public static ServiceResult<T> BadRequest(string error) =>
            new() { Success = false, Error = error, StatusCode = 400 };

        public static ServiceResult<T> Unauthorized(string error) =>
            new() { Success = false, Error = error, StatusCode = 401 };
    }
}
