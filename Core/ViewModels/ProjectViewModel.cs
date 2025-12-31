using Core.Models;

namespace Core.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? AmountNeeded { get; set; }
        public decimal? AmountObtained { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByPhoneNumber { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? SupportLink { get; set; }
        public string? QRCodeBase64 { get; set; }
        public string? OwnersProfilePic { get; set; }
        public DateTime? CreatedByDateJoined { get; set; }
        public virtual ICollection<ProjectSupporter>? ProjectSupporters { get; set; }
        public virtual ICollection<ProjectComment>? Comments { get; set; }
        public List<ProjectCustomFieldVM>? CustomFields { get; set; }
    }
    public class ProjectCustomFieldVM
    {
        public int? Id { get; set; }
        public string? FieldName { get; set; }
        public string? FieldType { get; set; }
        public bool IsRequired { get; set; }
    }
}