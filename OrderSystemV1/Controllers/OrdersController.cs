using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderSystemV1.Data;
using OrderSystemV1.Models.Entities;
using OrderSystemV1.Models;
using System.Data.Common;

namespace OrderSystemV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public OrdersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var allOrders = dbContext.Orders
                .Select(o => new OrderResponseDto()
                {
                    Id = o.Id,
                    Amount = o.Amount,
                    OrderDate = o.OrderDate,
                    Status = o.Status.ToString(),
                    ClientId = o.ClientId
                })
                .ToList();

            return Ok(allOrders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = dbContext.Orders.Find(id);

            if (order == null) return NotFound();

            var response = new OrderResponseDto()
            {
                Id = order.Id,
                Amount = order.Amount,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                ClientId = order.ClientId
            };
            return Ok(response);
        }

        [HttpPost]
        public IActionResult AddOrder(OrderDto order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!dbContext.Clients.Any(c => c.Id == order.ClientId)) return BadRequest("Указанный клиент не существует");

            var orderEntity = new Orders()
            {
                Amount = order.Amount,
                OrderDate = order.OrderDate,
                Status = Enum.Parse<OrderStatus>(order.Status),
                ClientId = order.ClientId
            };

            dbContext.Orders.Add(orderEntity);

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                return StatusCode(500, "Ошибка при сохранении данных");
            }

            var response = new OrderResponseDto()
            {
                Id = orderEntity.Id,
                Amount = orderEntity.Amount,
                OrderDate = orderEntity.OrderDate,
                Status = orderEntity.Status.ToString(),
                ClientId = orderEntity.ClientId
            };

            return CreatedAtAction(nameof(GetOrder), new { id = orderEntity.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, OrderDto order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderEntity = dbContext.Orders.Find(id);

            if (orderEntity == null) return NotFound();

            if (!dbContext.Clients.Any(c => c.Id == order.ClientId)) return BadRequest("Указанный клиент не существует");

            orderEntity.Amount = order.Amount;
            orderEntity.Status = Enum.Parse<OrderStatus>(order.Status);
            orderEntity.ClientId = order.ClientId;
            orderEntity.OrderDate = order.OrderDate;

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                return StatusCode(500, "Ошибка при сохранении данных");
            }

            var response = new OrderResponseDto()
            {
                Id = orderEntity.Id,
                Amount = orderEntity.Amount,
                OrderDate = orderEntity.OrderDate,
                Status = orderEntity.Status.ToString(),
                ClientId = orderEntity.ClientId
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = dbContext.Orders.Find(id);

            if (order == null) return NotFound();

            dbContext.Orders.Remove(order);

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                return StatusCode(500, "Ошибка при сохранении данных");
            }

            return NoContent();
        }

        [HttpGet("orders-on-birthday")]
        public IActionResult GetOrdersOnBirthdat()
        {
            var result = dbContext.BirthdayOrderTotalResults.FromSqlRaw("select * from get_orders_on_birthday()").ToList();

            return Ok(result);
        }

        [HttpGet("average-check-by-hour")]
        public IActionResult GetAverageCheckByHour()
        {
            var result = dbContext.AverageCheckByHourResults.FromSqlRaw("select * from get_average_check_by_hour()").ToList();

            return Ok(result);
        }
    }
}
