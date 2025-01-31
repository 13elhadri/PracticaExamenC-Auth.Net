using examn.Funkos.Dto;
using examn.Funkos.Models;
using examn.User.Dto;

namespace examn.Funkos.Services;

public interface IFunkoService
{
    public Task<IEnumerable<Funko>> GetAllAsync();
    
    public Task<Funko?> GetByIdAsync(long id);
    
    public Task<Funko> CreateAsync(FunkoRequest funkoRequest);
    
    public Task<Funko?> DeleteByGuidAsync(long id);

    public Task<string> SaveFileAsync(IFormFile file);

    public Task<FileStream> GetFileAsync(string fileName);

    public Task<string> UpdateFunkoFotoAsync(long id, IFormFile file);
}