using ETS.Models.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.DataAccess.Repository.IRepository
{
    public interface IRegistrationRepository : IRepositoryAsync<Registration>
    {
        public void Update(Registration registration);
    }
}
