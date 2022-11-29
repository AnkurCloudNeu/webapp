using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Interface;

public interface IUserCreator
{
    Task<bool> CreateUser(AccountRequest user);
    Task<AccountResponse[]> GetUserAsync(string email);
    Task<bool> DeleteUserAsync(string email);
}