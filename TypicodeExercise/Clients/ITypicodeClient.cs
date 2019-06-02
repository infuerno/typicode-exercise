using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypicodeExercise.Models;

namespace TypicodeExercise.Clients
{
    public interface ITypicodeClient
    {
        Task<List<Album>> GetAlbumsAsync();
        Task<List<Album>> GetAlbumsByUserIdAsync(int userId);
        Task<List<Photo>> GetPhotosAsync();
        Task<List<Photo>> GetPhotosByAlbumIdAsync(int albumId);
    }
}
