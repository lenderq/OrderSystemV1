using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderSystemV1.Controllers;
using OrderSystemV1.Data;
using OrderSystemV1.Models.Entities;
using OrderSystemV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace TestControllers.Tests
{
    public class OrdersControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public OrdersControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetAllOrders_ReturnsPaginatedList()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                SeedTestData(context);
                var controller = new OrdersController(context);

                var result = controller.GetAllOrders(null, null, null, null, null, null, 1, 5);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value;

                var totalCount = (int)response.GetType().GetProperty("TotalCount").GetValue(response);
                var items = (List<OrderResponseDto>)response.GetType().GetProperty("Items").GetValue(response);

                Assert.Equal(10, totalCount);
                Assert.Equal(5, items.Count);
            }
        }

        [Fact]
        public void GetOrder_ReturnsOrder_WhenExists()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                SeedTestData(context);

                var controller = new OrdersController(context);

                var result = controller.GetOrder(1);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var order = Assert.IsType<OrderResponseDto>(okResult.Value);
                Assert.Equal(1, order.Id);
            }
        }

        [Fact]
        public void GetOrder_ReturnsNotFound_WhenNotExists()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var controller = new OrdersController(context);

                var result = controller.GetOrder(999);

                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public void DeleteOrder_ReturnsNoContent_WhenOrderExists()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                SeedTestData(context);

                var controller = new OrdersController(context);

                var result = controller.DeleteOrder(1);

                Assert.IsType<NoContentResult>(result);
                Assert.Equal(9, context.Orders.Count());
            }
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            SeedTestClients(context);

            for (int i = 1; i <= 10; i++)
            {
                context.Orders.Add(new Orders
                {
                    Id = i,
                    Amount = 100 * i,
                    OrderDate = DateTime.Now.AddDays(-i),
                    Status = i % 3 == 0 ? OrderStatus.Cancelled :
                             i % 3 == 1 ? OrderStatus.Completed : OrderStatus.NotProcessed,
                    ClientId = 1
                });
            }
            context.SaveChanges();
        }

        private void SeedTestClients(ApplicationDbContext context)
        {
            context.Clients.Add(new Clients
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Client",
                BirthDate = new DateTime(1990, 1, 1)
            });
            context.SaveChanges();
        }
    }
}