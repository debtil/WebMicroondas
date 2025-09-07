using Microsoft.AspNetCore.Mvc.Testing;

namespace WebMicroondasTest.Integration.Helpers
{
    public static class MockHelper
    {
        public static HttpClient GetClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }
    }
}
