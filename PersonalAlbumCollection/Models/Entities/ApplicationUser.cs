namespace PersonalAlbumCollection.Models.Entities;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserAlbum> UserAlbums { get; set; }
}