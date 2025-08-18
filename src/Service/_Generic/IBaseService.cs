using Domain.Common;
using System;
using System.Threading.Tasks;

namespace MockExams.Service.Generic;

public interface IBaseService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    Task<PagedList<TDto>> PagedListAsync(int itemsPerPage = 10, int page = 1, string order = "CreatedAt Desc", string filter = "", string[] includes = null);
    Task<TEntity> FindByIdAsync(Guid id);
    Task<Result<TEntity>> InsertAsync(TEntity entity);
    Task<Result<TEntity>> DeleteAsync(Guid id);
    Task<Result<TEntity>> UpdateAsync(TDto dto);
}
