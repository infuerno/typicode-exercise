using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace TypicodeExercise.Models
{
    public class Photo
    {
        public int Id { get; set; }
        // AlbumId should be used in Deserialization, but not in Serialization when emitting final results
        // Adding "internal" achieves this
        // See https://stackoverflow.com/questions/11564091/making-a-property-deserialize-but-not-serialize-with-json-net
        // TODO: if requiring flexibility, use Custom Serializer instead
        public int AlbumId { internal get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}