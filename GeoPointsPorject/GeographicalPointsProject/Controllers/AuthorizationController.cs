using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using GeographicalPointsProject.Controllers.Base;
using GeographicalPointsProject.Model;
using GP.Lib.Base.DataLayer;
using GP.Lib.Base.ViewModel;
using GP.Lib.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GP.Web.Api.Controllers
{
    [AllowAnonymous]
    public class AuthorizationController : BaseController
    {
        #region Fields
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly UserManager<DbUser> _userManager;
        private readonly ILogger _logger;
        private readonly IPasswordHasher<DbUser> _passwordHash;
        //private readonly IServiceUser _serviceUser;

        #endregion

        #region Ctor
        public AuthorizationController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<DbUser> signInManager,
            UserManager<DbUser> userManager,
            ILoggerFactory loggerFactory,
            ILogger<AuthorizationController> logger, IPasswordHasher<DbUser> passwordHasher/*,IServiceUser serviceUser*/) : base(logger)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<AuthorizationController>();
            _passwordHash = passwordHasher;
            //_serviceUser = serviceUser;
        }
        #endregion


        [HttpPost("~/connect/token"),
        Produces("application/json")]
        public async Task<IActionResult> Exchange(OpenIdConnectRequest request)
        {
            //Debug.Assert(request.IsTokenRequest(),
            //    "The OpenIddict binder for ASP.NET Core MVC is not registered. " +
            //    "Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");
            ResponseViewModel<BadRequestObjectResult> badRequestResponse = null;

            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {

                    badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                    {
                        IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                        IsValid = false,
                        Message = "The username/password couple is invalid.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return BadRequest(badRequestResponse);
                }


                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                    {
                        IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                        IsValid = false,
                        Message = "The username/password couple is invalid.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return BadRequest(badRequestResponse);
                }
                var results = await CreateTicketAsync(request, user);
                AuthenticationTicket ticket = (AuthenticationTicket)results.Result;
                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            if (request.IsRefreshTokenGrantType())
            {
                var info = await HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);

                var user = await _userManager.GetUserAsync(info.Principal);
                if (user == null)
                {
                    badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                    {
                        IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                        IsValid = false,
                        Message = "The refresh token is no longer valid.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Result = new BadRequestObjectResult(new OpenIdConnectResponse
                        {
                            Error = OpenIdConnectConstants.Errors.InvalidGrant,
                            ErrorDescription = "The refresh token is no longer valid."
                        })
                    };
                    return BadRequest(badRequestResponse);
                }

                // Ensure the user is still allowed to sign in.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                    {
                        IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                        IsValid = false,
                        Message = "The user is no longer allowed to sign in.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Result = new BadRequestObjectResult(new OpenIdConnectResponse
                        {
                            Error = OpenIdConnectConstants.Errors.InvalidGrant,
                            ErrorDescription = "The user is no longer allowed to sign in."
                        })
                    };
                    return BadRequest(badRequestResponse);
                }

                var results = await CreateTicketAsync(request, user, info.Properties);
                AuthenticationTicket ticket = (AuthenticationTicket)results.Result;
                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
            {
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                IsValid = false,
                Message = "The specified grant type is not supported.",
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Result = new BadRequestObjectResult(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                })
            };
            return BadRequest(badRequestResponse);
        }

        [HttpGet("~/connect/authorize"),
        Produces("application/json")]
        public async Task<IActionResult> Authorize(OpenIdConnectRequest request)
        {

            var info = await _signInManager.GetExternalLoginInfoAsync();
            var email = (string)request["email"];
            if (!string.IsNullOrEmpty(email))
            {
                var user = new DbUser { UserName = email, Email = email };
                var accountCreateResult = await _userManager.CreateAsync(user);
                if (accountCreateResult.Succeeded)
                {
                    _logger.LogInformation(6, $"User created an account using ${info.LoginProvider} provider.");
                    var results = await CreateTicketAsync(request, user);
                    AuthenticationTicket ticket = (AuthenticationTicket)results.Result;

                    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                }
                else
                {
                    return BadRequest("Email already exists");
                }
            }

            return BadRequest("Just Bad Request");
        }

        [HttpPost("~/connect/SignUp"),
        Produces("application/json")]
        public async Task<IActionResult> SignUp([FromBody]RegisterModel model)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var email = (string)model.Email;
            if (!string.IsNullOrEmpty(email))
            {
                var user = new DbUser { UserName = model.Username, Email = model.Email,  AccessFailedCount = 0, CreatedAt = DateTime.Now, CreatedBy = "User", FirstName = model.FirstName, LastName = model.Password, EmailConfirmed = true, PhoneNumberConfirmed = true };
                user.PasswordHash = _passwordHash.HashPassword(user, model.Password);
                var accountCreateResult = await _userManager.CreateAsync(user);
                if (accountCreateResult.Succeeded)
                {

                    var results = await CreateTicketAsync(new OpenIdConnectRequest()
                    {
                        ClientId = ConstantApp.ApiClientId,
                        ClientSecret = ConstantApp.ApiClientIdSecret,
                        Username = model.Username,
                        Password = model.Password,
                    }, user);
                    AuthenticationTicket ticket = (AuthenticationTicket)results.Result;

                    return Ok(new {  results = "Registration successful. Please login with the login api" });
                }
                else
                {

                    return BadRequest("Email already exists");
                }
            }

            return BadRequest("Just Bad Request");
        }


        private async Task<ResponseViewModel<object>> CreateTicketAsync(OpenIdConnectRequest request, DbUser user, AuthenticationProperties properties = null)
        {
            try
            {

                var principal = await _signInManager.CreateUserPrincipalAsync(user);


                var ticket = new AuthenticationTicket(principal, properties,
                    OpenIdConnectServerDefaults.AuthenticationScheme);

                if (!request.IsRefreshTokenGrantType())
                {

                    ticket.SetScopes(new[]
                    {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                }.Intersect(request.GetScopes()));
                }

                ticket.SetResources("resource_server");

                foreach (var claim in ticket.Principal.Claims)
                {

                    if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                    {
                        continue;
                    }

                    var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                    if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
                        (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                    {
                        destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                    }

                    claim.SetDestinations(destinations);
                }


                var okRequestResponse = new ResponseViewModel<object>
                {
                    IsAuthenticated = false,
                    IsValid = true,
                    Result = ticket
                };
                return okRequestResponse;
            }
            catch (Exception ex)
            {
                var badRequestResponse = new ResponseViewModel<object>
                {
                    IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                    IsValid = false,
                    Message = $"An error occurred creating ticket for {user.Email}",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return badRequestResponse;
                
            }
        }
    }
}