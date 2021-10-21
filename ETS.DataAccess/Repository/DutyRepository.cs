using ETS.DataAccess.Data;
using ETS.DataAccess.Repository.IRepository;
using ETS.Models.Models;

namespace ETS.DataAccess.Repository
{
    public class DutyRepository : RepositoryAsync<Duty>, IDutyRepository
    {
        private readonly ApplicationDbContext _db;

        public DutyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

      
        public void Update(Duty duty)
        {
            _db.duties.Update(duty);
         
        }
    }
}
