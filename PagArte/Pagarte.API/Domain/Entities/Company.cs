namespace Pagarte.API.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;     // encrypted
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Service> Services { get; set; } = [];
    }
}
