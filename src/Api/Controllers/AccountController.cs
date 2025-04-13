using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MockExams.Api.Filters;
using Domain;
using Domain.Common;
using Domain.DTOs;
using Domain.Exceptions;
using MockExams.Infra.CrossCutting.Identity;
using MockExams.Infra.CrossCutting.Identity.Interfaces;
using MockExams.Lgpd;
using MockExams.Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace MockExams.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAllHeaders")]
    [GetClaimsFilter]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IApplicationSignInManager _signManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILgpdService _lgpdService;

        public AccountController(IUserService userService,
                                 IApplicationSignInManager signManager,
                                 IMapper mapper,
                                 IConfiguration configuration,
                                 ILgpdService lgpdService)
        {
            _userService = userService;
            _signManager = signManager;
            _mapper = mapper;
            _configuration = configuration;
            _lgpdService = lgpdService;
        }

        #region GET

        [HttpGet]
        [Authorize("Bearer")]
        public UserListDTO Get()
        {
            var id = new Guid(Thread.CurrentPrincipal?.Identity?.Name);
            var user = _userService.FindById(id);

            var userVM = _mapper.Map<UserListDTO>(user);
            return userVM;
        }

        [Authorize("Bearer")]
        [HttpGet("Profile")]
        public object Profile()
        {
            var id = new Guid(Thread.CurrentPrincipal?.Identity?.Name);
            return new { profile = _userService.FindById(id).Profile.ToString() };
        }

        [Authorize("Bearer")]
        [HttpGet("WhoAccessed")]
        [ProducesResponseType(typeof(AccessHistoryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> WhoAccessedMyProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _lgpdService.GetWhoAccessedMyProfile((Guid)userId);
            var resultDto = _mapper.Map<IList<AccessHistoryDto>>(result);
            return Ok(resultDto);
        }

        [HttpGet("IsEmailAndPhoneVerified")]
        [Throttle(Name = "IsEmailAndPhoneVerified", Seconds = 1, VaryByIp = false)]
        public async Task<IActionResult> IsEmailAndPhoneVerified([FromQuery]string Email = "", [FromQuery] string Phone = "")
        {
            var result = await _userService.IsEmailAndPhoneVerified(Email, Phone);
            return Ok(result);
        }

        [HttpPost("SendEmailVerificationCode")]
        [Throttle(Name = "SendEmailVerificationCode", Seconds = 1, VaryByIp = false)]
        public async Task<IActionResult> SendEmailVerificationCode([FromBody] string Email = "")
        {
            await _userService.SendEmailVerificationCode(Email);
            return Ok();
        }

        [HttpPost("SendPhoneVerificationCode")]
        [Throttle(Name = "SendPhoneVerificationCode", Seconds = 1, VaryByIp = false)]
        public async Task<IActionResult> SendPhoneVerificationCode([FromBody] string Phone = "")
        {
            await _userService.SendPhoneVerificationCode(Phone);
            return Ok();
        }

        [HttpPost("EmailVerification")]
        [Throttle(Name = "EmailVerification", Seconds = 1, VaryByIp = false)]
        public async Task<IActionResult> EmailVerification(
            [FromServices] SigningConfigurations signingConfigurations,
            [FromServices] TokenConfigurations tokenConfigurations,
            [FromBody] EmailVerificationDto payload
        )
        {
            var user = await _userService.EmailVerification(payload.Email, payload.VerificationCode);
            var userDto = _mapper.Map<UserDto>(user);

            if (user.IsEmailVerified && user.IsPhoneVerified)
                return Ok(_signManager.GenerateTokenAndSetIdentity(user, signingConfigurations, tokenConfigurations));
            else
                return Ok(userDto);
        }

        [HttpPost("PhoneVerification")]
        [Throttle(Name = "PhoneVerification", Seconds = 1, VaryByIp = false)]
        public async Task<IActionResult> PhoneVerification(
            [FromServices] SigningConfigurations signingConfigurations,
            [FromServices] TokenConfigurations tokenConfigurations,
            [FromBody] PhoneVerificationDto payload
        )
        {
            var user = await _userService.PhoneVerification(payload.Phone, payload.VerificationCode);
            var userDto = _mapper.Map<UserDto>(user);

            if (user.IsEmailVerified && user.IsPhoneVerified)
                return Ok(_signManager.GenerateTokenAndSetIdentity(user, signingConfigurations, tokenConfigurations));
            else
                return Ok(userDto);
        }

        #endregion GET

        #region POST

        [HttpPost("Register")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(409)]
        public IActionResult Post([FromBody] UserRegisterDto registerUserDto, [FromServices] SigningConfigurations signingConfigurations, [FromServices] TokenConfigurations tokenConfigurations)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(registerUserDto);
            var result = _userService.Insert(user);

            if (result.Success)
            {
                var resulDto = _mapper.Map<Result<UserDto>>(result);
                return Ok(resulDto);
            }
                
            return Conflict(result);
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public IActionResult Login(
            [FromBody] UserLoginDTO loginUserVM,
            [FromServices] SigningConfigurations signingConfigurations,
            [FromServices] TokenConfigurations tokenConfigurations,
            [FromHeader(Name = "x-requested-with")] string client,
            [FromHeader(Name = "client-version")] string clientVersion)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // mensagem amigável para usuários mobile antigos
            if (!IsValidClientVersion(client, clientVersion))
                throw new BizException("Não é possível fazer login porque seu app está desatualizado. Por favor atualize seu app na loja do Google Play.");

            var user = _mapper.Map<User>(loginUserVM);
            var result = _userService.AuthenticationByEmailAndPassword(user);

            if (result.Success)
            {
                var response = new Result
                {
                    Value = _signManager.GenerateTokenAndSetIdentity(result.Value, signingConfigurations, tokenConfigurations)
                };

                return Ok(response);
            }

            return NotFound(result);
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginGoogle(
            [FromBody] string idToken, 
            [FromServices] SigningConfigurations signingConfigurations,
            [FromServices] TokenConfigurations tokenConfigurations)
        {

            var result = await _userService.AuthenticationByGoogleIdToken(idToken);

            if (result.Success)
            {
                var response = new Result
                {
                    Value = _signManager.GenerateTokenAndSetIdentity(result.Value, signingConfigurations, tokenConfigurations)
                };

                return Ok(response);
            }

            return NotFound(result);
        }


        [HttpPost("ForgotMyPassword")]
        [ProducesResponseType(typeof(Result), 200)]
        [ProducesResponseType(404)]
        public IActionResult ForgotMyPassword([FromBody] UserForgotMyPasswordDTO forgotMyPasswordVM)
        {
            var result = _userService.GenerateHashCodePasswordAndSendEmailToUser(forgotMyPasswordVM.Email);

            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost("Anonymize")]
        [Authorize("Bearer")]
        public IActionResult Anonymize([FromBody] UserAnonymizeDto dto)
        {
            var userIdFromSession = new Guid(Thread.CurrentPrincipal?.Identity?.Name);
            if (dto.UserId != userIdFromSession)
                throw new BizException(BizException.Error.Forbidden, "Você não tem permissão para remover esse conta.");

            _lgpdService.Anonymize(dto);
            return Ok(new Result("Sua conta foi removida com sucesso."));
        }

        #endregion POST

        #region PUT

        [HttpPut]
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(Result<User>), 200)]
        [ProducesResponseType(409)]
        public IActionResult Update([FromBody] UserDto updateUserVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(updateUserVM);

            user.Id = new Guid(Thread.CurrentPrincipal?.Identity?.Name);

            var result = _userService.Update(user);

            if (!result.Success)
                return Conflict(result);

            return Ok();
        }

        [Authorize("Bearer")]
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] UserChangePasswordDTO changePasswordUserVM)
        {
            var user = new User() { Password = changePasswordUserVM.OldPassword };
            user.Id = new Guid(Thread.CurrentPrincipal?.Identity?.Name);

            var result = _userService.ValidOldPasswordAndChangeUserPassword(user, changePasswordUserVM.NewPassword);
            if (result.Success) return Ok();
            else return BadRequest(result.Messages);
        }

        [HttpPut("ChangeUserPasswordByHashCode")]
        [ProducesResponseType(typeof(Result<User>), 200)]
        [ProducesResponseType(404)]
        public IActionResult ChangeUserPasswordByHashCode([FromBody] ChangeUserPasswordByHashCodeVM changeUserPasswordByHashCodeVM)
        {
            var result = _userService.ConfirmHashCodePassword(changeUserPasswordByHashCodeVM.HashCodePassword);
            if (!result.Success)
                return NotFound(result);
            var newPassword = changeUserPasswordByHashCodeVM.NewPassword;
            var user = _userService.FindById((result.Value as User).Id);
            user.Password = newPassword;

            var resultChangePasswordUser = _userService.ChangeUserPassword(user, newPassword);
            var resultChangePasswordUserDto = _mapper.Map<Result<UserDto>> (resultChangePasswordUser);

            if (!resultChangePasswordUser.Success)
                return BadRequest(resultChangePasswordUserDto);

            return Ok(resultChangePasswordUserDto);
        }

        #endregion PUT

        private bool IsValidClientVersion(string client, string clientVersion)
        {
            switch (client)
            {
                case "web":
                    return true;

                // mobile android
                case "com.makeztec.MockExams":
                    var minVersion = _configuration["ClientSettings:AndroidMinVersion"];
                    return Helper.ClientVersionValidation.IsValidVersion(clientVersion, minVersion);

                default:
                    return false;
            }                       
        }

        private bool IsAdmin(Guid userId)
        {
            var user = _userService.FindById(userId);
            return user.Profile == Domain.Enums.Profile.Admin;
        }

        private Guid? GetCurrentUserId()
        {
            var guidStr = Thread.CurrentPrincipal?.Identity?.Name;
            if(string.IsNullOrEmpty(guidStr)) return null;
            else return new Guid(guidStr);
        }
    }
}