using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperHeroeAPI.Models;

namespace SuperHeroeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        public readonly DataContext _context;

        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> Get()
        {
            return Ok(await _context.SuperHeroes.ToListAsync());
        }

        [HttpGet("GetSP")]
        public async Task<ActionResult<List<SuperHero>>> GetSP()
        {
            try
            {
                return Ok(await _context.SuperHeroes.FromSqlRaw("TestProc").ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<SuperHero>>> Get(int id)
        {
            var hero = await _context.SuperHeroes.FindAsync(id);

            if (hero == null)
            {
                var error = new
                {
                    message = "Not found"
                };

                return NotFound(error);
            }

            return Ok(hero);
        }

        [HttpGet("GetById/{userId}")]
        public async Task<ActionResult<List<SuperHero>>> GetById(int userId)
        {
            try
            {
                var hero = await _context.SuperHeroes.FromSqlRaw($"spGetById {userId}").ToListAsync();

                if (hero.Count == 0)
                {
                    var error = new
                    {
                        message = "Not found"
                    };

                    return NotFound(error);
                }

                return Ok(hero);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddHero(SuperHero hero)
        {
            if (hero != null)
            {
                _context.SuperHeroes.Add(hero);

                await _context.SaveChangesAsync();

                return Ok(await _context.SuperHeroes.ToListAsync());
            }

            return BadRequest(new 
                {
                    message = "Bad request"
                }
            );
            
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> Update(SuperHero hero)
        {
            var dbHero = await _context.SuperHeroes.FindAsync(hero.Id);

            if (dbHero != null)
            {
                dbHero.FirstName = hero.FirstName;
                dbHero.LastName = hero.LastName;
                dbHero.Name = hero.Name;
                dbHero.Place = hero.Place;

                await _context.SaveChangesAsync();

                return Ok(await _context.SuperHeroes.ToListAsync());
            }

            return BadRequest(new
                {
                    message = "Bad request"
                }
            );
        }
    }
}
