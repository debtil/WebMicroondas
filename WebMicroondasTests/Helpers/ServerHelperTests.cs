using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMicroondasTests.Helpers
{
    public class ServerHelperTests
    {
        public static async Task<HttpClient> GetTestServerClient()
        {
            var application = new WebApplicationFactory<Program>(); // Aqui "Program" é o ponto de entrada da sua API
            var client = application.CreateClient();
            return client;
        }
    }
}
