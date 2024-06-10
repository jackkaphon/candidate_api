using AutoMapper;
using EcommerceApi.Data;
using EcommerceApi.Dto.CartDto;
using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;
using EcommerceApi.Exceptions;
using EcommerceApi.Services.Interface;
using EcommerceApi.Utils;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EcommerceApi.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        public UserService(IConfiguration configuration, ILogger<UserService> logger,IHttpContextAccessor httpContextAccessor, DataContext context, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbContext = context;
            _mapper = mapper;
            _logger= logger;
        }

        public UserInfoDto GetUserInfo()
        {
            UserInfoDto userInfoDto = new UserInfoDto();
          
            if (_httpContextAccessor.HttpContext is not null)
            {
                userInfoDto.UserId = _httpContextAccessor.HttpContext.User.FindFirstValue(CustomClaimTypes.UserId); 
                userInfoDto.ManagerOfStores = _httpContextAccessor.HttpContext.User.FindAll(CustomClaimTypes.ManagerOfStores).ToList();
                userInfoDto.OwnerOfStores = _httpContextAccessor.HttpContext.User.FindAll(CustomClaimTypes.StoresOwner).ToList();
            }
            else
            {
                throw new HttpResponseException(StatusCodes.Status401Unauthorized, "Not found user info");
            }
            return userInfoDto;
        }

        public async Task<MessageResponse> CreateUser(UserCreateDto request)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents = new Dictionary<string, object>()
            };
            var entity = _mapper.Map<UserCreateDto, User>(request);

            var entityDb = await _dbContext.Users.Where(o => o.Username == request.Username && !o.IsDeleted).FirstOrDefaultAsync();

            if (entityDb != null)
            {
                throw new HttpResponseException(StatusCodes.Status409Conflict, "Username already exists.");
            }
            else
            {
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                using (var dbTransaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        _dbContext.Add(entity);                       
                        await _dbContext.SaveChangesAsync();
                        _dbContext.Add(new Cart { UserId = entity.Id });
                        await _dbContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();

                        result.Id = entity.Id;
                        result.IsSuccessful = true;
                        result.Contents.Add("token", CreateToken(entity));
                        result.Messages.Add("Successfully created.");
                    }
                    catch (DbUpdateException dbEx)
                    {
                        await dbTransaction.RollbackAsync();
                        _logger.LogError(dbEx, "An error occurred while saving changes to the database.");
                        throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An error occurred while creating the user. Please try again later.");
                    }
                    catch (Exception ex)
                    {
                        await dbTransaction.RollbackAsync();
                        _logger.LogError(ex, "An unexpected error occurred.");
                        throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
                    }
                    return result;
                }
            }



        }
        public async Task<MessageResponse> Login(UserLoginDto request)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents= new Dictionary<string, object>()
            };

            var entityDb = await _dbContext.Users                
                .Where(o => o.Username == request.Username && !o.IsDeleted)
                .FirstOrDefaultAsync();

            if (entityDb != null)
            {
                result.IsSuccessful = true;
                if (BCrypt.Net.BCrypt.Verify(request.Password, entityDb.PasswordHash))
                {

                     var userInto= await _dbContext.Users.Where(u => u.Id == entityDb.Id & !u.IsDeleted)
     .Select(u => new UserStoreResponseDto
     {
         Id = u.Id,
         Username = u.Username,
         ManagerOfStores = u.Stores.Select(s => new Store
         {
             Id = s.Id,
             Name = s.Name
         }).ToList()
     })
     .FirstOrDefaultAsync();

                    userInto.StoresOwner = await _dbContext.Stores.Where(s => !s.IsDeleted & s.OwnerId == entityDb.Id).ToListAsync();

                    result.IsSuccessful = true;
                    result.Contents["token"] = CreateToken(userInto);
                    result.Messages.Add("Successfully logged in.");
                }
                else
                {
                    _logger.LogWarning("Invalid username or password.");
                    throw new HttpResponseException(StatusCodes.Status401Unauthorized, "Invalid username or password.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid username or password.");
                throw new HttpResponseException(StatusCodes.Status401Unauthorized, "Invalid username or password.");
            }

            return result;
        }



        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
            };

            if (user.Roles != null)
            {
                foreach (string role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

            }    

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string CreateToken(UserStoreResponseDto user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
            };

            if (user.Roles != null)
            {
                foreach (string role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

            }

            if (user.StoresOwner != null)
            {
                foreach (var store in user.StoresOwner)
                {
                    claims.Add(new Claim(CustomClaimTypes.StoresOwner, store.Id.ToString()));
                }

            }
            if (user.ManagerOfStores != null)
            {
                foreach (var store in user.ManagerOfStores)
                {
                    claims.Add(new Claim(CustomClaimTypes.ManagerOfStores, store.Id.ToString()));
                }

            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public Task<UserCartResponseDto> GetMyCart(int userId)
        {
            throw new NotImplementedException();
        }
    }
}