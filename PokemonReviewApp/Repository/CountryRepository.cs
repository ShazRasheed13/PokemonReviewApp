using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CountryRepository(DataContext context, IMapper mapper) : ICountryRepository
    {
        public bool CountryExists(int countryId)
        {
            return context.Countries.Any(c=>c.Id == countryId);
        }

        public bool CreateCountry(Country category)
        {
            context.Add(category);
            context.SaveChanges();
            return Save();
        }

        public bool Save()
        {
            var saved= context.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public ICollection<Country> GetCountries()
        {
            return context.Countries.OrderBy(c=>c.Id).ToList();
        }

        public Country? GetCountry(int id)
        {
            return context.Countries.FirstOrDefault(c=>c.Id == id);
        }

        public Country? GetCountryByOwner(int ownerId)
        {
            return context.Owners.Where(o=>o.Id == ownerId).Select(o=>o.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersByCountry(int countryId)
        {
            return context.Owners.Where(o=>o.Country.Id == countryId).ToList();
        }
    }
}
