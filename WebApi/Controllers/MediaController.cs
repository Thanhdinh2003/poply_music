using App.Domain.Entities;
using App.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        public MediaController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        // GET: api/<MediaController>
        [HttpGet("media")]
        public async Task<IActionResult> SearchMedia(string? namesearchText)
        {
            var res = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.MediaContent)
                .Where(p => string.IsNullOrWhiteSpace(namesearchText) || p.Name.ToLower().Contains(namesearchText.Trim().ToLower()))
                .AsNoTracking()
                .ToListAsync();
            return Ok(res);
        }

        // GET api/<MediaController>/5
        [HttpGet("media/{id}")]
        public async Task<IActionResult> GetDetailMedia(Guid id)
        {
            var res = await _databaseContext.Media
                 .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return Ok(res);
        }

        // POST api/<MediaController>
        [HttpPost("media")]
        public async Task<IActionResult> CreateMedia([FromBody] Media request)
        {
            request.Id = Guid.NewGuid();
            foreach (var item in request.MediaContent)
            {
                item.MediaId = request.Id;
            }
            await _databaseContext.Media.AddAsync(request);

            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // POST api/<MediaController>
        [HttpPut("media")]
        public async Task<IActionResult> UpdateMedia([FromBody] Media request)
        {
            var media = await _databaseContext.Media.FirstOrDefaultAsync(p => p.Id == request.Id);
            _databaseContext.Update(request);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // PUT api/<MediaController>/5
        [HttpDelete("media/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var media = await _databaseContext.Media.FirstOrDefaultAsync(p => p.Id == id);
            if(media == null)
            {
                throw new ArgumentException("cannot found media");
            }
            _databaseContext.Remove(media);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // GET: api/<MediaController>
        [HttpGet("author")]
        public async Task<IActionResult> GetAuthor(string? namesearchText)
        {
            var res = await _databaseContext.Authors
                .Where(p => string.IsNullOrWhiteSpace(namesearchText) || p.Name.ToLower().Contains(namesearchText.Trim().ToLower()))
                .AsNoTracking()
                .ToListAsync();
            return Ok(res);
        }

        // GET api/<MediaController>/5
        [HttpGet("author/{id}")]
        public async Task<IActionResult> GetDetailAuthor(Guid id)
        {
            var res = await _databaseContext.Authors
                .Where(p => p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return Ok(res);
        }

        [HttpPost("author")]
        public async Task<IActionResult> CreateAuthor([FromBody] Author request)
        {
            await _databaseContext.AddAsync(request);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // POST api/<MediaController>
        [HttpPut("author")]
        public async Task<IActionResult> UpdateAuthor([FromBody] Author request)
        {
            var media = await _databaseContext.Authors.FirstOrDefaultAsync(p => p.Id == request.Id);
            _databaseContext.Update(request);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // PUT api/<MediaController>/5
        [HttpDelete("author/{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var media = await _databaseContext.Authors.FirstOrDefaultAsync(p => p.Id == id);
            _databaseContext.Remove(media);
            return Ok(await _databaseContext.SaveChangesAsync());
        }


        // GET: api/<MediaController>
        [HttpGet("category")]
        public async Task<IActionResult> GetCategory(string? namesearchText)
        {
            var res = await _databaseContext.Categories
                .Where(p => string.IsNullOrWhiteSpace(namesearchText) || p.Name.ToLower().Contains(namesearchText.Trim().ToLower()))
                .AsNoTracking()
                .ToListAsync();
            return Ok(res);
        }

        // GET api/<MediaController>/5
        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetDetailCategory(Guid id)
        {
            var res = await _databaseContext.Categories
                .Where(p => p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return Ok(res);
        }

        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory([FromBody] Category request)
        {
            await _databaseContext.AddAsync(request);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // POST api/<MediaController>
        [HttpPut("category")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category request)
        {
            var media = await _databaseContext.Categories.FirstOrDefaultAsync(p => p.Id == request.Id);
            _databaseContext.Update(request);
            return Ok(await _databaseContext.SaveChangesAsync());
        }

        // PUT api/<MediaController>/5
        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var media = await _databaseContext.Authors.FirstOrDefaultAsync(p => p.Id == id);
            _databaseContext.Remove(media);
            return Ok(await _databaseContext.SaveChangesAsync());
        }
    }
}
