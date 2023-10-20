using Microsoft.AspNetCore.Mvc;
using PeachMvc.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text;
using System;
using System.Transactions;

namespace PeachMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        Dictionary<string, object> paymentDetails = new Dictionary<string, object>
        {
            { "checkoutId", "" },
            { "registrationId", "" },
            { "amount", 20 }
        };
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            // Prepare the data for the Peach Payments request
            string data = "entityId=8ac7e0086c5330023" +
                "&amount=92.00" +
                "&currency=ZAR" +
                "&paymentType=DB" +
                "&createRegistration=true" +
                "&standingInstruction.type=RECURRING" +
                "&standingInstruction.source=CIT" +
                "&standingInstruction.mode=INITIAL";
            string url = "https://eu-test.oppwa.com/v1/checkouts"; // Specify the correct endpoint
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Create and configure the HTTP request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer OGFjN2E0Y2JzOXhtTUpDUg==";
            request.ContentType = "application/x-www-form-urlencoded";

            // Send the request and handle the response
            try
            {
                Stream postData = request.GetRequestStream();
                postData.Write(buffer, 0, buffer.Length);
                postData.Close();

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    // Deserialize the JSON response using System.Text.Json
                    var responseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());


                    var checkoutId = responseData.ElementAt(4).Value;
                    Console.WriteLine(checkoutId);



                    reader.Close();
                    dataStream.Close();

                    ViewBag.CheckoutId = checkoutId;

                    paymentDetails["checkoutId"] = "checkoutId";

                    // Return the response from Peach Payments

                    return View();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        public Dictionary<string, dynamic> GetPaymentStatus(string resourcePath)
        {
            try
            {
                Dictionary<string, dynamic> responseData;

                string ResourcePath = resourcePath;
                string data = "entityId=8ac7a4ca8a227686c5330023";
                string url = "https://eu-test.oppwa.com" + ResourcePath + "?" + data;

                Console.WriteLine(url);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = "Bearer OGFjN2E0Y2E4Aw8ZkJzOXhtTUpD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    // Use System.Text.Json for deserialization
                    responseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }
                var registrationId = responseData.ElementAt(1).Value;

                paymentDetails["registrationId"] = responseData.ElementAt(1).Value;
                paymentDetails["initialTransactionId"] = "";

                return paymentDetails;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //8ac7a49f8b4b0e03018b4c074e4d483b
        //Initial TransactionId for James   8ac7a4a28b4b15cc018b4c074f082b07
        public Dictionary<string, dynamic> Request(Dictionary<string, object> paymentDetails)
        {

            Dictionary<string, dynamic> responseData;
            string data = "entityId=8a82941744e78cf26461790" +
                "&amount=92.00" +
                "&currency=ZAR" +
                "&paymentType=PA" +
                "&standingInstruction.mode=REPEATED" +
                "&standingInstruction.type=RECURRING" +
                "&standingInstruction.source=MIT";



            var id = paymentDetails["registrationId"].ToString();
            string url = "https://eu-test.oppwa.com/v1/registrations/" + "id" + "/payments";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer OGFjN2E0Y2E4YTIyN3NzAwMWR8ZkJzOXhtT";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseContent = reader.ReadToEnd();
                responseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(responseContent);
            }
            return responseData;
        }



        //public async Task<Dictionary<string, dynamic>> Request123()
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, "https://eu-test.oppwa.com/v1/registrations/8ac7a49f7c5111c1fe4a57/payments");
        //    request.Headers.Add("Authorization", "Bearer OGE8XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX==");
        //    var collection = new List<KeyValuePair<string, string>>();
        //    collection.Add(new KeyValuePair<string, string>("entityId", "8a82xxxxxxxxxxxxxxxxxxxxxxxxxxxx"));
        //    collection.Add(new KeyValuePair<string, string>("amount", "92.00"));
        //    collection.Add(new KeyValuePair<string, string>("currency", "ZAR"));
        //    collection.Add(new KeyValuePair<string, string>("paymentType", "DB"));
        //    collection.Add(new KeyValuePair<string, string>("standingInstruction.mode", "REPEATED"));
        //    collection.Add(new KeyValuePair<string, string>("standingInstruction.type", "UNSCHEDULED"));
        //    collection.Add(new KeyValuePair<string, string>("standingInstruction.source", "MIT"));
        //    var content = new FormUrlEncodedContent(collection);
        //    request.Content = content;
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(responseContent);

        //    // Parse the responseContent into a Dictionary if needed

        //    // Return the result or null as needed
        //    return null;
        //}




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}






