using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository(DataContext context) : ICategoryRepository
    {
        public bool CategoryExists(int categoryId) => context.Categories.Any(c=>c.Id == categoryId);
        public bool CreateCategory(Category category)
        {
            //change Tracker - add,update,modifying,connected vs disconnected
            context.Add(category);
            context.SaveChanges();
            return Save();
        }

        public bool UpdateCategory(Category category)
        {
            context.Update(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            context.Remove(category);
            return Save();
        }


        public bool Save()
        {
            var saved= context.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public ICollection<Category> GetCategories() => context.Categories.OrderBy(c=>c.Id).ToList();

        public Category? GetCategory(int id) => context.Categories.FirstOrDefault(c=>c.Id == id);

        public Category? GetCategory(string name) => throw new NotImplementedException();

        public ICollection<Pokemon> GetPokemonsByCategory(int categoryId) => context.PokemonCategories.Where(pc=>pc.CategoryId == categoryId).Select(pc=>pc.Pokemon).ToList();
    }
}
