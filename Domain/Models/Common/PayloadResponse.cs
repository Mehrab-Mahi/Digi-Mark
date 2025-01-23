namespace Domain.Models.Common
{
    public class PayloadResponse()
    {
        public bool IsSuccess { get; set; }
        public dynamic? Content { get; set; }
        public string TimeStamp { get; set; } = DateTime.Now.ToString("[dd/MM/yyyy#HHmmss]");
        public string? PayloadType { get; set; }
        public string? Message { get; set; }
    }
}
