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
        public IActionResult GetAllOrders(
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status,
            [FromQuery] int? clientId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = dbContext.Orders.AsQueryable();

            if (minAmount.HasValue)
                query = query.Where(o => o.Amount >= minAmount);

            if (maxAmount.HasValue)
                query = query.Where(o => o.Amount <= maxAmount);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.ToString() == status);

            if (clientId.HasValue)
                query = query.Where(o => o.ClientId == clientId);

            var totalCount = query.Count();

            var orders = query.Include(o => o.Client).Skip((page - 1) * pageSize).Take(pageSize).Select(o => new OrderResponseDto
            {
                Id = o.Id,
                Amount = o.Amount,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                ClientId = o.ClientId,
                ClientName = $"{o.Client.FirstName} {o.Client.LastName}",
            }).ToList();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = orders
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = dbContext.Orders.Where(o => o.Id == id).Select(o => new OrderResponseDto
            {
                Id = o.Id,
                Amount = o.Amount,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                ClientId = o.ClientId,
                ClientName = $"{o.Client.FirstName} {o.Client.LastName}"
            }).FirstOrDefault();

            if (order == null) return NotFound();

            return Ok(order);
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
