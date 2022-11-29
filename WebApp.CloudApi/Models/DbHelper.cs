using WebApp.CloudApi.EfCore;
using WebApp.CloudApi.Helper;
using WebApp.CloudApi.RequestModel;
using WebApp.CloudApi.ResponseModel;

namespace WebApp.CloudApi.Model;

public class DbHelper :IDbHelper
{
    private EF_DataContext _context;
    string key = "E546C8DF278CD5931069B522E695D4F2";
    private readonly ApplicationInstance _application;

    private readonly IConfiguration _config;

    public DbHelper(EF_DataContext context, IConfiguration config,
        ApplicationInstance application)
    {
        _context = context;
        _config = config;
        _application = application;
    }

    public AccountResponse? GetAccount(Guid id)
    {
        var account = _context.Accounts.Where(m => m.AccountID.Equals(id)
        && m.AccountID.Equals(_application.Application)).Single();
        if (!account.Verified) {
            return null;
        }
        return new AccountResponse
        {
            AccountId = account.AccountID,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            AccountCreated = account.AccountCreated,
            AccountUpdated = account.AccountUpdated
        };
    }

    public bool GetAccount(string email, string password)
    {
        var account = _context.Accounts.Where(m => m.Email.Equals(email)).Single();
        _application.Application = account.AccountID;
        string decryptedPassword = EncryptDecrypt.DecryptString(account.Password, key);
        return password == decryptedPassword;
    }

    public Account GetAccount(string email)
    {
        if (_context.Accounts.ToList().Count == 0)
        {
            return new Account();
        }
        var account = _context.Accounts.Where(m => m.Email.Equals(email)).First();
        account.Password = EncryptDecrypt.DecryptString(account.Password, key);
        return account;
    }

    public bool VerifyAccount(string email)
    {
        if (_context.Accounts.ToList().Count == 0)
        {
            return false;
        }
        var account = _context.Accounts.Where(m => m.Email.Equals(email)).First();
        if (account.Verified) {
            return false;
        }
        account.Verified = true;
        _context.SaveChanges();
        return true;
    }

    public async Task<AccountResponse> SaveAccount(AccountRequest request)
    {
        request.Password = EncryptDecrypt.EncryptString(request.Password, key);
        Account account = new Account
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            AccountCreated = DateTime.UtcNow,
            AccountUpdated = DateTime.UtcNow
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return new AccountResponse
        {
            AccountId = account.AccountID,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            AccountCreated = account.AccountCreated,
            AccountUpdated = account.AccountUpdated
        };
    }

    public async Task<AccountResponse> UpdateAccount(Guid id, AccountRequest request)
    {
        Account account = _context.Accounts.Where(m => m.AccountID.Equals(id)).First();
        if (account.AccountID == new Guid())
        {
            return new AccountResponse();
        }
        else
        {
            account.Password = EncryptDecrypt.EncryptString(request.Password, key);
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.AccountUpdated = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return new AccountResponse
        {
            AccountId = account.AccountID,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            AccountCreated = account.AccountCreated,
            AccountUpdated = account.AccountUpdated
        };
    }

    public async Task<DocumentRequest> SaveDocument(DocumentRequest request)
    {
        Document document = new Document
        {
            BucketPath = request.BucketPath,
            DocumentCreated = DateTime.UtcNow,
            Name = request.Name,
            AccountID = _application.Application
        };
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
        return new DocumentRequest
        {
            DocumentID = request.DocumentID,
            BucketPath = request.BucketPath,
            DocumentCreated = DateTime.UtcNow,
            Name = request.Name
        };
    }

    public async Task<bool> DeleteDocument(string key) {
        var doc = _context.Documents.Where(m => m.Name == key).FirstOrDefault();
        _context.Documents.Remove(doc);
        return true;
    }

}