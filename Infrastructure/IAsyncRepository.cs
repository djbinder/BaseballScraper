using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BaseballScraper.Models;
using Microsoft.EntityFrameworkCore;


namespace BaseballScraper.Infrastructure
{
    // STATUS [ September 9, 2019 ] : this was straight pulled from: http://bit.ly/2lKxIOM
    // * Haven't customized for BaseballScraper yet
    // * So not sure if any of this would work (i.e., it's a TO-DO)
    // * This is added as a scoped service in Startup.cs
    // * 5 Parts
    //      1) public interface IAsyncRepository<T> where T : BaseEntity
    //      2) public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
    //      3) public class Worker : BaseEntity
    //      4) public interface IWorkerRepository : IAsyncRepository<Worker>
    //      5) public class WorkerRepository : EfRepository<Worker>, IWorkerRepository

    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);

        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);

        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate);

        Task<int> CountAll();
        Task<int> CountWhere(Expression<Func<T, bool>> predicate);
    }


    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {

        #region Fields

        protected BaseballScraperContext _context;
        public System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();

        #endregion

        public EfRepository(BaseballScraperContext context)
        {
            _context = context;
        }

        #region Public Methods

        public Task<T> GetById(int id) => _context.Set<T>().FindAsync(id);

        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().FirstOrDefaultAsync(predicate);

        public async Task Add(T entity)
        {
            // await Context.AddAsync(entity);
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task Update(T entity)
        {
            // In case AsNoTracking is used
            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public Task<int> CountAll() => _context.Set<T>().CountAsync();

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().CountAsync(predicate);

        #endregion

    }



    // NOTE: these are all examples and would need to be adjusted for BaseballScraper

    public class Worker : BaseEntity
    {
        public string FirstName { get;set;}
    }


    public interface IWorkerRepository : IAsyncRepository<Worker>
    {
        Task<Worker> GetByFirstName(string firstName);
    }


    public class WorkerRepository : EfRepository<Worker>, IWorkerRepository
    {
        public WorkerRepository(BaseballScraperContext context) : base(context) { }

        public Task<Worker> GetByFirstName(string firstName)
            => FirstOrDefault(w => w.FirstName == firstName);
    }
}
