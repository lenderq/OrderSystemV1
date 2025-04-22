using OrderSystemV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestControllers.Models
{
    public class PaginatedClientResponse
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<ClientResponseDto> Items { get; set; }
    }
}
