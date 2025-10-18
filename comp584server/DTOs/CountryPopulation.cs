using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using comp584server.DTOs;
using WorldModel;

namespace comp584server.DTOs
{
    public class CountryPopulation
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        
        public required string Iso2 { get; set; }

        public required string Iso3 { get; set; }

        public required decimal Population { get; set; }
    }
}
