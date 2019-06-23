namespace Gu.Roslyn.Asserts.Tests.NetCoreWithAttributes
{
    using Gu.Roslyn.Asserts.Tests.NetCoreWithAttributes.AnalyzersAndFixes;
    using NUnit.Framework;

    public class RoslynAssertTests
    {
        [Test]
        public void ValidOrdersController()
        {
            var order = @"
namespace ValidCode
{
    public class Order
    {
        public int Id { get; set; }
    }
}";
            var db = @"
namespace ValidCode
{
    using Microsoft.EntityFrameworkCore;

    public class Db : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}";
            var controller = @"
namespace ValidCode
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    public class OrdersController : Controller
    {
        private readonly Db db;

        public OrdersController(Db db)
        {
            this.db = db;
        }

        [HttpGet(""api/orders/{id}"")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var match = await this.db.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (match == null)
            {
                return this.NotFound();
            }

            return this.Ok(match);
        }
    }
}";
            var analyzer = new FieldNameMustNotBeginWithUnderscore();
            RoslynAssert.Valid(analyzer, order, db, controller);
        }
    }
}
