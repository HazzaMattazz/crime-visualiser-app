using crime_visualiser.HttpClients.Interfaces;
using crime_visualiser.Models;
using crime_visualiser.Services;
using Moq;

namespace crime_visualiser.tests.Services
{
    public class CrimeDataServiceTests
    {
        [Fact]
        public async Task GetCrimeCategoryCountsAsync_ReturnsNull_WhenClientReturnsNull()
        {
            var mockClient = new Mock<IUkCrimeDataClient>();
            mockClient
                .Setup(c => c.GetCrimesAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<DateOnly>()))
                .ReturnsAsync((IEnumerable<CrimeDto>?)null);

            var service = new CrimeDataService(mockClient.Object);

            var result = await service.GetCrimeCategoryCountsAsync(new CrimeQuery { Latitude = 0, Longitude = 0, Date = new DateOnly(2023, 1, 1) });

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCrimeCategoryCountsAsync_GroupsAndCountsCategories_IncludingUnknown()
        {
            var crimes = new List<CrimeDto>
            {
                new CrimeDto { Category = "theft" },
                new CrimeDto { Category = null },
                new CrimeDto { Category = " " },
                new CrimeDto { Category = "theft" },
                new CrimeDto { Category = "assault" }
            };

            var mockClient = new Mock<IUkCrimeDataClient>();
            mockClient
                .Setup(c => c.GetCrimesAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(crimes);

            var service = new CrimeDataService(mockClient.Object);

            var result = await service.GetCrimeCategoryCountsAsync(new CrimeQuery { Latitude = 0, Longitude = 0, Date = new DateOnly(2023, 1, 1) });

            Assert.NotNull(result);
            var list = result!.ToList();

            // Expected categories: "theft" (2), "Unknown" (2 -> null and whitespace), "assault" (1)
            Assert.Equal(3, list.Count);

            var theft = list.FirstOrDefault(x => x.Category == "theft");
            var unknown = list.FirstOrDefault(x => x.Category == "Unknown");
            var assault = list.FirstOrDefault(x => x.Category == "assault");

            Assert.NotNull(theft);
            Assert.Equal(2, theft.Count);

            Assert.NotNull(unknown);
            Assert.Equal(2, unknown.Count);

            Assert.NotNull(assault);
            Assert.Equal(1, assault.Count);
        }
    }
}
