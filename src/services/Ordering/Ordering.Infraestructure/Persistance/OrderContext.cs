using Microsoft.EntityFrameworkCore;


namespace Ordering.Infraestructure.Persistance
{
    public class OrderContext:DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

    }
}
