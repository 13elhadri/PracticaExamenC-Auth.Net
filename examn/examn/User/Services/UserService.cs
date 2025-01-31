using System.Security.Claims;
using examn.Database;
using examn.User.Dto;
using examn.User.Mappers;
using examn.Utils.Auth.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace examn.User.Services;

public class UserService: IUserService
{
    
    private readonly GeneralDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKeyPrefix = "User:";
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserService(
        GeneralDbContext context,
        ILogger<UserService> logger,
        IMemoryCache memoryCache, 
        IJwtService jwtService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _memoryCache = memoryCache;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        _logger.LogInformation("Buscondo todos los usuarios desde la base de datos");
       var usuarios = await _context.Usuarios.Where(u=> u.IsDeleted == false ).ToListAsync();

       return usuarios.Select(u => u.ToResponseFromEntity());
    }

    public async Task<UserResponse?> GetByIdAsync(long guid)
    {
        _logger.LogInformation($"Buscando usuario con id: {guid}");

        var cacheKey = CacheKeyPrefix + guid;
        
        if (_memoryCache.TryGetValue(cacheKey, out Models.User? memoryCacheUser))
        {
            _logger.LogInformation("Usuario obtenido desde la memoria caché");
            return memoryCacheUser!.ToResponseFromModel();
        }
        
        
        var userEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == guid);
        if (userEntity != null)
        {
            _logger.LogInformation($"Usuario encontrado con id: {guid}");

            // Mapear entidad a modelo y respuesta
            var userModel = userEntity.ToModelFromEntity();
            var userResponse = userModel.ToResponseFromModel();

            // Guardar en la memoria caché
            _memoryCache.Set(cacheKey, userModel, TimeSpan.FromMinutes(30));
            
            return userResponse;
        }

        _logger.LogInformation($"Usuario no encontrado con id: {guid}");
        return null;

    }

    public async Task<UserResponse?> GetByUsernameAsync(string username)
    {
        _logger.LogInformation($"Buscando usuario con nombre de usuario: {username}");

        var userEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        if (userEntity.IsDeleted)
        {
            return null;
        }
        if (userEntity != null)
        {
            _logger.LogInformation($"Usuario encontrado con nombre de usuario: {username}");
            return userEntity.ToResponseFromEntity();
        }

        _logger.LogInformation($"Usuario no encontrado con nombre de usuario: {username}");
        return null;    
    }

    public async Task<UserResponse> CreateAsync(UserRequest userRequest)
    {
        _logger.LogInformation("Creando Usuario");

        if (await _context.Usuarios.AnyAsync(u => u.Username.ToLower() == userRequest.Username.ToLower()))
        {
            _logger.LogWarning($"Ya existe un usuario con el nombre: {userRequest.Username} (en base de datos)");
            throw new Exception($"Ya existe un Usuario con el nombre: {userRequest.Username}");
        }
        

        //Crear Usuario
        var userModel = userRequest.ToModelFromRequest();
            
        // Generar el hash con bcrypt
        userModel.Password = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);
            
        var userEntity = userModel.ToEntityFromModel();
        _context.Usuarios.Add(userEntity);
        await _context.SaveChangesAsync();

        var cacheKey = CacheKeyPrefix + userModel.Id;

        // Guardar el usuario 
        _memoryCache.Set(cacheKey, userModel, TimeSpan.FromMinutes(30));

        _logger.LogInformation("Usuario creado con éxito");
        return userModel.ToResponseFromModel();    
    }

    public async Task<UserResponse?> UpdateAsync(long guid, UserRequestUpdate userRequestUpdate)
    {
     
        var user = await _context.Usuarios.FirstOrDefaultAsync(c => c.Id == guid);
        
        if (user != null)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRequestUpdate.Contraseña);
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            
            var cacheKey = CacheKeyPrefix + user.Id;
            _memoryCache.Remove(cacheKey);

            _memoryCache.Set(cacheKey, user.ToModelFromEntity(), TimeSpan.FromMinutes(30));
            
            return user.ToResponseFromEntity();
            
        }

        return null;
    }

    public async Task<UserResponse?> DeleteByGuidAsync(long guid)
    {
         _logger.LogInformation($"Borrando Usuario con guid: {guid}");

            var userExistenteEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == guid);
            if (userExistenteEntity == null)
            {
                _logger.LogWarning($"Usuario no encontrado con id: {guid}");
                return null;
            }

            userExistenteEntity.IsDeleted = true;
            userExistenteEntity.UpdatedAt = DateTime.UtcNow;
            _context.Usuarios.Update(userExistenteEntity);
            
            _logger.LogInformation("Guardando todos los cambios en la base de datos");
            await _context.SaveChangesAsync();

            var cacheKey = CacheKeyPrefix + userExistenteEntity.Id;

            // Eliminar de la cache en memoria
            _memoryCache.Remove(cacheKey);
            
            _logger.LogInformation($"Usuario borrado (desactivado) con id: {guid}");
            return userExistenteEntity.ToResponseFromEntity();
    }

    public string Authenticate(string username, string password)
    {
        _logger.LogInformation($"Buscando usuario para realizar login");
        var user = _context.Usuarios.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning($"Credenciales inválidas para el usuario: {username}");
            throw new UnauthorizedAccessException($"Credenciales inválidas para el usuario: {username}");
        }
            
        _logger.LogInformation($"Usuario encontrado y verificado, generando Token");
        return _jwtService.GenerateToken(user.ToModelFromEntity());
    }

    public Models.User? GetAuthenticatedUser()
    {
        _logger.LogInformation("Buscando usuario autenticado");
        var username = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("No se ha encontrado el nombre de usuario en el contexto de seguridad");
            return null;
        }

        _logger.LogInformation($"Buscando usuario con nombre de usuario: {username}");
        var userEntity = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
        if (userEntity != null)
        {
            _logger.LogInformation($"Usuario encontrado con nombre de usuario: {username}");
            return userEntity.ToModelFromEntity();
        }

        _logger.LogInformation($"Usuario no encontrado con nombre de usuario: {username}");
        return null;
    }
}