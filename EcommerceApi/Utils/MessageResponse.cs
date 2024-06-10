namespace EcommerceApi.Utils
{
    public class MessageResponse
    {
        public int Id { get; set; }

        public bool IsSuccessful { get; set; }

        public List<string>? Messages { get; set; }
        public Dictionary<string,object>? Contents { get; set; }
    }
}
