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
            var metadataService = new Mock<IMediaMetadataService>();
            var logger = new Mock<ILogger<MediaService>>();
            var dbContext = CreateDbContext();
            var userId = Guid.NewGuid();

            metadataService.Setup(x => x.ExtractAsync(It.IsAny<string>()))
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
            dbContext.SaveChanges();

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
    }
}
