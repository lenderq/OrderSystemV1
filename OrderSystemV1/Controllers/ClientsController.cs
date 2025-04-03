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
        public IActionResult GetAllClients()
        {
            var allClients = dbContext.Clients.ToList();
            return Ok(allClients);
        }

        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            var client = dbContext.Clients.Find(id);

            if (client == null) return NotFound();

            var response = new ClientResponseDto()
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                BirthDate = client.BirthDate,
            };

            return Ok(response);
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
