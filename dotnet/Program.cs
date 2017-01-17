using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace MehdiMe.HttpHeaderParsingRepro
{
    class Program
    {
        private const int NbIterations = 500;

		// Set the URL to test here
	    private const string Url = "https://sdr.qasalesloft.com/api/users/authenticate";

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Console.WriteLine("Will be making {0} POST requests to {1}. Press Enter to start.", NbIterations, Url);

            var nbProtocolViolations = 0;
            var nbOtherErrors = 0;

            for (int i = 1; i <= NbIterations; i++)
            {
                Console.Write("Try #{0}...  ", i);

                try
                {
                    HttpWebRequest request = BuildWebRequest();
                        
                    var response = request.GetResponse() as HttpWebResponse;
                    var responseContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
	                Console.Write("Success - " + response.StatusCode);
                }
                catch (WebException e)
                {
                    if (e.Message ==
                        "The server committed a protocol violation. Section=ResponseHeader Detail=CR must be followed by LF")
                    {
                        nbProtocolViolations++;
                        Console.Write(e);
                    }
                    else if (e.Response is HttpWebResponse && ((HttpWebResponse) e.Response).StatusCode >= HttpStatusCode.BadRequest)
                    {
						Console.Write("Success - " + ((HttpWebResponse)e.Response).StatusCode);
                    }
                    else
                    {
                        nbOtherErrors++;
                        Console.Write(e);
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    nbOtherErrors++;
                }

                Console.WriteLine();
            }

            Console.WriteLine(
                "The end. Number of tries: {0}. Number of protocol violations: {1}. Number of other errors: {2}. Press Enter to exit.",
                NbIterations, nbProtocolViolations, nbOtherErrors);
            Console.ReadLine();
        }

        private static string ReadEmbeddedResource(string resourceName)
        {
            using (var bodyStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(bodyStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

		private static HttpWebRequest BuildWebRequest()
        {
            var request = WebRequest.Create(Url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ServicePoint.Expect100Continue = false;

            var body = ReadEmbeddedResource("MehdiMe.HttpHeaderParsingRepro.Body.json");
            request.ContentLength = body.Length;

            using (var requestStream = request.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(body);
                }
            }

            return request;
        }
    }
}