namespace ProductManagement.Core.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, T? data, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T data) => new(true, data);
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
        public static Result<T> Failure(List<string> errors) => new(false, default) { Errors = errors };
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true);
        public static Result Failure(string errorMessage) => new(false, errorMessage);
        public static Result Failure(List<string> errors) => new(false) { Errors = errors };
    }
}
