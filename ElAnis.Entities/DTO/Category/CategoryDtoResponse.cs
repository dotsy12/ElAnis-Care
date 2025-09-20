
namespace ElAnis.Entities.DTO.Category
{
	public class CategoryDtoResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string NameEn { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Icon { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}