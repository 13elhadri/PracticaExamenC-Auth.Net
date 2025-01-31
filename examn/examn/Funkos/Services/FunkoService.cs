using examn.Categoria.Mappers;
using examn.Database;
using examn.Funkos.Dto;
using examn.Funkos.Mapper;
using examn.Funkos.Models;
using examn.Funkos.Storage.Config;
using examn.User.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace examn.Funkos.Services;

public class FunkoService:IFunkoService
{
    private readonly GeneralDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeyPrefix = "Funko:";
    private readonly FileStorageConfig _fileStorageConfig;
    
    public FunkoService(
        GeneralDbContext context,
        ILogger<UserService> logger,
        IMemoryCache memoryCache,
        FileStorageConfig fileStorageConfig
        )
    {
        _context = context;
        _logger = logger;
        _memoryCache = memoryCache;
        _fileStorageConfig = fileStorageConfig;
    }

    public async Task<IEnumerable<Funko>> GetAllAsync()
    {
        _logger.LogInformation("Buscondo todos los funkos desde la base de datos");
        
        var funkos = await _context.Funkos
            .Include(f=>f.Categoria)
            .Where(u=> u.IsDeleted == false )
            .ToListAsync();

        return funkos.Select(u => u.ToModelFromEntity());
    }

    public async Task<Funko?> GetByIdAsync(long id)
    {
        _logger.LogInformation($"Buscando funko con id: {id}");

        var cacheKey = CacheKeyPrefix + id;
        
        if (_memoryCache.TryGetValue(cacheKey, out Funko? funkoCache))
        {
            _logger.LogInformation("Funko obtenido desde la memoria caché");
            return funkoCache!;
        }
        
        
        var funkoEntity = await _context.Funkos            
            .Include(f=>f.Categoria)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (funkoEntity != null)
        {
            _logger.LogInformation($"Funko encontrado con id: {id}");

            // Mapear entidad a modelo y respuesta
            var funkoModel = funkoEntity.ToModelFromEntity();

            // Guardar en la memoria caché
            _memoryCache.Set(cacheKey, funkoModel, TimeSpan.FromMinutes(30));
            
            return funkoModel;
        }

        _logger.LogInformation($"Funko no encontrado con id: {id}");
        return null;
    }

    public async Task<Funko> CreateAsync(FunkoRequest funkoRequest)
    {
        _logger.LogInformation("Creando Funko");

        var categoria = _context.Categorias.FirstOrDefault(u => u.Nombre.ToLower() == funkoRequest.Categoria.ToLower());

        if (categoria != null)
        {
            _logger.LogWarning($"Ya existe una categoria con el nombre: {funkoRequest.Categoria} (en base de datos)");
            throw new Exception($"Ya existe una categoria con el nombre: {funkoRequest.Categoria}");
        }

        var funko = new Funko
        {
            Nombre = funkoRequest.Nombre,
            Categoria = categoria!.ToModelFromEntity()
        };
        
        var funkoEntity = funko.ToEntityFromModel();
        _context.Funkos.Add(funkoEntity);
        await _context.SaveChangesAsync();

        var cacheKey = CacheKeyPrefix + funko.Id;

        _memoryCache.Set(cacheKey, funko, TimeSpan.FromMinutes(30));

        _logger.LogInformation("Funko creado con éxito");
        return funko; 
    }
    
    public async Task<Funko?> DeleteByGuidAsync(long id)
    {
        _logger.LogInformation($"Borrando Funko con id: {id}");

        var funkoEntity = await _context.Funkos
            .Include(f=>f.Categoria)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (funkoEntity == null)
        {
            _logger.LogWarning($"Funko no encontrado con id: {id}");
            return null;
        }

        funkoEntity.IsDeleted = true;
        funkoEntity.UpdatedAt = DateTime.UtcNow;
        _context.Funkos.Update(funkoEntity);
            
        _logger.LogInformation("Guardando todos los cambios en la base de datos");
        await _context.SaveChangesAsync();

        var cacheKey = CacheKeyPrefix + funkoEntity.Id;

        // Eliminar de la cache en memoria
        _memoryCache.Remove(cacheKey);
            
        _logger.LogInformation($"Funko borrado (desactivado) con id: {id}");
        return funkoEntity.ToModelFromEntity();
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        // Generar un nombre base único para el archivo
        var baseFileName = "prueba" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        _logger.LogInformation($"Saving file with base name: {baseFileName}");

        // Verificar si el tamaño del archivo es válido
        if (file.Length > _fileStorageConfig.MaxFileSize)
        {
            throw new Exception("El tamaño del fichero excede del máximo permitido");
        }

        // Obtener la extensión del archivo y verificar si está permitida
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!_fileStorageConfig.AllowedFileTypes.Contains(fileExtension))
        {
            throw new Exception("Tipo de fichero no permitido");
        }

        // Configurar la ruta donde se guardará el archivo
        var uploadPath = Path.Combine(_fileStorageConfig.UploadDirectory);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath); // Crear la carpeta si no existe
        }

        // Crear el nombre completo del archivo
        var fullFileName = $"{baseFileName}{fileExtension}";
        var filePath = Path.Combine(uploadPath, fullFileName);

        // Guardar el archivo en la carpeta
        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream); // Copiar el archivo al destino
        }

        _logger.LogInformation($"File saved successfully: {fullFileName}");

        // Retornar la ruta completa donde se guardó el archivo
        return fullFileName;
    }


    public async Task<string> UpdateFunkoFotoAsync(long id, IFormFile file)
    {
        _logger.LogInformation($"Updating profile photo for funko with ID: {id}");

        // Verificar si el archivo es válido
        if (file == null || file.Length == 0)
        {
            throw new FileNotFoundException("No file was provided or the file is empty.");
        }

        // Buscar el Funko en la base de datos
        var funkoEntity = await _context.Funkos
            .Include(f => f.Categoria)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (funkoEntity == null)
        {
            throw new Exception($"Funko with ID {id} not found.");
        }

        // Guardar el nuevo archivo (imagen)
        var newFileName = await SaveFileAsync(file); // Guardar el archivo y obtener el nombre

        // Actualizar el Funko con la nueva imagen
        funkoEntity.Foto = newFileName;
        funkoEntity.UpdatedAt = DateTime.UtcNow;

        // Guardar los cambios en la base de datos
        _context.Funkos.Update(funkoEntity);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Profile photo updated successfully for funko with ID: {id}");

        return newFileName; // Retornar el nombre del archivo guardado
    }
    
    public async Task<FileStream> GetFileAsync(string fileName)
    {
        _logger.LogInformation($"Getting file: {fileName}");
        try
        {
            var filePath = Path.Combine(_fileStorageConfig.UploadDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                throw new FileNotFoundException($"File not found: {fileName}");
            }
            
            _logger.LogInformation($"File found: {filePath}");
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file");
            throw;
        }
    }

}