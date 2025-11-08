using comp584server.Data;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using WorldModel;

namespace comp584server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(Comp584Context context, IHostEnvironment environment,
        RoleManager<IdentityRole> roleManager, UserManager<WorldModelUser> userManager, IConfiguration configuration) : ControllerBase
    {
        string _pathName = Path.Combine(environment.ContentRootPath, "Data/worldcities.csv");
        [HttpPost ("Countries")]
        public async Task<ActionResult> PostCountries()
        {
            Dictionary<string, Country> countries = await context.Countries.AsNoTracking().
                ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new(CultureInfo.InvariantCulture) 
            { 
                HasHeaderRecord = true, 
                HeaderValidated = null 
            }; 
            using StreamReader reader = new(_pathName); 
            using CsvReader csv = new(reader, config); 
            List<Comp584csv> records = csv.GetRecords<Comp584csv>().ToList();
            foreach (Comp584csv record in records)
            {
                if (!countries.ContainsKey(record.country))
                {
                    Country country = new() 
                    { 
                        Name = record.country, 
                        Iso2 = record.iso2, 
                        Iso3 = record.iso3 
                    };
                    countries.Add(country.Name, country);
                    await context.Countries.AddAsync(country);
                }
            }
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Cities")]
        public async Task<ActionResult> PostCities()
        {
            Dictionary<string, Country> countries = await context.Countries.AsNoTracking().
                ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            { 
                HasHeaderRecord = true, 
                HeaderValidated = null 
            };
            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<Comp584csv> records = csv.GetRecords<Comp584csv>().ToList();
            int cityCount = 0;
            foreach (Comp584csv record in records)
            {
                if (record.population.HasValue && record.population.Value > 0)
                {
                    if (countries.TryGetValue(record.country, out Country? country))
                    {
                        City city = new()
                        {
                            Name = record.city,
                            Lat = (double)record.lat,
                            Lng = (double)record.lng,
                            Population = (int)record.population.Value,
                            CountryId = countries[record.country].Id
                        };
                        await context.Cities.AddAsync(city);
                        cityCount++;
                    }
                }
            }

            await context.SaveChangesAsync();

            return new JsonResult(cityCount);
        }

        [HttpPost("Users")]
        public async Task<ActionResult> PostUsers()
        {
            string administrator = "admin";
            string registeredUser = "user";
            if (!await roleManager.RoleExistsAsync(administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(administrator));
            }

            if (!await roleManager.RoleExistsAsync(registeredUser))
            {
                await roleManager.CreateAsync(new IdentityRole(registeredUser));
            }

            WorldModelUser adminUser = new()
            {
                UserName = "admin",
                Email = "joshkosof@gmail.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(adminUser, configuration["DefaultPasswords:admin"]!);

            await userManager.AddToRoleAsync(adminUser, administrator);

            WorldModelUser regularUser = new()
            {
                UserName = "user",
                Email = "joshkosof@aol.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(regularUser, configuration["DefaultPasswords:user"]!);

            await userManager.AddToRoleAsync(regularUser, registeredUser);

            return Ok();
        }
    }
}
