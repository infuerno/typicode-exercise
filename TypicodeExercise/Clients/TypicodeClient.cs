using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Serilog;
using TypicodeExercise.Controllers;
using TypicodeExercise.Models;

namespace TypicodeExercise.Clients
{
    /// <summary>
    /// This class uses an instance of HttpClient, rather than a static HttpClient
    /// It is therefore designed to be used / registered as a singleton
    /// </summary>
    public class TypicodeClient : ITypicodeClient
    {
        private readonly HttpClient _httpClient;
        private static readonly ILogger _log = Serilog.Log.Logger.ForContext<TypicodeClient>();

        public TypicodeClient()
        {
            string configSettingName = "TypicodeApi.BaseUrl";
            var baseUrl = ConfigurationManager.AppSettings[configSettingName];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ConfigurationErrorsException($"Cannot find app setting '{configSettingName}', please ensure this is correctly configured");
            }
            _httpClient = new HttpClient() { BaseAddress = new Uri(baseUrl) };
            _log.Debug("Successfully initialised HttpClient with base url {BaseUrl}", baseUrl);
        }

        public async Task<List<Album>> GetAlbumsAsync()
        {
            return await GetResourceAsync<List<Album>>("albums");
        }

        public async Task<List<Album>> GetAlbumsByUserIdAsync(int userId)
        {
            return await GetResourceAsync<List<Album>>($"albums/userId={userId}");
        }

        public async Task<List<Photo>> GetPhotosAsync()
        {
            return await GetResourceAsync<List<Photo>>("photos");
        }

        public async Task<List<Photo>> GetPhotosByAlbumIdAsync(int albumId)
        {
            return await GetResourceAsync<List<Photo>>($"photos/albumId={albumId}");
        }

        private async Task<T> GetResourceAsync<T>(string url)
        {
            _log.Debug("Calling Typicode API url {Url}", _httpClient.BaseAddress + url);
            using (var response = await _httpClient.GetAsync(url))
            {
                _log.Debug("Received response, checking success");
                response.EnsureSuccessStatusCode();
                _log.Debug("Reading contents of response");
                var content = await response.Content.ReadAsStringAsync();
                _log.Debug("Deserializing to type {Type}", typeof(T));
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}