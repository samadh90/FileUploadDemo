namespace FileUploadDemo.Models;

public class FileModel
{
    public Guid ID { get; set; }
    public string MimeType { get; set; }
    public byte[] Data { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set;}
}
