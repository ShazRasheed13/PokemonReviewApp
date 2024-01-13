using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository(DataContext context, IMapper mapper) : IOwnerRepository
    {
        public bool OwnerExists(int ownerId)
        {
            return context.Owners.Any(o=>o.Id == ownerId);
        }

        public ICollection<Owner> GetOwners()
        {
            return context.Owners.OrderBy(o=>o.Id).ToList();
        }

        public Owner? GetOwner(int ownerId)
        {
            return context.Owners.FirstOrDefault(o=>o.Id == ownerId);
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return context.PokemonOwners.Where(p => p.Pokemon.Id == pokeId).Select(o => o.Owner).ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return context.PokemonOwners.Where(o => o.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool CreateOwner(Owner owner)
        {
            context.Add(owner);
            context.SaveChanges();
            return Save();
        }

        public bool Save()
        {
            var saved= context.SaveChanges();
            return saved >= 0 ? true : false;
        }
    }
}
