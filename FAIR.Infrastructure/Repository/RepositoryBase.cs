
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FAIR.Infrastructure.Repository    
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly dbContext context;
        public RepositoryBase(dbContext context)
        {
            this.context = context;
        }
        public void Create(T entity) => context.Set<T>().Add(entity);
        public void Delete(T entity) => context?.Set<T>().Remove(entity);
        public void Update(T entity) => context.Set<T>().Update(entity);

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ? context.Set<T>().AsNoTracking()
            : context.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges) =>
        !trackChanges ? context.Set<T>().Where(condition).AsNoTracking() : context.Set<T>().Where(condition);
    }
}
