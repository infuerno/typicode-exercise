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
        // GET: api/albums?userId=5
        public async Task<IHttpActionResult> Get(int? userId = null)
        {
            _log.Debug("Endpoint api/albums called with user id {UserId}, retrieving data", userId);
            if (userId.HasValue && userId <= 0)
            {
                _log.Debug("Invalid userId, returning BadRequest", userId);
                return BadRequest("Value for parameter userId must be greater than 0");
            }

            List<Album> albums = null;
            try
            {
                if (userId.HasValue)
                    albums = await GetAlbumsAndPhotosForUserId(userId.Value);
                else
                    albums = await GetAllAlbumsAndPhotos();
            }
            catch (Exception ex)
            {
                var message = $"Error occured retrieving albums and photos";
                if (userId != null) message += $" for user id {userId}";
                _log.Error(ex, $"{message}. See exception for details.");
                return InternalServerError(new Exception(message));
            }

            if (albums == null)
            {
                // no reason albums should be null, will have empty list if userId not found, this is an error scenario
                var message = $"Null response retrieving albums and photos";
                if (userId != null) message += $" for user id {userId}";
                _log.Warning($"{message}. Returning exception result.");
                return InternalServerError(new Exception(""));
            }
            _log.Debug("Successfully retrieved, returning data");
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