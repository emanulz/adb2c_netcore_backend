using Microsoft.EntityFrameworkCore;

namespace DocumentsApi.Models
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options) : base(options) { }

        public DbSet<DocumentItem> DocumentItems { get; set; } = null!;
    }
}
