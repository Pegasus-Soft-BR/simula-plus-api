using Domain.Common;
using System;
using System.Linq.Expressions;

namespace MockExams.Service.Generic
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        PagedList<TEntity> PagedList(int itemsPerPage = 10, int page = 1, string order = "CreatedAt Desc", string filter = "");

        TEntity FindById(Guid Id);

        Result<TEntity> Insert(TEntity entity);

        Result<TEntity> Delete(Guid Id);

        Result<TEntity> Update(TEntity entity);

    }
}
