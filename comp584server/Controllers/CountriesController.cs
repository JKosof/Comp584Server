using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldModel;
using comp584server.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace comp584server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(Comp584Context context) : ControllerBase
    {

        // GET: api/Countries
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            return await context.Countries.ToListAsync();
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            context.Entry(country).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            context.Countries.Add(country);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return context.Countries.Any(e => e.Id == id);
        }

        // Replace the following method in CountriesController

        [HttpGet("population")]
        public async Task<ActionResult<IEnumerable<CountryPopulation>>> GetCountryPopulation()
        {
            return await context.Countries
                .Select(c => new CountryPopulation
                {
                    Id = c.Id,
                    Name = c.Name,
                    Iso2 = c.Iso2,
                    Iso3 = c.Iso3,
                    Population = c.Cities.Sum(city => city.Population)
                })
                .ToListAsync();
        }

        [HttpGet("population/{id}")]
        public ActionResult<CountryPopulation> GetCountryPopulationById(int id)
        {            
            return context.Countries.Select(country => new CountryPopulation
                {
                Id = country.Id,
                Name = country.Name,
                Iso2 = country.Iso2,
                Iso3 = country.Iso3,
                Population = country.Cities.Sum(city => city.Population)
            }).Single(c => c.Id == id);
        }
    }
}
