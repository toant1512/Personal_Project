using FluentAssertions;
using MediaArchive.Application.Common.Models;
using MediaArchive.Application.Exceptions;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;
using MediaArchive.Domain.Entities;
using MediaArchive.Domain.Enums;
using MediaArchive.Infrastructure.Persistence;
using MediaArchive.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

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

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedResults()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dbContext = CreateDbContext();

            for (int i = 1; i <= 5; i++)
            {
                dbContext.MediaItems.Add(new MediaItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = $"Song {i}",
                    Platform = "youtube",
                    SourceUrl = $"https://youtube.com/watch?v={i}",
                    Status = DownloadStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(i)
                });
            }

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            var request = new PagedRequest
            {
                Page = 1,
                PageSize = 2
            };

            // Act
            var result = await service.GetAllAsync(userId, request);

            // Assert
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.TotalPages.Should().Be(3);
            result.Items.ElementAt(0).Title.Should().Be("Song 5");
            result.Items.ElementAt(1).Title.Should().Be("Song 4");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyCurrentUserMedia()
        {
            // Arrange
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();

            var dbContext = CreateDbContext();

            dbContext.MediaItems.Add(new MediaItem
            {
                Id = Guid.NewGuid(),
                UserId = userA,
                Title = "User A Song",
                Platform = "youtube",
                SourceUrl = "https://youtube.com/a",
                Status = DownloadStatus.Completed,
                CreatedAt = DateTime.UtcNow
            });

            dbContext.MediaItems.Add(new MediaItem
            {
                Id = Guid.NewGuid(),
                UserId = userB,
                Title = "User B Song",
                Platform = "youtube",
                SourceUrl = "https://youtube.com/b",
                Status = DownloadStatus.Completed,
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            // Act
            var result = await service.GetAllAsync(userA, new PagedRequest());

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.ElementAt(0).Title.Should().Be("User A Song");
            result.Items.Should().NotContain(x => x.Title == "User B Song");
        }

        [Fact]
        public async Task Search_ShouldFilterByTitle()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dbContext = CreateDbContext();

            dbContext.MediaItems.AddRange(
                new MediaItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Wildfire",
                    Platform = "youtube",
                    SourceUrl = "url1",
                    Status = DownloadStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                },
                new MediaItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Moon Halo",
                    Platform = "youtube",
                    SourceUrl = "url2",
                    Status = DownloadStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                },
                new MediaItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Hope Is The Thing With Feathers",
                    Platform = "youtube",
                    SourceUrl = "url3",
                    Status = DownloadStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            // Act
            var result = await service.GetAllAsync(userId,
                new PagedRequest
                {
                    Search = "wild"
                });

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.ElementAt(0).Title.Should().Be("Wildfire");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenUserHasNoMedia()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var dbContext = CreateDbContext();

            var service = new MediaService(dbContext, metadataService.Object, logger.Object);

            // Act
            var result = await service.GetAllAsync(userId, new PagedRequest());

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
    }
}
