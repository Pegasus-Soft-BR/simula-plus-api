using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MockExams.Api.Filters;
using Domain;
using Domain.Common;
using Domain.DTOs;
using MockExams.Infra.CrossCutting.Identity.Interfaces;
using MockExams.Service;
using MockExams.Service.Authorization;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MockExams.Api.Controllers;

[Route("api/[controller]")]
[GetClaimsFilter]
[EnableCors("AllowAllHeaders")]
public class UserManagementController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;
    private readonly IApplicationSignInManager _signManager;

    public UserManagementController(IUserService service, IMapper mapper, IApplicationSignInManager signManager)
    {
        _service = service;
        _mapper = mapper;
        _signManager = signManager;
    }

    [HttpGet("")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult Get([FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1, [FromQuery] string order = "CreatedAt desc", [FromQuery] string filter = "")
    {
        var result = _service.PagedList(itemsPerPage, page, order, filter);
        var resultDTO = _mapper.Map<PagedList<UserListDTO>>(result);
        return Ok(resultDTO);
    }

    [HttpGet("{id}")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult Get([FromRoute] Guid id)
    {
        var idAdmin = GetCurrentUserId();
        var user = _service.FindById(id, (Guid)idAdmin);
        var userDTO = _mapper.Map<UserDtoAdmin>(user);
        return Ok(userDTO);
    }

    [HttpPost("")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult Post([FromBody] UserDtoAdmin userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        var result = _service.Insert(user);
        var resultDTO = _mapper.Map<Result<UserDtoAdmin>>(result);
        return result.Success ? Ok(resultDTO) : BadRequest(resultDTO);
    }

    [HttpPut("")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult Put([FromBody] UserDtoAdmin userDTO)
    {
        var user = _mapper.Map<User>(userDTO);
        var result = _service.Update(user);
        var resultDTO = _mapper.Map<Result<UserDtoAdmin>>(result);
        return result.Success ? Ok(resultDTO) : BadRequest(resultDTO);
    }

    [HttpPut("ChangePassword")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult ChangePassword([FromBody] UserChangePasswordDTOAdmin changeDTO)
    {
        var user = _service.FindById(changeDTO.Id);
        if (user == null) return NotFound();

        var result = _service.ChangeUserPassword(user, changeDTO.Password);
        var resultDTO = _mapper.Map<Result<UserDtoAdmin>>(result);
        return result.Success ? Ok(resultDTO) : BadRequest(resultDTO);
    }

    [HttpDelete("{id}")]
    [Authorize("Bearer")]
    [AuthorizationFilter(Permissions.Permission.Admin)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var result = _service.Delete(id);
        var resultDTO = _mapper.Map<Result<UserDtoAdmin>>(result);
        return result.Success ? Ok(resultDTO) : BadRequest(resultDTO);
    }

    private Guid? GetCurrentUserId()
    {
        var guidStr = Thread.CurrentPrincipal?.Identity?.Name;
        if (string.IsNullOrEmpty(guidStr)) return null;
        else return new Guid(guidStr);
    }

}
