namespace Planify_BackEnd.DTOs
{
    public class ResponseDTO
    {       
        public int Status { get; set; }
        public string Message { get; set; } = "";
        public DateTime DateTime { get; set; } = DateTime.Now;
        public object? Result { get; set; }

        public ResponseDTO(int status, string message, object? result)
        {
            Status = status;
            Message = message;
            Result = result;
        }
    }
}
