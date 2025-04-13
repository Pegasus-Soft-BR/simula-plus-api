using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;
using Domain.DTOs;
using MockExams.Service;

namespace MockExams.Api.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowAllHeaders")]
public class ContactUsController : ControllerBase
{
    private readonly IContactUsService _contactUsService;
    private readonly IMapper _mapper;

    public ContactUsController(IContactUsService contactUsService,
                               IMapper mapper)
    {
        _contactUsService = contactUsService;
        _mapper = mapper;
    }

    [HttpPost("SendMessage")]
    public Result<ContactUs> SendMessage([FromBody]ContactUsDTO contactUsVM)
    {
        var contactUS = _mapper.Map<ContactUs>(contactUsVM);

        return _contactUsService.SendContactUs(contactUS);
    }
}