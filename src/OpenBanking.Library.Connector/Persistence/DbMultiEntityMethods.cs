using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    /// Non entity- (type-) specific DB methods 
    /// </summary>
    public class DbMultiEntityMethods : IDbMultiEntityMethods
    {
        private readonly BaseDbContext _db;

        public DbMultiEntityMethods(BaseDbContext db)
        {
            _db = db;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
