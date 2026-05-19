using Microsoft.EntityFrameworkCore;

namespace PersonalAlbumCollection.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}