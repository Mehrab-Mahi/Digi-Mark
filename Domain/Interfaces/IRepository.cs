using System.Linq.Expressions;
using Domain.Models;

namespace Domain.Interfaces;

public interface IRepository<T> where T : Entity
{
    void Insert(T entity);
    void Insert(List<T> entities);
    void Update(T entity);
    void AddOrUpdate(T entity);
    IQueryable<T> GetConditional(Expression<Func<T, bool>> expression);
    IQueryable<T> GetAll();
    T? Find(string id);
    void Delete(T entity);
    void Delete(List<T> entity);
    void Delete(string id);
    void SaveChanges();
    void SaveChangesAsync();
    bool Any(Expression<Func<T, bool>> expression);
    void RemoveAll(List<T> entities);
}