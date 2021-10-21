using ETS.Models.Models;


namespace ETS.DataAccess.Repository.IRepository
{
    public interface IDutyRepository : IRepositoryAsync<Duty>
    {
        public void Update(Duty duty);
    }
}
