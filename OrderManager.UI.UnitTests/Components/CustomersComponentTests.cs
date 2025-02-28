using Bunit;
using Microsoft.Extensions.DependencyInjection;
using OrderManager.UI.Components;
using OrderManager.UI.Models;
using OrderManager.UI.UnitTests.Common;
using Shouldly;
using System.Text.Json;

namespace OrderManager.UI.UnitTests.Components
{
    public class CustomersComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange
            _httpMessageHandlerMock.SetResponse($"{BASE_URL_ADDRESS}/api/customers", "GET", new HttpResponseMessage
            {
                Content = CreateStringContent(Array.Empty<CustomerDTO>())
            });

            // Act
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();

            // Assert
            customersComponent.ShouldNotBeNull();
            customersComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        private StringContent CreateStringContent<T>(T data)
        {
            return new StringContent(JsonSerializer.Serialize(data));
        }

        private readonly TestContext _testContext;
        private readonly HttpMessageHandlerMock _httpMessageHandlerMock;
        private const string BASE_URL_ADDRESS = "http://localhost";

        public CustomersComponentTests()
        {
            _testContext = new ConfiguredTestContext();
            _httpMessageHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(_httpMessageHandlerMock)
            {
                BaseAddress = new Uri(BASE_URL_ADDRESS)
            };
            _testContext.Services.AddScoped((_) => httpClient);
        }

        public class HttpMessageHandlerMock : HttpMessageHandler
        {
            private string? _requestUrl;
            private string? _methodName;
            private HttpResponseMessage? _response;
            private HttpResponseMessage? _defaultResponse;

            public void SetResponse(HttpResponseMessage response)
            {
                _response = response;
            }

            public void SetResponse(string requestUrl, string methodName, HttpResponseMessage response)
            {
                _requestUrl = requestUrl;
                _methodName = methodName;
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                if (request.RequestUri?.ToString() == _requestUrl && request.Method.Method == _methodName)
                {
                    return Task.FromResult(_response ?? new HttpResponseMessage());
                }

                return Task.FromResult(_defaultResponse ?? new HttpResponseMessage());
            }
        }
    }
}
