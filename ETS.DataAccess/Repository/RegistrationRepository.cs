using ETS.DataAccess.Data;
using ETS.DataAccess.Repository.IRepository;
using ETS.Models.Models;
using ETS.Models.Models.Authentication;

namespace ETS.DataAccess.Repository
{
    public class RegistrationRepository : RepositoryAsync<Registration>, IRegistrationRepository
    {
        private readonly ApplicationDbContext _db;

        public RegistrationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Registration registration)
        {
            _db.registrations.Update(registration);

        }
    }
}
