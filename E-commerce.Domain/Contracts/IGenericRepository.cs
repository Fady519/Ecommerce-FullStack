using E_commerce.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntitey<TKey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity , TKey> specifications);

        Task<TEntity?> GetById(TKey id);

        Task<TEntity?> GetById(ISpecifications<TEntity ,TKey> specifications);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);

        Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications);

    }
}
