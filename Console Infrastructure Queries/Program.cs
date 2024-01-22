
/*

   You need the following in a separate file called Secrets.cs:

   public class SecretValues
   {

       public static string EndpointDomain = "relone.yourdomain.com";
       public static Uri ProxyURI = new("http://proxy.yourdomain.com:8080");
       public static string APIBaseAddress = new("https://relone.yourdomain.com/");

       public static string CookieString = "Made you look";

       public static string Env = either "Home" or "Work"; // (delete as appropriate)
   }

*/


using System.Net;

namespace Console_Infrastructure_Queries
{
    public class Link_Model
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    internal class Program
    {

        static void Main()
        {
            PickOption().Wait();
        }


        public static int resourcePoolID = 123456; // NB. This needs updating

        public static List<Link_Model> data_model = new List<Link_Model> {

            new() { Name = "Ping: ", Link = "Relativity.REST/api/Relativity.Services.Environmental.IEnvironmentModule/Ping%20Service/Ping" },
            new() { Name = "Resource Pools: ", Link = "Relativity.Rest/API/relativity-environment/v1/workspace/eligible-resource-pools" },
            new() { Name = "SQL Servers: ", Link = $"Relativity.Rest/API/relativity-environment/v1/workspace/eligible-resource-pools/{resourcePoolID}/eligible-sql-servers" },
            new() { Name = "File Repositories: ", Link = $"Relativity.Rest/API/relativity-environment/v1/workspace/eligible-resource-pools/{resourcePoolID}/eligible-file-repositories" },
            new() { Name = "Cache: ", Link = $"Relativity.Rest/API/relativity-environment/v1/workspace/eligible-resource-pools/{resourcePoolID}/eligible-cache-locations" },
            new() { Name = "URL Handler: ", Link = "Relativity.Rest/API/relativity-environment/v1/workspace/default-download-handler-url" },
            new() { Name = "Workspace Statuses: ", Link = "/Relativity.Rest/API/relativity-environment/v1/workspace/eligible-statuses" },
            new() { Name = "SQL Server Languages: ", Link = "/Relativity.Rest/API/relativity-environment/v1/workspace/eligible-sql-full-text-languages" }
        };
        static async Task PickOption()
        {

            if (string.IsNullOrEmpty(SecretValues.CookieString))
            {
                Console.WriteLine("You forgot to add your RelativityAuth cookie value. Please add and try again.");
            }
            else
            {
                CookieContainer mycontainer = new();

                mycontainer.Add(new Cookie("RelativityAuth", SecretValues.CookieString, "/", SecretValues.EndpointDomain));

                SocketsHttpHandler handler = new() { UseCookies = true, CookieContainer = mycontainer };

                if (SecretValues.Env == "Work")
                {

                    handler.UseProxy = true;
                    handler.Proxy = new WebProxy() { Address = SecretValues.ProxyURI, UseDefaultCredentials = true };

                };

                HttpClient myClient = new(handler);
                myClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");

                Console.WriteLine("What would you like to do?");
                Console.WriteLine();
                Console.WriteLine("0. Ping Test");
                Console.WriteLine("1. Resource Pools");
                Console.WriteLine("2. SQL Servers");
                Console.WriteLine("3. File Repository Servers");
                Console.WriteLine("4. Cache Location");
                Console.WriteLine("5. URL Default Download Handler");
                Console.WriteLine("6. Workspace Statuses");
                Console.WriteLine("7. SQL Server Language(s)");
                Console.WriteLine("8. Exit");

                Console.WriteLine();

                Console.Write("Selection: ");
                int Selection = int.Parse(Console.ReadLine());

                Console.WriteLine();

                if (Selection == 0)
                {

                    Link_Model choice = data_model[Selection];

                    HttpResponseMessage response = await myClient.PostAsync(SecretValues.APIBaseAddress + choice.Link, null);

                    Console.WriteLine($"Response code: {response.StatusCode}");

                    Console.WriteLine();
                    await PickOption();

                }
                else if (Selection > 0 && Selection < 8)
                {

                    Link_Model choice = data_model[Selection];

                    HttpResponseMessage response = await myClient.GetAsync(SecretValues.APIBaseAddress + choice.Link);

                    Console.WriteLine($"Response code: {response.StatusCode}");
                    Console.WriteLine($"{choice.Name}: {response.Content.ReadAsStringAsync().Result}");

                    Console.WriteLine();
                    await PickOption();
                }
                else if (Selection == 8)
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Not a sensible selection - try again");
                    Console.WriteLine();
                    await PickOption();
                }


            }

        }
    }
}
