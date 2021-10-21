using System;
using System.Threading.Tasks;

namespace ETS.DataAccess.Repository.IRepository
{

    public interface IUnitOfWork : IDisposable
    {
        
        IDutyRepository Duty { get; }

        IRegistrationRepository Registration { get; }



        Task<int> SaveAsync();
    }

}
