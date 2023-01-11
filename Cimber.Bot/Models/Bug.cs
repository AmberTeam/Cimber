namespace Cimber.Bot.Models
{
    public class Bug
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public Type Type { get; set; }
        public string? Path { get; set; }
        public long? FromUser { get; set; }
    }
}
