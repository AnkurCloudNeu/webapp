using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Interface;

public interface IUserCreator
{
    Task<bool> CreateUser(AccountResponse user);
    Task<AccountResponse[]> GetUserAsync(string email);
    Task<bool> DeleteUserAsync(string email);
}