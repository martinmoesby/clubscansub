using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Service.Interfaces
{
    internal interface IGenericService<T>  where T : class
    {
        Task<IList<T>> GetAllAsync();

        Task<T> GetAsync(string Id);
        Task<T> GetAsync(Guid Id);
        
        Task<T> AddAsync(T Item);
        Task<IList<T>> AddAsync(IList<T> Items);
        Task<IList<T>> AddAsync(params T[] Items);
        
        Task<T> UpdateAsync(T item);
        Task<IList<T>> UpdateAsync(IList<T> Items);
        Task<IList<T>> UpdateAsync(params T[] Items);

        Task DeleteAsync(T Item);

    }
}
