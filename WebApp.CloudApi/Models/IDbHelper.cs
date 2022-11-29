using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Model
{
    public interface IDbHelper
    {
        public AccountResponse? GetAccount(Guid id);
        public bool GetAccount(string email, string password);
        public Account GetAccount(string email);
        public Task<AccountResponse> SaveAccount(AccountRequest request);
        public Task<AccountResponse> UpdateAccount(Guid id, AccountRequest request);
        public Task<DocumentRequest> SaveDocument(DocumentRequest request);
        public Task<bool> DeleteDocument(string key);
        public Account VerifyAccount(string email);
    }
}