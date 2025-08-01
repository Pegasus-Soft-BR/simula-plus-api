using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MockExams.Api.Filters;
using Domain.Common;
using MockExams.Service.Authorization;
using MockExams.Service.Generic;
using System;

namespace MockExams.Api.Controllers;

[EnableCors("AllowAllHeaders")]

// TODO: colocar um DTO genérico.
// aí poderia ser reproveitado pelos controllers mais complexos como TICKET e USER.
public class BaseController<T> : ControllerBase where T : BaseEntity
{
    protected IBaseService<T> _service;

    public BaseController(IBaseService<T> service)
    {
        _service = service;
    }

    [HttpGet("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public IActionResult Get([FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1, [FromQuery] string order = "CreatedAt desc", [FromQuery] string filter = "")
    {
        var items = _service.PagedList(itemsPerPage, page, order, filter);
        return Ok(items);
    }

    [HttpGet("{id}")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public IActionResult Get([FromRoute] Guid id)
    {
        var item = _service.FindById(id);
        return Ok(item);
    }

    [HttpPost("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public IActionResult Post([FromBody] T entity)
    {
        var result = _service.Insert(entity);
        if (result.Success)
            return Ok(result);
        else return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var result = _service.Delete(id);
        return Ok(result);
    }

    [HttpPut("")]
    [Authorize("Bearer")]
    [AppAuthorizationFilter("admin")]
    public IActionResult Put([FromBody] T entity)
    {
        var result = _service.Update(entity);
        if (result.Success)
            return Ok(result);
        else return BadRequest(result);
    }
}