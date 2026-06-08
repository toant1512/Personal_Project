using FluentAssertions;
using MediaArchive.Application.Exceptions;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;
using MediaArchive.Domain.Entities;
using MediaArchive.Infrastructure.Persistence;
using MediaArchive.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MediaArchive.Domain.Enums;

namespace MediaArchive.Tests.Services
{
    public class MediaServiceTests
    {
        private readonly Mock<IMediaMetadataService> metadataService = new Mock<IMediaMetadataService>();
        private readonly Mock<ILogger<MediaService>> logger = new Mock<ILogger<MediaService>>();

        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDuplicateExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var userId = Guid.NewGuid();

            metadataService
                .Setup(x => x.ExtractAsync(It.IsAny<string>()))
                .ReturnsAsync(new MediaMetadataDto
                {
                    Title = "Test Song",
                    Platform = "youtube",
                    DurationSeconds = 120
                });

            dbContext.MediaItems.Add(new MediaItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Existing Song",
                SourceUrl = "https://youtube.com/watch?v=test",
                Platform = "youtube",
                Status = DownloadStatus.Completed,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            var request = new CreateMediaRequest
            {
                SourceUrl = "https://youtube.com/watch?v=test"
            };

            // Act
            Func<Task> act = async () => await service.CreateAsync(userId, request);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();

        }

        [Fact]
        public async Task CreateAsync_ShouldCreateMedia_WhenRequestIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dbContext = CreateDbContext();

            metadataService
                .Setup(x => x.ExtractAsync(It.IsAny<string>()))
                .ReturnsAsync(new MediaMetadataDto
                {
                    Title = "Test Song",
                    Platform = "youtube",
                    DurationSeconds = 120,
                    ThumbnailUrl = "thumbnail.jpg"
                });

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            var request = new CreateMediaRequest
            {
                SourceUrl = "https://youtube.com/watch?v=test"
            };

            // Act
            await service.CreateAsync(userId, request);

            // Assert
            var mediaItem = await dbContext.MediaItems.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
            mediaItem.Should().NotBeNull();
            mediaItem.UserId.Should().Be(userId);
            mediaItem.SourceUrl.Should().Be(request.SourceUrl);
            mediaItem.Title.Should().Be("Test Song");
            mediaItem.Platform.Should().Be("youtube");
            mediaItem.Status.Should().Be(DownloadStatus.Pending);
        }
    }
}
