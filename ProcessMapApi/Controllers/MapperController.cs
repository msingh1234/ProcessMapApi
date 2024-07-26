using Mapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProcessMapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapperController : ControllerBase
    {
        private readonly IProcessor _processor;
        public MapperController(IProcessor processor)
        {
            this._processor = processor?? throw new ArgumentNullException(nameof(processor));
        }

        // GET api/<MapperController>/"Sky is the limit."
        [HttpGet("longText")]
        public string Get(string longText)
        {
            return _processor.GenrateResponse(longText);

        }
    }
}
