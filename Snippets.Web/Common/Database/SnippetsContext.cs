﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common.Database
{
    public class SnippetsContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Karma> Karma { get; set; }

        public DbSet<SnippetCategory> SnippetCategories { get; set; }

        public SnippetsContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Snippets and Category pivot table builder
            modelBuilder.Entity<SnippetCategory>(sc =>
            {
                sc.HasKey(t => new
                {
                    t.SnippetId, t.CategoryId
                });

                sc.HasOne(pt => pt.Snippet)
                    .WithMany(p => p.SnippetCategories)
                    .HasForeignKey(pt => pt.SnippetId);

                sc.HasOne(pt => pt.Category)
                    .WithMany(p => p.SnippetCategories)
                    .HasForeignKey(pt => pt.CategoryId);
            });

            modelBuilder.Entity<Person>(p =>
            {
                #region User Preferences Notice
                /* 
                 * Make sure to manually update your 
                 * entity after making changes to the person's preferences, e.g.:
                 * 
                 * person.Preferences.IsProfilePublic = true;
                 * _context.Person.Update(person);
                 * _context.SaveChanges();
                 *
                 */
                #endregion

                p.Property(u => u.Preferences)
                    .HasConversion(
                        x => JsonConvert.SerializeObject(x), 
                        x => JsonConvert.DeserializeObject<UserPreferences>(x));
            });
        }
    }
}
