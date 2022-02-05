using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NSubstitute;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.DI;
using SevenAssignmentLibrary.Exceptions;
using SevenAssignmentLibrary.Settings;
using Xunit;

namespace SevenAssignment.Tests.Unit    
{
    public class SevenClientTests
    {

        [Fact]
        public async Task Should_Return_Users_When_Successful()
        {
            var dummyResponse = File.ReadAllText("./TestData/Response.json");
            var handler = GetHttpMessageHandler(HttpStatusCode.OK, dummyResponse);

            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("http://www.test.com");

            var settings = Substitute.For<SevenAssignmentClientSettings>();
            var logger = Substitute.For<ILogger<ISevenClient>>();

            var sevenClient = new SevenClient(client, settings, logger);

            var response = await sevenClient.GetUsersAsync();

            response.Count.Should().Be(4);

        }

        [Theory]
        [ClassData(typeof(ClientBadStatusCodes))]
        public async Task Should_Throw_Exception_When_Not_Successful(HttpStatusCode statusCode)
        {
            var handler = GetHttpMessageHandler(statusCode, string.Empty);

            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("http://www.test.com");

            var settings = Substitute.For<SevenAssignmentClientSettings>();
            var logger = Substitute.For<ILogger<ISevenClient>>();

            var sevenClient = new SevenClient(client, settings, logger);

            await Assert.ThrowsAsync<SevenAssignmentException>(() => sevenClient.GetUsersAsync());

        }

        private Mock<HttpMessageHandler> GetHttpMessageHandler(HttpStatusCode statusCode, string dummyData)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(dummyData)
                })
                .Verifiable();

            return handlerMock;
        }
    }

    public class ClientBadStatusCodes : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { HttpStatusCode.BadGateway  }; 
            yield return new object[] { HttpStatusCode.GatewayTimeout }; 
            yield return new object[] { HttpStatusCode.ServiceUnavailable };
            yield return new object[] { HttpStatusCode.InternalServerError };
            yield return new object[] { HttpStatusCode.BadRequest };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
