using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Helper;
using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Model;

public class DbHelper {
    private EF_DataContext _context;
    
    private readonly IConfiguration _config;
    
    public DbHelper(EF_DataContext context, IConfiguration config) {
        _context = context;
        _config = config;
    }

    public async Task<AccountResponse> GetAccount(int id) {
        var account = _context.Accounts.Where(m => m.AccountID.Equals(id)).Single();
        return new AccountResponse {
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName
        };
    }

    public bool GetAccount(string email, string password) {
        var account = _context.Accounts.Where(m => m.Email.Equals(email)).Single();
        string decryptedPassword = EncryptDecrypt.DecryptString(account.Password, _config.GetValue<string>("Salt"));
        return password == decryptedPassword;
    }

    public Account GetAccount(string email) {
        var account = _context.Accounts.Where(m => m.Email.Equals(email)).Single();
        account.Password = EncryptDecrypt.DecryptString(account.Password, _config.GetValue<string>("Salt"));
        return account;
    }

    public async Task<Account> SaveAccount(AccountRequest request) {
        request.Password = EncryptDecrypt.EncryptString(request.Password, _config.GetValue<string>("Salt"));
        Account account = new Account {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            AccountCreated = DateTime.UtcNow,
            AccountUpdated = DateTime.UtcNow
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

     public async Task<Account> UpdateAccount(int id, AccountRequest request) {
        Account account = _context.Accounts.Where(m => m.AccountID.Equals(id)).First();
        if(account.AccountID == 0) {
            return new Account();
        } else {
            request.Password = EncryptDecrypt.EncryptString(request.Password, _config.GetValue<string>("Salt"));
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.AccountUpdated = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return account;
    }
}