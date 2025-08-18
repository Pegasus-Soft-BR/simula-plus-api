using AutoMapper;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockExams.Api.Filters;
using MockExams.Service.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[EnableCors("AllowAllHeaders")]
public class BaseCrudController<T, TDto> : ControllerBase
    where T : BaseEntity
    where TDto : class
{
    protected readonly IBaseService<T, TDto> _service;
    protected readonly IMapper _mapper;
    protected readonly ILogger<T> _logger;

    public BaseCrudController(IBaseService<T, TDto> service, IMapper mapper, ILogger<T> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public virtual async Task<IActionResult> Get(
        [FromQuery] int itemsPerPage = 10,
        [FromQuery] int page = 1,
        [FromQuery] string order = "CreatedAt desc",
        [FromQuery] string filter = "")
    {
        var paged = await _service.PagedListAsync(itemsPerPage, page, order, filter);
        var dtoList = paged.Items.Select(_mapper.Map<TDto>).ToList();

        var result = new PagedList<TDto>
        {
            Page = paged.Page,
            ItemsPerPage = paged.ItemsPerPage,
            TotalItems = paged.TotalItems,
            Items = dtoList
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public virtual async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var item = await _service.FindByIdAsync(id);
            var dto = _mapper.Map<TDto>(item);
            return Ok(new Result<TDto>(dto));
        }
        catch (Exception ex)
        {
            return NotFound(new Result(ex.Message));
        }
    }

    [HttpPost("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public virtual async Task<IActionResult> Post([FromBody] TDto dto)
    {
        var entity = _mapper.Map<T>(dto);
        var result = await _service.InsertAsync(entity);
        var resultDto = _mapper.Map<Result<TDto>>(result);

        return resultDto.Success
            ? Ok(resultDto)
            : BadRequest(resultDto);
    }

    [HttpPut("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public virtual async Task<IActionResult> Put([FromBody] TDto dto)
    {
        var result = await _service.UpdateAsync(dto);
        var resultDto = _mapper.Map<Result<TDto>>(result);

        return resultDto.Success
            ? Ok(resultDto)
            : BadRequest(resultDto);
    }

    [HttpDelete("{id}")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public virtual async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _service.DeleteAsync(id);
        var resultDto = _mapper.Map<Result<TDto>>(result);

        return resultDto.Success
            ? Ok(resultDto)
            : BadRequest(resultDto);
    }
}
