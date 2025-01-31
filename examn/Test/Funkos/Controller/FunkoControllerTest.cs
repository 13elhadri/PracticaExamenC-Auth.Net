using examn.Funkos.Controller;
using examn.Funkos.Dto;
using examn.Funkos.Models;
using examn.Funkos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Funkos.Controller;

[TestFixture]
[TestOf(typeof(FunkoController))]
public class FunkoControllerTest
{

     private Mock<ILogger<FunkoController>> _loggerMock;
    private Mock<IFunkoService> _funkoServiceMock;
    private FunkoController _controller;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<FunkoController>>();
        _funkoServiceMock = new Mock<IFunkoService>();
        _controller = new FunkoController(_funkoServiceMock.Object);
    }

    [Test]
    public async Task GetAll_ShouldReturnOk_WhenFunkosExist()
    {
        // Arrange
        var funkos = new List<Funko>
        {
            new Funko { Id = 1, Nombre = "Funko1" },
            new Funko { Id = 2, Nombre = "Funko2" }
        };

        _funkoServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(funkos);

        // Act
        var result = await _controller.Getall();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        var returnValue = okResult?.Value as IEnumerable<Funko>;
        Assert.That(returnValue, Is.EqualTo(funkos));
    }

    [Test]
    public async Task GetById_ShouldReturnOk_WhenFunkoExists()
    {
        // Arrange
        var funko = new Funko { Id = 1, Nombre = "Funko1" };
        _funkoServiceMock.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(funko);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnValue = okResult?.Value as Funko;
        Assert.That(returnValue, Is.EqualTo(funko));
    }

    [Test]
    public async Task GetById_ShouldReturnNotFound_WhenFunkoDoesNotExist()
    {
        // Arrange
        _funkoServiceMock.Setup(service => service.GetByIdAsync(999)).ReturnsAsync((Funko)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Create_ShouldReturnOk_WhenFunkoIsCreatedSuccessfully()
    {
        // Arrange
        var request = new FunkoRequest { Nombre = "Funko3" };
        var expectedFunko = new Funko { Id = 3, Nombre = "Funko3" };

        _funkoServiceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(expectedFunko);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnValue = okResult?.Value as Funko;
        Assert.That(returnValue, Is.EqualTo(expectedFunko));
    }

    [Test]
    public async Task Create_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new FunkoRequest { Nombre = "FunkoError" };
        _funkoServiceMock.Setup(service => service.CreateAsync(request)).ThrowsAsync(new Exception("Error"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_ShouldReturnOk_WhenFunkoIsDeletedSuccessfully()
    {
        // Arrange
        var funko = new Funko { Id = 1, Nombre = "Funko1" };
        _funkoServiceMock.Setup(service => service.DeleteByGuidAsync(1)).ReturnsAsync(funko);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnValue = okResult?.Value as Funko;
        Assert.That(returnValue, Is.EqualTo(funko));
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_WhenFunkoDoesNotExist()
    {
        // Arrange
        _funkoServiceMock.Setup(service => service.DeleteByGuidAsync(999)).ReturnsAsync((Funko)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Update_ShouldReturnOk_WhenFunkoIsUpdatedSuccessfully()
    {
        // Arrange
        var request = new FunkoRequest { Nombre = "Updated Funko" };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("newImage.png");
        fileMock.Setup(f => f.Length).Returns(1024);  // Simulando un tamaño de archivo
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Suponemos que el método UpdateFunkoFotoAsync devolverá el nombre del archivo
        _funkoServiceMock.Setup(service => service.UpdateFunkoFotoAsync(1, fileMock.Object))
            .ReturnsAsync("newImage.png");

        // Act
        var result = await _controller.Update(1, fileMock.Object);

        // Assert
        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnValue = okResult?.Value as string;

        // Verificamos que el valor devuelto sea el nombre del archivo
        Assert.That(returnValue, Is.EqualTo("newImage.png"));
    }


    [Test]
    public async Task Update_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new FunkoRequest { Nombre = "Invalid Update" };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("invalidImage.png");
        fileMock.Setup(f => f.Length).Returns(2048);  // Simulando un tamaño de archivo

        // Hacemos que el método UpdateFunkoFotoAsync lance una excepción
        _funkoServiceMock.Setup(service => service.UpdateFunkoFotoAsync(1, fileMock.Object))
            .ThrowsAsync(new Exception("Update error"));

        // Act
        var result = await _controller.Update(1, fileMock.Object);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult?.Value, Is.EqualTo("Error en la actualización del Funko: Update error"));
    }

}