﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Snippets.Web.Common.Database;

namespace Snippets.Web.Migrations
{
    [DbContext(typeof(SnippetsContext))]
    partial class SnippetsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ChangeDetector.SkipDetectChanges", "true")
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");
#pragma warning restore 612, 618
        }
    }
}
