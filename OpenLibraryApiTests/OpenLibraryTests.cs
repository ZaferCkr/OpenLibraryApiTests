using NUnit.Framework;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace InsurancePlatformTests
{
    public class OpenLibraryTests
    {
        [Test]
        public async Task SearchBooks_ReturnsSuccess()
        {
            var client = new RestClient("https://openlibrary.org");
            var request = new RestRequest("/search.json?q=harry+potter", Method.Get);

            var response = await client.ExecuteAsync(request);

            Assert.That(response.IsSuccessful, Is.True, "API isteği başarısız!");

        }
        [Test]
        public async Task SearchBooks_ReturnsBooksData()
        {
            var client = new RestClient("https://openlibrary.org");
            var request = new RestRequest("/search.json?q=harry+potter", Method.Get);

            var response = await client.ExecuteAsync(request);

            Assert.That(response.IsSuccessful, Is.True, "API isteği başarısız!");

            string content = response.Content;
            string responseBody = content;
            Assert.That(responseBody, Is.Not.Null, "Response body null geldi!");
            Assert.That(responseBody, Is.Not.Empty, "Response body boş geldi!");

            TestContext.WriteLine(responseBody);
        }
        [Test]
        public async Task SearchBooks_ShouldContainAtLeastOneBookTitle()
        {
            var client = new RestClient("https://openlibrary.org");
            var request = new RestRequest("search.json");
            request.AddParameter("q", "harry potter");

            var response = await client.ExecuteAsync(request);

            NUnit.Framework.Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = response.Content;
            NUnit.Framework.Assert.That(json, Is.Not.Null.And.Not.Empty);

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            // Kitap sayısını kontrol et
            int numFound = data.numFound;
            NUnit.Framework.Assert.That(numFound, Is.GreaterThan(0), "Hiç kitap bulunamadı!");

            // İlk kitabın başlığını al ve yazdır
            string firstTitle = data.docs[0].title;
            TestContext.WriteLine("İlk Kitap Başlığı: " + firstTitle);

            NUnit.Framework.Assert.That(firstTitle, Is.Not.Null.And.Not.Empty);
        }
        [Test]
        [TestCase("harry potter")]
        [TestCase("lord of the rings")]
        [TestCase("game of thrones")]
        public async Task SearchBooks_ShouldReturnDataAndLog_FixedPath(string query)
        {
            var client = new RestClient("https://openlibrary.org");
            var request = new RestRequest("search.json");
            request.AddParameter("q", query);

            var response = await client.ExecuteAsync(request);

            NUnit.Framework.Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = response.Content;
            NUnit.Framework.Assert.That(json, Is.Not.Null.And.Not.Empty);

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            int numFound = data.numFound;
            NUnit.Framework.Assert.That(numFound, Is.GreaterThan(0), $"Hiç kitap bulunamadı: {query}");

            string firstTitle = data.docs[0].title;
            TestContext.WriteLine($"Query: {query} → İlk Kitap Başlığı: {firstTitle}");

            // Sabit ve erişimi kolay log dosyası
            string logFileName = "API_Test_Log.txt";
            string projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string logPath = Path.Combine(projectDir, logFileName);

            File.AppendAllText(logPath, $"Query: {query} → İlk Kitap Başlığı: {firstTitle}" + Environment.NewLine);

            NUnit.Framework.Assert.That(firstTitle, Is.Not.Null.And.Not.Empty);
        }

    }

}


