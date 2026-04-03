using Microsoft.EntityFrameworkCore;
using Pagarte.API.Domain.Entities;
using Pagarte.API.Interfaces;

namespace Pagarte.API.Infrastructure.Repository
{
    public class CreditCardRepository(PagarteDbContext context) : ICreditCardRepository
    {
        private readonly PagarteDbContext _context = context;

        public async Task<IEnumerable<CreditCard>> GetByClientIdAsync(string clientId)
        {
            return await _context.CreditCards
                .Where(c => c.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<CreditCard?> GetByIdAsync(Guid id)
        {
            return await _context.CreditCards.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CreditCard> CreateAsync(CreditCard creditCard)
        {
            _context.CreditCards.Add(creditCard);
            await _context.SaveChangesAsync();
            return creditCard;
        }

        public async Task UpdateAsync(CreditCard creditCard)
        {
            _context.CreditCards.Update(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, string clientId)
        {
            var card = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.Id == id && c.ClientId == clientId);
            if (card != null)
            {
                card.Delete();
                await _context.SaveChangesAsync();
            }
        }
    }
}
