using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using crime_visualiser.HttpClients;
using crime_visualiser.Models;

namespace crime_visualiser.tests.HttpClients
{
    public class UkCrimeDataClientTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;
            public HttpRequestMessage? LastRequest { get; private set; }

            public FakeHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(_response);
            }
        }

        [Fact]
        public async Task GetCrimesAsync_ReturnsData_WhenResponseIsSuccess()
        {
            var json = "[ { \"category\": \"theft\", \"id\": 1 }, { \"category\": \"assault\", \"id\": 2 } ]";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new FakeHandler(response);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test/") };

            var client = new UkCrimeDataClient(httpClient, NullLogger<UkCrimeDataClient>.Instance);

            var result = await client.GetCrimesAsync(51.5, -0.1, DateOnly.FromDateTime(DateTime.Today));

            Assert.NotNull(result);

            var list = new List<CrimeDto>(result!);
            Assert.Equal(2, list.Count);
            Assert.Contains(list, d => d.Category == "theft");
            Assert.Contains(list, d => d.Category == "assault");

            // Ensure the expected endpoint was called (date formatted as yyyy-MM)
            Assert.NotNull(handler.LastRequest);
            Assert.Contains("crimes-street/all-crime", handler.LastRequest!.RequestUri!.PathAndQuery);
        }

        [Fact]
        public async Task GetCrimesAsync_ReturnsNull_WhenResponseIsServerError()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var handler = new FakeHandler(response);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test/") };

            var client = new UkCrimeDataClient(httpClient, NullLogger<UkCrimeDataClient>.Instance);

            var result = await client.GetCrimesAsync(51.5, -0.1, DateOnly.FromDateTime(DateTime.Today));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCrimesAsync_ReturnsNull_WhenParametersInvalid()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };

            var handler = new FakeHandler(response);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test/") };

            var client = new UkCrimeDataClient(httpClient, NullLogger<UkCrimeDataClient>.Instance);

            // Invalid latitude (> 90)
            var result = await client.GetCrimesAsync(120, 0, DateOnly.FromDateTime(DateTime.Today));
            Assert.Null(result);

            // Invalid longitude (< -180)
            result = await client.GetCrimesAsync(0, -200, DateOnly.FromDateTime(DateTime.Today));
            Assert.Null(result);

            // Default date
            result = await client.GetCrimesAsync(0, 0, default);
            Assert.Null(result);
        }
    }
}
