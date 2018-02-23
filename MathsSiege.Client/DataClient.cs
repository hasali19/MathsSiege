using MathsSiege.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MathsSiege.Client
{
    public class DataClient
    {
        private UserPreferences preferences;

        private HttpClient client = new HttpClient();

        private string token;

        public DataClient(UserPreferences preferences)
        {
            this.preferences = preferences;
        }

        /// <summary>
        /// Logs in the user to the server to obtain an authentication token.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // Serialize user credentials to json.
                string data = JsonConvert.SerializeObject(new User { Username = username, Password = password });
                // Create request content object containing the serialized data.
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                // Make the request to the server.
                var response = await this.client.PostAsync(preferences.HostAddress + "/api/auth/authenticate", content);

                // Check if the request was successful.
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                // Retrieve the token from the response content.
                string responseBody = await response.Content.ReadAsStringAsync();
                this.token = JsonConvert.DeserializeObject<TokenResponse>(responseBody).Token;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
