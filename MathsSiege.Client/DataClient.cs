using MathsSiege.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MathsSiege.Client
{
    public class DataClient
    {
        private UserPreferences preferences;

        private HttpClient client = new HttpClient();
        private Random random = new Random();

        private string token;
        private List<Question> questions;

        private List<Question> unusedQuestions = new List<Question>();

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
                var response = await this.client.PostAsync(this.preferences.HostAddress + "/api/auth/authenticate", content);

                // Check if the request was successful.
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                // Retrieve the token from the response content.
                string responseBody = await response.Content.ReadAsStringAsync();
                this.token = JsonConvert.DeserializeObject<TokenResponse>(responseBody).Token;

                this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", this.token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Downloads a list of questions from the server.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadQuestionsAsync()
        {
            try
            {
                // Make the request to the server.
                var response = await this.client.GetAsync(this.preferences.HostAddress + "/api/questions");

                // Check if the request was successful.
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                // Retrieve the questions from the response content.
                string responseBody = await response.Content.ReadAsStringAsync();
                this.questions = JsonConvert.DeserializeObject<List<Question>>(responseBody);

                // Initialise the references from choices to questions.
                foreach (var question in this.questions)
                {
                    foreach (var choice in question.Choices)
                    {
                        choice.Question = question;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets a random unused question (unless all questions are used, in which
        /// case it starts again).
        /// </summary>
        /// <returns></returns>
        public Question GetRandomQuestion()
        {
            if (this.unusedQuestions.Count == 0)
            {
                this.unusedQuestions.AddRange(this.questions);
            }

            int i = this.random.Next(this.unusedQuestions.Count);
            var question = this.unusedQuestions[i];
            this.unusedQuestions.RemoveAt(i);

            return question;
        }

        public async Task<bool> PostGameSession(GameSession session)
        {
            // Set the choice ID for each answer. This is necessary for the server
            // to identify it as an existing choice.
            foreach (var answer in session.Answers)
            {
                answer.ChoiceId = answer.Choice.Id;
                answer.Choice = null;
            }

            try
            {
                // Serialize session data to json string.
                string data = JsonConvert.SerializeObject(session, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                
                // Create the request content object.
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                // Send the request to the server.
                var response = await this.client.PostAsync(this.preferences.HostAddress + "/api/gamesessions", content);

                // Check if the request was successful.
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

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
