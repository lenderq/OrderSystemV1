using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderSystemV1.Controllers;
using OrderSystemV1.Data;
using OrderSystemV1.Models.Entities;
using OrderSystemV1.Models;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ClientsControllerTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public ClientsControllerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void GetAllClients_ReturnsPaginatedList()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            SeedTestClients(context);
            var controller = new ClientsController(context);

            var result = controller.GetAllClients(null, null, null, null, 1, 5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalCount = (int)response.GetType().GetProperty("TotalCount").GetValue(response);
            var items = (List<ClientResponseDto>)response.GetType().GetProperty("Items").GetValue(response);

            Assert.Equal(10, totalCount);
            Assert.Equal(5, items.Count);
        }
    }

    [Fact]
    public void GetAllClients_FiltersByFirstName()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            SeedTestClients(context);
            var controller = new ClientsController(context);

            var result = controller.GetAllClients("John", null, null, null, 1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            var totalCount = (int)response.GetType().GetProperty("TotalCount").GetValue(response);
            var items = (List<ClientResponseDto>)response.GetType().GetProperty("Items").GetValue(response);

            Assert.Equal(1, totalCount);
            Assert.Equal("John", items[0].FirstName);
        }
    }

    [Fact]
    public void GetClient_ReturnsClient_WhenExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            SeedTestClients(context);

            var controller = new ClientsController(context);

            var result = controller.GetClient(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var client = Assert.IsType<ClientResponseDto>(okResult.Value);
            Assert.Equal(1, client.Id);
        }
    }

    [Fact]
    public void GetClient_ReturnsNotFound_WhenNotExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var controller = new ClientsController(context);

            var result = controller.GetClient(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public void DeleteClient_ReturnsNotFound_WhenClientNotExists()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var controller = new ClientsController(context);

            var result = controller.DeleteClient(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }

    private void SeedTestClients(ApplicationDbContext context)
    {
        for (int i = 1; i <= 10; i++)
        {
            context.Clients.Add(new Clients
            {
                Id = i,
                FirstName = i == 1 ? "John" : $"FirstName{i}",
                LastName = $"LastName{i}",
                BirthDate = new DateTime(1990 + i, 1, 1)
            });
        }
        context.SaveChanges();
    }
}
