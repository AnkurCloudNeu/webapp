using CloudApi.EfCore;
using CloudApi.RequestModel;

namespace CloudApi.Model;

public class DbHelper {
    private EF_DataContext _context;
    
    public DbHelper(EF_DataContext context) {
        _context = context;
    }

    public async Task<Account> GetAccount(int id) {
        return _context.Accounts.Where(m => m.AccountID.Equals(id)).Single();
    }

    public Account GetAccount(string email, string password) {
        return _context.Accounts.Where(m => m.Email.Equals(email) && m.Password.Equals(password)).Single();
    }

    public Account GetAccount(string email) {
        return _context.Accounts.Where(m => m.Email.Equals(email)).Single();
    }

    public async Task<Account> SaveAccount(AccountRequest request) {
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
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.AccountUpdated = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return account;
    }
}