using EcommerceApi.Data;
using EcommerceApi.Dto.StoreDto;
using EcommerceApi.Dto.UserDto;
using EcommerceApi.Entities;
using EcommerceApi.Exceptions;
using EcommerceApi.Services.Interface;
using EcommerceApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Data.Common;
using System.Xml.Linq;

namespace EcommerceApi.Services.Implementation
{
    public class StoreService : IStoreService
    {
        private readonly DataContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public StoreService(ILogger<UserService> logger,DataContext dbContext )
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<StoreResponseDto>> GetAllStoresAsync(int ownerId)
        {
            List<int> storeIds = GetStoreIdsByUserId(ownerId);
            return await _dbContext.Stores.Where(s => (s.OwnerId == ownerId || storeIds.Contains(s.Id)) &  !s.IsDeleted)
     .Select(s => new StoreResponseDto
     {
         Id = s.Id,
         Name = s.Name,
         Owner= new UserResponseDto { Id= s.OwnerId,Name=s.Owner.Username },
         Managers = s.Managers.Select(u => new UserResponseDto
         {
             Id = u.Id,
             Name = u.Username
         }).ToList()
     })
     .ToListAsync();
 
        }

        public async Task<StoreResponseDto> GetStoreByIdAsync(int storeId)
        {
            var result= await _dbContext.Stores.Where(s => s.Id == storeId & !s.IsDeleted)
     .Select(u => new StoreResponseDto
     {
         Id = u.Id,
         Name = u.Name,
         Owner = new UserResponseDto { Id = u.OwnerId, Name = u.Owner.Username },
         Managers = u.Managers.Select(s => new UserResponseDto
         {
             Id = s.Id,
             Name = s.Username
         }).ToList()
     })
     .FirstOrDefaultAsync();

            if (result == null)
            {
                return new StoreResponseDto();
            }
            return result;
        }
 
        public async Task<MessageResponse> AddStoreAsync(StoreCreateDto request)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents = new Dictionary<string, object>()
            }; try
            {
                var newStore = new Store
                {
                    Name = request.Name,
                    OwnerId = request.OwnerId,
                };
                _dbContext.Stores.Add(newStore);
                await _dbContext.SaveChangesAsync();
                result.IsSuccessful = true;
                result.Messages.Add("Successfully added store.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
            
            return result;
        }


        public async Task<Store> UpdateStoreAsync(int storeId,StoreUpdateDto request)
        {
            Store store = _dbContext.Stores.Where(s=>s.Id==storeId & !s.IsDeleted).FirstOrDefault();
            if (store == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound,"Store not exists");
            }
            store.Name = request.Name;
            _dbContext.Entry(store).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return store;
        }

        public async Task DeleteStoreAsync(int storeId)
        {
            var store = await _dbContext.Stores.FindAsync(storeId);
            if (store != null)
            {
                store.IsDeleted = true;
                _dbContext.Entry(store).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Not found");
            }
        }

        public List<int> GetStoreIdsByUserId(int userId)
        {
            var storeIds = new List<int>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"SELECT \"StoreId\" FROM \"StoreManager\" WHERE \"UserId\" =  @userId";
                var userIdParam = command.CreateParameter();
                userIdParam.ParameterName = "@userId";
                userIdParam.Value = userId;
                command.Parameters.Add(userIdParam);
                _dbContext.Database.OpenConnection();
              
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        storeIds.Add(result.GetInt32(0));
                    }
                }
            }

            return storeIds;
        }

        public async Task<MessageResponse> AddManager(int storeId, int userId)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents = new Dictionary<string, object>()
            };
            try
            {
                int rowsAffected;
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "INSERT INTO \"StoreManager\" VALUES  ( @userId,@storeId)";

                    var userIdParam = command.CreateParameter();
                    userIdParam.ParameterName = "@userId";
                    userIdParam.Value = userId;
                    command.Parameters.Add(userIdParam);
                    var storeIdParam = command.CreateParameter();
                    storeIdParam.ParameterName = "@storeId";
                    storeIdParam.Value = storeId;
                    command.Parameters.Add(storeIdParam);

                    _dbContext.Database.OpenConnection();
                    rowsAffected = command.ExecuteNonQuery();
                    result.IsSuccessful = true;
                    result.Messages.Add("Successfully Added.");
                }

            }
            catch (DbUpdateException dbEx)
            {

                _logger.LogError(dbEx, "An error occurred while saving changes to the database.");
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An error occurred while creating the user. Please try again later.");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An unexpected error occurred.");
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
            return result;

           
        }

        public async Task<MessageResponse> Revork(int storeId, int userId)
        {
            var result = new MessageResponse()
            {
                IsSuccessful = false,
                Messages = new List<string>(),
                Contents = new Dictionary<string, object>()
            };
            try
            {
                int rowsAffected;
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "DELETE FROM \"StoreManager\" WHERE  \"StoreId\" = @storeId AND \"UserId\" = @userId";

                    var userIdParam = command.CreateParameter();
                    userIdParam.ParameterName = "@userId";
                    userIdParam.Value = userId;
                    command.Parameters.Add(userIdParam);
                    var storeIdParam = command.CreateParameter();
                    storeIdParam.ParameterName = "@storeId";
                    storeIdParam.Value = storeId;
                    command.Parameters.Add(storeIdParam);

                    _dbContext.Database.OpenConnection();
                    rowsAffected = command.ExecuteNonQuery();
                }
                result.IsSuccessful = true;
                result.Messages.Add("Successfully Revorked.");
            }
            catch (DbUpdateException dbEx)
            {
              
                _logger.LogError(dbEx, "An error occurred while saving changes to the database.");
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An error occurred while creating the user. Please try again later.");
            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex, "An unexpected error occurred.");
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
            return result;
           
           
        }
    }
    class StoreIdDto
    {
        public int StoreId { get; set; }
    }
}
