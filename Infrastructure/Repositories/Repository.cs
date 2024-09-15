using System.Linq.Expressions;
using System.Text;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly DmDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _loggedInUserName = string.Empty;

    public Repository(DmDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        ParseLoggedInUser();
    }

    private void ParseLoggedInUser()
    {
        _httpContextAccessor.HttpContext.Session.TryGetValue("username", out var bytes);

        if (bytes is not null)
        {
            _loggedInUserName = Encoding.UTF8.GetString(bytes);
        }
    }

    public void Insert(T entity)
    {
        if (entity != null!)
        {
            var createTime = DateTime.UtcNow;
            entity.CreateTime = createTime;
            entity.LastModifiedTime = createTime;
        }

        entity!.CreatedBy = _loggedInUserName;
        entity.LastModifiedBy = _loggedInUserName;

        _dbContext.Entry(entity).State = EntityState.Added;
    }

    public void Insert(List<T> entities)
    {
        foreach (var entity in entities)
        {
            Insert(entity);
        }
    }

    public void Update(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        entity.LastModifiedTime = DateTime.UtcNow;
        entity.LastModifiedBy = _loggedInUserName;

        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void AddOrUpdate(T entity)
    {
        var exists = Any(e => e.Id == entity.Id);
        if (!exists) Insert(entity);
        else Update(entity);
    }

    public virtual IQueryable<T> GetConditional(Expression<Func<T, bool>> expression)
    {
        return _dbContext.Set<T>().Where(expression);
    }

    public IQueryable<T> GetAll()
    {
        return _dbContext.Set<T>();
    }

    public T? Find(string id)
    {
        return GetAll().FirstOrDefault(e => e.Id == id);
    }

    public void Delete(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Deleted;
    }

    public void Delete(List<T> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }

    public void Delete(string id)
    {
        Delete(Find(id)!);
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
    
    public void SaveChangesAsync()
    {
        _dbContext.SaveChangesAsync();
    }

    public virtual bool Any(Expression<Func<T, bool>> expression)
    {
        return GetAll().Any(expression);
    }

    public void RemoveAll(List<T> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }
}