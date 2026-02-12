using Microsoft.AspNetCore.Mvc;


namespace Contoroller;
[ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = new List<string>
            {
                "Laptop",
                "Mouse",
                "Keyboard"
            };

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok($"Product dengan ID {id}");
        }

        [HttpPost]
        public IActionResult Create([FromBody] string name)
        {
            return Ok($"Product {name} berhasil dibuat");
        }
    }