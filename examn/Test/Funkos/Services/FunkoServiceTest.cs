using examn.Database;
using examn.Database.Entities;
using examn.Funkos.Services;
using examn.Funkos.Storage.Config;
using examn.User.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Funkos.Services;

[TestFixture]
[TestOf(typeof(FunkoService))]
public class FunkoServiceTest
{

        private FunkoService _funkoService;
        private Mock<ILogger<UserService>> _mockLogger;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<FileStorageConfig> _mockFileStorageConfig;
        private GeneralDbContext _context;

        // Este método se ejecuta antes de cada test para inicializar los mocks y el contexto.
        [SetUp]
        public void Setup()
        {
            // Configurar DbContext en memoria
            var options = new DbContextOptionsBuilder<GeneralDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new GeneralDbContext(options);

            // Crear datos de prueba
            var categoria = new CategoriaEntity { Id = 1, Nombre = "Categoria1" };
            _context.Categorias.Add(categoria);
            _context.Funkos.Add(new FunkoEntity { Id = 1, Nombre = "Funko1", Foto = "foto1.jpg", CategoriaId = 1, IsDeleted = false });
            _context.SaveChanges();

            // Configurar Mock de dependencias
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockFileStorageConfig = new Mock<FileStorageConfig>();
            _mockFileStorageConfig.Setup(x => x.UploadDirectory).Returns("C:\\uploads");

            _funkoService = new FunkoService(
                _context,
                _mockLogger.Object,
                _mockMemoryCache.Object,
                _mockFileStorageConfig.Object
            );
        }
        
        [TearDown]
        public void TearDown()
        {
            // Si usas un DbContext, puedes asegurarte de liberarlo correctamente
            _context?.Dispose();
        }


        [Test]
        public async Task UpdateFunkoFotoAsync_ShouldUpdateFunko_WhenFileIsValid()
        {
            // Arrange
            var funkoId = 1L;
            var mockFunko = new FunkoEntity
            {
                Id = funkoId,
                Foto = "oldFoto.jpg",
                CategoriaId = 1,
                IsDeleted = false
            };

            _context.Funkos.Update(mockFunko);
            _context.SaveChanges();

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1);
            mockFile.Setup(f => f.FileName).Returns("newFoto.jpg");

            // Act
            var result = await _funkoService.UpdateFunkoFotoAsync(funkoId, mockFile.Object);

            // Assert
            Assert.That(result, Is.Not.Null);  // Verifica que el resultado no es null
            Assert.That(result.Contains("prueba"), Is.True);  // Verifica que el nombre del archivo contiene la palabra "prueba"
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnFunko_WhenFoundInDatabase()
        {
            // Arrange
            var funkoId = 1L;

            // Act
            var result = await _funkoService.GetByIdAsync(funkoId);

            // Assert
            Assert.That(result, Is.Not.Null);  // Verifica que el resultado no es null
            Assert.That(result.Id, Is.EqualTo(funkoId));  // Verifica que el id del funko es correcto
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenFunkoNotFound()
        {
            // Arrange
            var funkoId = 999L; // Id no existente

            // Act
            var result = await _funkoService.GetByIdAsync(funkoId);

            // Assert
            Assert.That(result, Is.Null);  // Verifica que el resultado es null
        }
    }
