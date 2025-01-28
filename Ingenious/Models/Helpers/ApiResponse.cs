namespace Ingenious.Models.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        // Constructor for success response
        public ApiResponse(T data, string message = "Operation successful", int statusCode = 200)
        {
            Success = true;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        // Constructor for error response
        public ApiResponse(string message, int statusCode = 400)
        {
            Success = false;
            Data = default;
            Message = message;
            StatusCode = statusCode;
        }
    }
}
