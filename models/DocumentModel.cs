namespace DocumentsApi.Models
{
    public class DocumentItem
    {
        public long Id { get; set; }
        public string UserId { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentType { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
