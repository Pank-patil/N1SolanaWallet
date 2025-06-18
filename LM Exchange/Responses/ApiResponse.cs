namespace LM_Exchange.Models.Responses
{
    public class ApiResponse<T>
    {
        public string status { get; set; } = "true";
        public T data { get; set; }
    }

    public class ApiErrorResponse
    {
        public string status { get; set; } = "false";
        public ApiError error { get; set; }
    }

    public class ApiError
    {
        public string message { get; set; }
        public string code { get; set; }
    }
}
