using MRS.Infrastructure.Data.Common;

namespace MRS.Infrastructure.Data.Repositories;

public class ApplicatioDbRepository : Repository, IApplicatioDbRepository
{
    public ApplicatioDbRepository(ApplicationDbContext context)
    {
        this.Context = context;
    }
}