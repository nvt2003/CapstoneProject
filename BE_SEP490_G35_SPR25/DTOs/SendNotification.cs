namespace Planify_BackEnd.DTOs
{
    public class SendNotification
    {
        Guid UserId { get; set; }
        string Email { get; set; }
        string Message { get; set; }
    }
}
