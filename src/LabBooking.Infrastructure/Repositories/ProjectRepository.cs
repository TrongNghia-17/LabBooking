using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class ProjectRepository(LabBookingDbContext dbContext) : IProjectRepository
    {

        public async Task<Project> CreateProjectAsync(Project entity)
        {
            dbContext.Projects.Add(entity);
            //await dbContext.SaveChangesAsync();
            return entity;
        }

        // (Tùy chọn) Nếu cần lấy thông tin Project sau này
        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await dbContext.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
