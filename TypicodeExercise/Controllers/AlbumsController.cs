using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Serilog;
using TypicodeExercise.Clients;
using TypicodeExercise.Models;

namespace TypicodeExercise.Controllers
{
    public class AlbumsController : ApiController
    {
        private readonly ITypicodeClient _client;
        private static readonly ILogger _log = Serilog.Log.Logger.ForContext<AlbumsController>();

        public AlbumsController(ITypicodeClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        // GET: api/albums
        public async Task<IHttpActionResult> Get()
        {
            _log.Debug("Endpoint api/albums called, retrieving data");
            List<Album> albums;
            try
            {
                albums = await GetAllAlbumsAndPhotos();
            }
            catch (Exception ex)
            {
                var message = "Error occured trying to retrieve all albums and photos";
                _log.Error(ex, $"{message}. See exception for details.");
                return InternalServerError(new Exception(message));
            }

            if (albums == null)
            {
                Log.Debug("Null response retrieving all albums and photos, returning NotFound");
                return NotFound();
            }
            _log.Debug("Successfully retrieved and combined, returning data");
            return Ok(albums);
        }


        // GET: api/albums?userId=5
        public async Task<IHttpActionResult> Get(int userId)
        {
            _log.Debug("Endpoint api/albums called with user id {UserId}, retrieving data", userId);
            if (userId <= 0)
            {
                Log.Debug("Invalid userId, returning BadRequest", userId);
                return BadRequest("Value for parameter userId must be greater than 0");
            }

            List<Album> albums = null;
            try
            {
                albums = await GetAlbumsAndPhotosForUserId(userId);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("404 (Not Found)"))
                {
                    var message = $"Error occured trying to retrieve albums and photos for user id {userId}";
                    _log.Error(ex, $"{message}. See exception for details.");
                    return InternalServerError(new Exception(message));
                }
            }

            if (albums == null)
            {
                Log.Debug("Null response retrieving albums for user id {UserId} client, returning NotFound", userId);
                return NotFound();
            }
            Log.Debug("Successfully retrieved, returning data");
            return Ok(albums);
        }

        private async Task<List<Album>> GetAllAlbumsAndPhotos()
        {
            var albums = await _client.GetAlbumsAsync();
            if (albums != null)
            {
                var photos = await _client.GetPhotosAsync();
                Combine(albums, photos);
            }

            return albums;
        }

        private async Task<List<Album>> GetAlbumsAndPhotosForUserId(int userId)
        {
           var albums = await _client.GetAlbumsByUserIdAsync(userId);
           foreach (var album in albums)
           {
               album.Photos = await _client.GetPhotosByAlbumIdAsync(album.Id);
           }

           return albums;
        }

        private void Combine(List<Album> albums, List<Photo> photos)
        {
            if (photos == null)
                return;

            foreach (var album in albums)
            {
                album.Photos = photos.FindAll(p => p.AlbumId == album.Id);
            }
        }
    }

}