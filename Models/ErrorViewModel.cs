namespace CompanyPortal.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public String ContactError { get; set; }

        public String ErrorMsg {  get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
