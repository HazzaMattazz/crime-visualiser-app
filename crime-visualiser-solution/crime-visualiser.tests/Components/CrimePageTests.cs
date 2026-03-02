using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using crime_visualiser.Components.Pages;
using crime_visualiser.Models;
using crime_visualiser.Services.Interfaces;

namespace crime_visualiser.tests.Components
{
    public class CrimePageTests
    {
        [Fact]
        public async Task Search_ShowsNoDataMessage_WhenServiceReturnsNull()
        {
            using var ctx = new BunitContext();

            var mockService = new Mock<ICrimeDataService>();
            mockService
                .Setup(s => s.GetCrimeCategoryCountsAsync(It.IsAny<CrimeQuery>()))
                .ReturnsAsync((IEnumerable<CrimeCategoryCount>?)null);

            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(NullLogger<CrimePage>.Instance);

            var cut = ctx.Render<CrimePage>();

            // Click the search button
            var button = cut.Find("button");
            button.Click();

            cut.WaitForAssertion(() => mockService.Verify(s => s.GetCrimeCategoryCountsAsync(It.IsAny<CrimeQuery>()), Times.Once));

            // Expect the no-data message to be present
            cut.WaitForAssertion(() => Assert.Contains("No crime data found", cut.Markup));
        }

        [Fact]
        public async Task Search_DisplaysTable_WhenServiceReturnsCounts()
        {
            using var ctx = new BunitContext();

            var counts = new List<CrimeCategoryCount>
            {
                new CrimeCategoryCount("theft", 3),
                new CrimeCategoryCount("assault", 1)
            };

            var mockService = new Mock<ICrimeDataService>();
            mockService
                .Setup(s => s.GetCrimeCategoryCountsAsync(It.IsAny<CrimeQuery>()))
                .ReturnsAsync(counts);

            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(NullLogger<CrimePage>.Instance);

            var cut = ctx.Render<CrimePage>();

            var button = cut.Find("button");
            button.Click();

            cut.WaitForAssertion(() => Assert.Contains("Crime counts by category", cut.Markup));
            cut.WaitForAssertion(() => Assert.Contains("theft", cut.Markup));
            cut.WaitForAssertion(() => Assert.Contains("3", cut.Markup));
            cut.WaitForAssertion(() => Assert.Contains("assault", cut.Markup));
            cut.WaitForAssertion(() => Assert.Contains("1", cut.Markup));
        }

        [Fact]
        public async Task Search_DoesNotCallService_WhenValidationFails()
        {
            using var ctx = new BunitContext();

            var mockService = new Mock<ICrimeDataService>();
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(NullLogger<CrimePage>.Instance);

            var cut = ctx.Render<CrimePage>();

            // Set the date input to a future date so validation fails
            var dateInput = cut.Find("input[type='date']");
            var future = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
            dateInput.Change(future.ToString("yyyy-MM-dd"));

            var button = cut.Find("button");
            button.Click();

            // Service should not have been called because validation fails
            cut.WaitForAssertion(() => mockService.Verify(s => s.GetCrimeCategoryCountsAsync(It.IsAny<CrimeQuery>()), Times.Never));

            // Expect a validation message to be shown
            cut.WaitForAssertion(() => Assert.Contains("must be less than or equal to", cut.Markup, StringComparison.OrdinalIgnoreCase));
        }
    }
}
