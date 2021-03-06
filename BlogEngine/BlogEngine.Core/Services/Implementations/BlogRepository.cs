﻿using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogEngine.Core.Data.DatabaseContexts;
using BlogEngine.Core.Data.Entities;
using BlogEngine.Core.Services.Abstractions;
using BlogEngine.Core.Common.Exceptions;

namespace BlogEngine.Core.Services.Implementations
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<IEnumerable<Blog>> GetAllWithReferences()
        {
            return Task.FromResult(_context.Blogs
                 .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
                 .Include(b => b.MainComments).ThenInclude(mc => mc.SubComments)
                 .OrderByDescending(b => b.DateCreated)
                 .ThenByDescending(b => b.LastUpdateDate)
                 .AsEnumerable());
        }

        public override Blog GetById(int id)
        {
            return _context.Blogs
                .Where(b => b.ID.Equals(id))
                .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
                .Include(b => b.MainComments).ThenInclude(mc => mc.SubComments)
                .FirstOrDefault();
        }

        public override async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .Where(b => b.ID.Equals(id))
                .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
                .Include(b => b.MainComments).ThenInclude(mc => mc.SubComments)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Blog>> GetAllByUserIdAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id.Equals(id))
                .Include(u => u.Blogs)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new EntityNotFoundException(nameof(user));
            }

            var blogs = user.Blogs
                .OrderByDescending(b => b.DateCreated)
                .ThenByDescending(b => b.LastUpdateDate);

            return blogs;
        }

        public override Blog Insert(Blog entity)
        {
            entity = base.Insert(entity);

            _context.Entry(entity)
                .Collection(b => b.BlogCategories)
                .Query()
                .Include(b => b.Category)
                .Load();

            return entity;
        }

        public override async Task<Blog> InsertAsync(Blog entity)
        {
            entity = await base.InsertAsync(entity);

            _context.Entry(entity)
                .Collection(b => b.BlogCategories)
                .Query()
                .Include(b => b.Category)
                .Load();

            return entity;
        }

        public override Blog Update(Blog entity)
        {
            entity = base.Update(entity);

            _context.Entry(entity)
                .Collection(b => b.BlogCategories)
                .Query()
                .Include(b => b.Category)
                .Load();

            return entity;
        }

        public override async Task<Blog> UpdateAsync(Blog entity)
        {
            entity = await base.UpdateAsync(entity);

            _context.Entry(entity)
                .Collection(b => b.BlogCategories)
                .Query()
                .Include(b => b.Category)
                .Load();

            return entity;
        }
    }
}