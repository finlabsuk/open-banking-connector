using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public interface IDbMultiEntityMethods
    {
        Task SaveChangesAsync();
    }
}