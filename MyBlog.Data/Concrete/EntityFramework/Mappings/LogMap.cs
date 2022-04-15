using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Data.Concrete.EntityFramework.Mappings
{
    public class LogMap:IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedOnAdd();
            builder.Property(v => v.MachineName).IsRequired();
            builder.Property(v => v.MachineName).HasMaxLength(50);
            builder.Property(v => v.Logged).IsRequired();
            builder.Property(v => v.Level).IsRequired();
            builder.Property(v => v.Level).HasMaxLength(50);
            builder.Property(v => v.Message).IsRequired();
            builder.Property(v => v.Message).HasColumnType("NVARCHAR(MAX)");
            builder.Property(v => v.Logger).IsRequired(false);
            builder.Property(v => v.Logger).HasMaxLength(250);
            builder.Property(v => v.Callsite).IsRequired(false);
            builder.Property(v => v.Callsite).HasColumnType("NVARCHAR(MAX)");
            builder.Property(v => v.Exception).IsRequired(false);
            builder.Property(v => v.Exception).HasColumnType("NVARCHAR(MAX)");

            builder.ToTable("Logs");
        }
    }
}
