using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateProjectAsync(Project entity);

        // (Tùy chọn) Nếu cần lấy thông tin Project sau này
        Task<Project?> GetByIdAsync(Guid id);
    }
}
