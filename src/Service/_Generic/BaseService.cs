using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Exceptions;
using FluentValidation;
using Infra.Database;
using Infra.Database.UoW;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockExams.Service.Generic;

public class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
    where TEntity : BaseEntity
    where TDto : class
{
    protected readonly ApplicationDbContext _ctx;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IValidator<TEntity> _validator;
    protected readonly IMapper _mapper;
    protected readonly DbSet<TEntity> _entity;
    protected const int MaxItemsPerPage = 100;

    public BaseService(ApplicationDbContext context, IUnitOfWork unitOfWork, IValidator<TEntity> validator, IMapper mapper)
    {
        _ctx = context;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
        _entity = _ctx.Set<TEntity>();
    }

    public virtual async Task<PagedList<TDto>> PagedListAsync(
        int itemsPerPage = 10,
        int page = 1,
        string order = "CreatedAt desc",
        string filter = "",
        string[] includes = null)
    {
        try
        {
            if (itemsPerPage <= 0 || itemsPerPage > MaxItemsPerPage)
                throw new BizException(BizException.Error.BadRequest, $"itemsPerPage deve ser um valor entre 1 e {MaxItemsPerPage}.");

            var query = _entity.AsNoTracking().AsQueryable();

            if (includes != null)
            {
                foreach (var inc in includes)
                    query = query.Include(inc);
            }

            if (!string.IsNullOrWhiteSpace(filter))
                query = query.Where(filter);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(order)
                .Skip(itemsPerPage * (page - 1))
                .Take(itemsPerPage)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedList<TDto>
            {
                Items = items,
                ItemsPerPage = itemsPerPage,
                Page = page,
                TotalItems = total
            };
        }
        catch (Exception ex)
        {
            if (ex.Source == "System.Linq.Dynamic.Core")
                throw new BizException(BizException.Error.BadRequest, ex.Message);
            else
                throw;
        }
    }

    public virtual async Task<TEntity> FindByIdAsync(Guid id)
    {
        var entity = await _entity.FindAsync(id);
        if (entity == null)
            throw new BizException(BizException.Error.NotFound);
        return entity;
    }

    protected Result<TEntity> Validate(TEntity entity) => new Result<TEntity>(_validator.Validate(entity));

    protected Result<TEntity> Validate(TEntity entity, params Expression<Func<TEntity, object>>[] filter) =>
        new Result<TEntity>(_validator.Validate(entity, options => options.IncludeProperties(filter)));

    public virtual async Task<Result<TEntity>> InsertAsync(TEntity entity)
    {
        var result = Validate(entity);

        if (result.Success)
        {
            await _entity.AddAsync(entity);
            await _ctx.SaveChangesAsync();
            result.Value = entity;
            result.SuccessMessage = "Inclusão efetuada com sucesso.";
        }
        else
        {
            result.Value = null;
        }

        return result;
    }

    public virtual async Task<Result<TEntity>> DeleteAsync(Guid id)
    {
        var entity = await _entity.FindAsync(id);
        if (entity == null)
            throw new BizException(BizException.Error.NotFound);

        _entity.Remove(entity);
        await _ctx.SaveChangesAsync();

        var result = new Result<TEntity>(entity);
        result.SuccessMessage = "Exclusão efetuada com sucesso.";
        return result;
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TDto dto)
    {
        var idProperty = typeof(TDto).GetProperty("Id");
        if (idProperty == null)
            throw new BizException("O DTO precisa ter uma propriedade 'Id'.");

        var id = (Guid)idProperty.GetValue(dto)!;
        var existingEntity = await _entity.FindAsync(id);
        if (existingEntity == null)
            throw new BizException(BizException.Error.NotFound);

        _mapper.Map(dto, existingEntity);
        existingEntity.UpdatedAt = DateTime.UtcNow;

        var result = Validate(existingEntity);
        if (!result.Success)
        {
            result.Value = null;
            return result;
        }

        await _ctx.SaveChangesAsync();

        result.Value = existingEntity;
        result.SuccessMessage = "Alteração efetuada com sucesso.";
        return result;
    }
}
