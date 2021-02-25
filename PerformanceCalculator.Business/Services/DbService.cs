﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PerformanceCalculator.Business.DbContexts;
using PerformanceCalculator.Business.Services.Interfaces;
using PerformanceCalculator.Business.Specifications;
using PerformanceCalculator.Common.Models;

namespace PerformanceCalculator.Business.Services
{
    public class DbService<T> : IDbService<T> where T : BaseModel
    {
        private readonly ApplicationDbContext _context;

        public DbService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task CreateAsync(T model)
        {
            _context.Set<T>().Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T model)
        {
            var entity = await _context.Set<T>().FindAsync(model.Id);
            _context.Entry(entity).CurrentValues.SetValues(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T model)
        {
            _context.Set<T>().Remove(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsExists(Guid id)
        {
            return await _context.Set<T>().AnyAsync(m => m.Id == id);
        }

        public async Task<T> GetModelWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).AsSingleQuery().ToListAsync();
        }

        public T GetByStorageProcedure(string procedure, string args)
        {
            return _context.Set<T>().FromSqlRaw<T>("{0} {1}", procedure, args).ToList().FirstOrDefault();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
        }
    }
}