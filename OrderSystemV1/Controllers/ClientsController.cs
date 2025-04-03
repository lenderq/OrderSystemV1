using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderSystemV1.Data;
using OrderSystemV1.Models.Entities;
using OrderSystemV1.Models;
using System.Data.Common;

namespace OrderSystemV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ClientsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllClients(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] DateTime? birthDateFrom,
            [FromQuery] DateTime? birthDateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = dbContext.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(c => c.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(c => c.LastName.Contains(lastName));

            if (birthDateFrom.HasValue)
                query = query.Where(c => c.BirthDate >= birthDateFrom);

            if (birthDateTo.HasValue)
                query = query.Where(c=> c.BirthDate <= birthDateTo);

            var totalCount = query.Count();
            var clients = query.Skip((page - 1)* pageSize).Take(pageSize).Select(c=> new ClientResponseDto()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                BirthDate = c.BirthDate,
                OrderCount = c.Orders.Count()
                
            }).ToList();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = clients
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            var client = dbContext.Clients.Where(c => c.Id == id).Select(c => new ClientResponseDto()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                BirthDate= c.BirthDate,
                OrderCount = c.Orders.Count()
            }).FirstOrDefault();

            if (client == null) return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public IActionResult AddClient(ClientDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clientEntity = new Clients()
            {
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                BirthDate = clientDto.BirthDate,
            };

            dbContext.Clients.Add(clientEntity);

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                return StatusCode(500, "Ошибка при сохранении данных");
            }

            var response = new ClientResponseDto()
            {
                Id = clientEntity.Id,
                FirstName = clientEntity.FirstName,
                LastName = clientEntity.LastName,
                BirthDate = clientEntity.BirthDate,
            };

            return CreatedAtAction(nameof(GetClient), new { id = clientEntity.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateClient(int id, ClientDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = dbContext.Clients.Find(id);

            if (client == null) return NotFound();

            client.FirstName = clientDto.FirstName;
            client.LastName = clientDto.LastName;
            client.BirthDate = clientDto.BirthDate;

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                return StatusCode(500, "Ошибка при сохранении данных");
            }

            var response = new ClientResponseDto()
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                BirthDate = client.BirthDate,
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = dbContext.Clients.Find(id);

            if (client == null) return NotFound();

            dbContext.Clients.Remove(client);

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

    }
}
