using Microsoft.EntityFrameworkCore;
using PaymentBot.Models;

namespace PaymentBot.Data
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options)
: base(options)
        {
        }

        public DbSet<Payment> TsPayments { get; set; }
    }
}
