using System.Linq.Expressions;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly AppDbContext _appDbContext;
    
    public RepositoryBase(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges ? _appDbContext.Set<T>().AsNoTracking() : _appDbContext.Set<T>();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) => !trackChanges
        ? _appDbContext.Set<T>().Where(expression).AsNoTracking()
        : _appDbContext.Set<T>().Where(expression);

    public void Create(T entity) => _appDbContext.Set<T>().Add(entity);
    
    public void Update(T entity) => _appDbContext.Set<T>().Update(entity);

    public void Delete(T entity) => _appDbContext.Set<T>().Remove(entity);
}