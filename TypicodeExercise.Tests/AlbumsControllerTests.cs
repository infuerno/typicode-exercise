using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TypicodeExercise.Clients;
using TypicodeExercise.Controllers;
using TypicodeExercise.Models;

namespace TypicodeExercise.Tests
{
    [TestClass]
    public class AlbumsControllerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NoTypicodeClientInjected_ArgumentNullExceptionThrown()
        {
            new AlbumsController(null);
        }

        #region Get
        [TestMethod]
        public void Get_ReturnsNonNullResponse()
        {
            var mockClient = new Mock<ITypicodeClient>();
            var controller = new AlbumsController(mockClient.Object);
            
            var result = controller.Get().GetAwaiter().GetResult();

            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Get_AlbumsFound_ReturnsSuccessResponse()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult(new List<Album>()));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            contentResult.Should().NotBeNull();
        }

        [TestMethod]
        public void Get_NoAlbumsFound_ReturnsNotFoundResponse()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult<List<Album>>(null));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as NotFoundResult;

            contentResult.Should().NotBeNull();
        }

        [TestMethod]
        public void Get_ReturnsAlbumsAndPhotosCombined()
        {
            // setup
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult(GetMockAlbums()));
            mockClient.Setup(client => client.GetPhotosAsync()).Returns(Task.FromResult(GetMockPhotos()));

            var controller = new AlbumsController(mockClient.Object);

            // act
            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            // assert
            contentResult.Should().NotBeNull();
            var albums = contentResult.Content as List<Album>;
            albums.Should().NotBeNull();
            albums.Count.Should().Be(2);
            albums[0].Photos.Should().NotBeNull();
            albums[0].Photos.Count().Should().Be(2);
            albums[1].Photos.Should().NotBeNull();
            albums[1].Photos.Count().Should().Be(3);
        }

        [TestMethod]
        public void Get_ReturnsAlbumsAndPhotosAllAttributesPopulated()
        {
            // setup
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult(GetMockAlbums()));
            mockClient.Setup(client => client.GetPhotosAsync()).Returns(Task.FromResult(GetMockPhotos()));
            var controller = new AlbumsController(mockClient.Object);

            // act
            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            // assert
            contentResult.Should().NotBeNull();
            var albums = contentResult.Content as List<Album>;
            albums.Should().NotBeNull();
            albums.Count.Should().Be(2);
            albums[0].Id.Should().Be(1);
            albums[0].Title.Should().Be("Album1");
            albums[0].UserId.Should().Be(1);
            albums[0].Photos.Count().Should().Be(2);
            albums[0].Photos[0].Id.Should().Be(1);
            //albums[0].Photos[0].AlbumId.Should().Be(1); // get accessor is internal - property won't be serialized
            albums[0].Photos[0].Title.Should().Be("Photo1");
            albums[0].Photos[0].Url.Should().Be("http://photo1");
            albums[0].Photos[0].ThumbnailUrl.Should().Be("http://thumb1");
        }

        [TestMethod]
        public void Get_WhenNoPhotos_ReturnsEmptyAlbums()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult(GetMockAlbums()));
            mockClient.Setup(client => client.GetPhotosAsync()).Returns(Task.FromResult<List<Photo>>(null));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            contentResult.Should().NotBeNull();
            var albums = contentResult.Content as List<Album>;
            albums.Should().NotBeNull();
            albums.Count.Should().Be(2);
            albums[0].Photos.Should().BeNull();
            albums[1].Photos.Should().BeNull();
        }

        [TestMethod]
        public void Get_WhenPhotosButNoAlbums_ReturnsNotFound()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsAsync()).Returns(Task.FromResult<List<Album>>(null));
            mockClient.Setup(client => client.GetPhotosAsync()).Returns(Task.FromResult(GetMockPhotos()));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get().GetAwaiter().GetResult();
            var contentResult = actionResult as NotFoundResult;

            contentResult.Should().NotBeNull();
        }
        #endregion

        #region GetByUserId
        [TestMethod]
        public void GetByUserId_ResponseNotNull()
        {
            var mockClient = new Mock<ITypicodeClient>();
            var controller = new AlbumsController(mockClient.Object);

            var result = controller.Get(0).GetAwaiter().GetResult();

            result.Should().NotBeNull();
        }

        [TestMethod]
        public void GetByUserId_UserIdNotValid_ReturnsBadRequest()
        {
            var mockClient = new Mock<ITypicodeClient>();
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get(0).GetAwaiter().GetResult();
            var contentResult = actionResult as BadRequestErrorMessageResult;

            contentResult.Should().NotBeNull();
        }
        [TestMethod]
        public void GetByUserId_AlbumsFound_ReturnsSuccessResponse()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsByUserIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new List<Album>()));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get(1).GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            contentResult.Should().NotBeNull();
        }

        [TestMethod]
        public void GetByUserId_NoAlbumsFound_ReturnsNotFoundResponse()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsByUserIdAsync(It.IsAny<int>())).Throws(new Exception("404 (Not Found)"));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get(1).GetAwaiter().GetResult();
            var contentResult = actionResult as NotFoundResult;

            contentResult.Should().NotBeNull();
        }

        [TestMethod]
        public void GetByUserId_ClientThrowsException_ReturnsInternalServerError()
        {
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsByUserIdAsync(It.IsAny<int>())).Throws(new Exception("Some error"));
            var controller = new AlbumsController(mockClient.Object);

            var actionResult = controller.Get(1).GetAwaiter().GetResult();
            var contentResult = actionResult as ExceptionResult;

            contentResult.Should().NotBeNull();
        }

        [TestMethod]
        public void GetByUserId_ReturnsAlbumWithPhotos()
        {
            // setup
            var mockClient = new Mock<ITypicodeClient>();
            mockClient.Setup(client => client.GetAlbumsByUserIdAsync(1)).Returns(Task.FromResult(GetMockAlbums().FindAll(a => a.UserId == 1)));
            mockClient.Setup(client => client.GetPhotosByAlbumIdAsync(1)).Returns(Task.FromResult(GetMockPhotos().FindAll(p => p.Id == 1 || p.Id == 2)));

            var controller = new AlbumsController(mockClient.Object);

            // act
            var actionResult = controller.Get(1).GetAwaiter().GetResult();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Album>>;

            // assert
            contentResult.Should().NotBeNull();
            var albums = contentResult.Content as List<Album>;
            albums.Should().NotBeNull();
            albums.Count.Should().Be(1);
            albums[0].Photos.Should().NotBeNull();
            albums[0].Photos.Count().Should().Be(2);
        }

        #endregion

        #region Private Methods

        private List<Album> GetMockAlbums()
        {
            return new List<Album>
            {
                new Album() {Id = 1, Title = "Album1", UserId = 1},
                new Album() {Id = 2, Title = "Album2", UserId = 2}
            };
        }

        private List<Photo> GetMockPhotos()
        {
            return new List<Photo>
            {
                new Photo() {Id = 1, AlbumId = 1, Title = "Photo1", Url= "http://photo1", ThumbnailUrl = "http://thumb1"},
                new Photo() {Id = 2, AlbumId = 1, Title = "Photo2", Url= "http://photo2", ThumbnailUrl = "http://thumb2"},
                new Photo() {Id = 3, AlbumId = 2, Title = "Photo3", Url= "http://photo3", ThumbnailUrl = "http://thumb3"},
                new Photo() {Id = 4, AlbumId = 2, Title = "Photo4", Url= "http://photo4", ThumbnailUrl = "http://thumb4"},
                new Photo() {Id = 5, AlbumId = 2, Title = "Photo5", Url= "http://photo5", ThumbnailUrl = "http://thumb5"},
            };
        }

        #endregion
    }
}
