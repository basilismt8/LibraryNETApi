namespace LibraryBlazor.Http
{
    public class ApiResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public Dictionary<string, string[]>? ValidationErrors { get; init; }

        public static ApiResult Ok() => new() { Success = true };

        public static ApiResult Fail(string? message = null,
            Dictionary<string, string[]>? errors = null)
            => new()
            {
                Success = false,
                Message = message,
                ValidationErrors = errors
            };
    }

    public sealed class ApiResult<T> : ApiResult
    {
        public T? Data { get; init; }

        public static ApiResult<T> Ok(T? data) => new() { Success = true, Data = data };

        public static ApiResult<T> Fail(string? message = null,
            Dictionary<string, string[]>? errors = null)
            => new()
            {
                Success = false,
                Message = message,
                ValidationErrors = errors
            };
    }
}
