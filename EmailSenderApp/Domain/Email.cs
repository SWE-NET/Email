namespace EmailSenderApp.Domain;

public class Email
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile> Attachment { get; set; }
}