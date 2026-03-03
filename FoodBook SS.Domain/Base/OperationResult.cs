
namespace FoodBook_SS.Domain.Base
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
        }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static OperationResult Ok(string message = "") =>
            new OperationResult { Success = true, Message = message };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }
}
