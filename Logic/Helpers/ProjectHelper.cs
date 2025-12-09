using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using X.PagedList;
using X.PagedList.Extensions;

namespace Logic.Helpers
{
    public class ProjectHelper(AppDbContext context) : IProjectHelper
    {
        private readonly AppDbContext _context = context;


        public IPagedList<ProjectViewModel> Projects(IPageListModel<ProjectViewModel> model, int page)
        {
            try
            {
                var user = Utility.GetCurrentUser();
                var request = AppHttpContext.Current.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}";

                var query = _context.Projects
                    .Include(p => p.CreatedBy)
                    .Where(p => !p.Deleted);

                if (user.UserRole == Constants.UserRole)
                {
                    query = query.Where(p => p.CreatedById == user.Id);
                }

                if (!string.IsNullOrEmpty(model.Keyword))
                {
                    var key = model.Keyword.ToLower();

                    query = query.Where(p =>
                        p.Title.ToLower().Contains(key) ||
                        p.CreatedBy.FirstName.ToLower().Contains(key) ||
                        p.CreatedBy.LastName.ToLower().Contains(key) ||
                        p.AmountNeeded.ToString().Contains(key) ||
                        (p.AmountObtained ?? 0).ToString().Contains(key)
                    );
                }

                if (model.EndDate.HasValue)
                {
                    query = query.Where(p => p.DateCreated <= model.EndDate.Value);
                }

                model.CanFilterByDeliveryStatus = true;

                var projected = query.Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained ?? 0,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,

                    CreatedBy = c.CreatedBy.FirstName + " " + c.CreatedBy.LastName,

                    ProjectSupporters = null,

                    SupportLink = $"{baseUrl}/Guest/View/{c.Id}"
                });

                var paged = projected.ToPagedList(page, 25);

                var projectIds = paged.Select(x => x.Id).ToList();

                var supportersLookup = _context.ProjectSupporters
                    .Where(s => projectIds.Contains(s.ProjectId) && s.Amount > 0)
                    .GroupBy(s => s.ProjectId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in paged)
                {
                    item.ProjectSupporters = supportersLookup.ContainsKey(item.Id)
                        ? supportersLookup[item.Id]
                        : new List<ProjectSupporter>();
                }

                return paged;
            }
            catch
            {
                throw;
            }
        }



        public IQueryable<ProjectViewModel> GetAllProjects()
        {
            var user = Utility.GetCurrentUser();
            var request = AppHttpContext.Current.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSupporters)
                .Where(p => !p.Deleted);

            if (user.UserRole == Constants.UserRole)
            {
                query = query.Where(p => p.CreatedById == user.Id);
            }

            return query.Select(c => new ProjectViewModel
            {
                Id = c.Id,
                Name = c.Title,
                Description = c.Description,
                AmountNeeded = c.AmountNeeded,
                AmountObtained = c.AmountObtained ?? 0,
                Deleted = c.Deleted,
                DateCreated = c.DateCreated,
                CreatedById = c.CreatedById,
                CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : "",
                ProjectSupporters = c.ProjectSupporters.Where(x => x.Amount > 0).ToList(),
                SupportLink = $"{baseUrl}/Guest/View/{c.Id}"
            });
        }


        public async Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Where(p => p.CreatedById == userId && !p.Deleted)
                .Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,
                    CreatedBy = c.CreatedBy.FirstName + " " + c.CreatedBy.LastName,
                    ProjectSupporters = c.ProjectSupporters
                })
                .ToListAsync();
        }

        public bool CreateProject(ProjectViewModel project)
        {
            var createdById = Utility.GetCurrentUser().Id;
            var newProject = new Project
            {
                Title = project.Name,
                Description = project.Description,
                AmountNeeded = project.AmountNeeded,
                CreatedById = createdById
            };

            _context.Add(newProject);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateProject(ProjectViewModel project)
        {
            var projectVM = _context.Projects
                .FirstOrDefault(p => p.Id == project.Id && !p.Deleted);

            if (projectVM == null)
                return false;

            projectVM.Title = project.Name;
            projectVM.Description = project.Description;
            projectVM.AmountNeeded = project.AmountNeeded;

            _context.SaveChanges();
            return true;
        }

        public List<ProjectSupporter> GetContributors()
        {
            return [.. _context.ProjectSupporters.Where(ps => !ps.Deleted)];
        }

        public ProjectViewModel? GetProjectById(int id)
        {
            return _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSupporters)
                .Where(p => !p.Deleted && p.Id == id)
                .Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained ?? 0,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : "",
                    CreatedByDateJoined = c.CreatedBy != null ? c.CreatedBy.DateCreated : DateTime.MinValue,
                    CreatedByEmail = c.CreatedBy != null ? c.CreatedBy.Email : "",
                    CreatedByPhoneNumber = c.CreatedBy != null ? c.CreatedBy.PhoneNumber : "",
                    OwnersProfilePic = c.CreatedBy != null ? c.CreatedBy.ProfilePictureUrl : "",
                    Comments = c.Comments,
                    ProjectSupporters = c.ProjectSupporters.Where(x => x.Amount > 0).ToList()
                }).FirstOrDefault();
        }

        public ProjectPaymentDTO GetPaymentsByProjectId(int projectId)
        {
            var projectPaymentDTO = new ProjectPaymentDTO();
            var project = _context.Projects
               .Where(p => p.Id == projectId)
               .Include(p => p.Contributions)
                    .ThenInclude(p => p.ProjectSupporter)
               .FirstOrDefault();
            if (project == null)
                return projectPaymentDTO;
            var contributions = project.Contributions;
            projectPaymentDTO.ProjectName = project.Title;
            projectPaymentDTO.Payments = [.. contributions.Select(c => new PaymentDTO
            {
                Contributor = c.ProjectSupporter?.FullName,
                PaymentDate = c.Date,
                AmountPaid = c.Amount,
                Reference = c.PaystackReference
            })];
            return projectPaymentDTO;
        }

        public static byte[] GenerateQRCode(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return null;
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(5);

            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public bool AddComments(ProjectCommentsViewModel commentDetails)
        {
            var comments = new ProjectComment
            {
                Comment = commentDetails.Comment,
                FullName = commentDetails.FullName,
                ProjectId = commentDetails.ProjectId,
            };

            _context.Add(comments);
            _context.SaveChanges();
            return true;
        }
    }
}