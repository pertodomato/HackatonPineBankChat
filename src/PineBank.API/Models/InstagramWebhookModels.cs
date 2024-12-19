// PineBank/src/PineBank.API/Models/InstagramWebhookModels.cs
namespace PineBank.API.Models
{
    public class InstagramWebhookRequest
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        public string Id { get; set; }
        public string Time { get; set; }
        public List<Messaging> Messaging { get; set; }
    }

    public class Messaging
    {
        public Sender Sender { get; set; }
        public Recipient Recipient { get; set; }
        public Message Message { get; set; }
    }

    public class Sender
    {
        public string Id { get; set; }
    }

    public class Recipient
    {
        public string Id { get; set; }
    }

    public class Message
    {
        public string Mid { get; set; }
        public string Text { get; set; }
        public Attachments Attachments { get; set; }
    }

    public class Attachments
    {
        public string Type { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public string Url { get; set; }
    }
}
