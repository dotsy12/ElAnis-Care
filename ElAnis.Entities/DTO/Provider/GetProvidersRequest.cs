namespace ElAnis.Entities.DTO.Provider
{
    public class GetProvidersRequest
    {
        public bool? Available { get; set; }
        public string? Governorate { get; set; }
        public string? City { get; set; }
        public Guid? CategoryId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}