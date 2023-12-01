using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace CloudOps.OpenAI.Functions
{
    public static class OpenAIFunction  
    {  
        [FunctionName("OpenAIFunction")]  
        public static async Task<IActionResult> Run(  
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,  
            ILogger log)  
        {  
            // Get the prompt from the request body  
            string question = await req.Content.ReadAsStringAsync();  
            string prompt = "You are an AI assistant that helps developers find accurate technical information with actual resources to support your answers.";

            string endpoint = "<OPENAI_ENDPOIN>";

            /* new keyvault grab */
            string secretName = "<OpenAI-API-Key>";
            var vaultUri = new Uri("<KEY_VAULT_URL>");
           
            var credential = new ManagedIdentityCredential();
            var secretClient = new SecretClient(vaultUri, credential);
            log.LogInformation($"Pulling {secretName} secret");
            var secretResponse = await secretClient.GetSecretAsync(secretName);
            var key = secretResponse.Value;
            /* end of keyvault grab */

            OpenAIClient client = new( new Uri(endpoint), new AzureKeyCredential(key.Value) );


            var chatCompletionsOptions = new ChatCompletionsOptions(){
                Messages =
                {
                    new ChatMessage(ChatRole.System, prompt),
                    new ChatMessage(ChatRole.User, question),
                 },
                MaxTokens = 4000
            };
            Response<ChatCompletions> response = client.GetChatCompletions(
            deploymentOrModelName: "<CHATDEPLOYMENT>", 
            chatCompletionsOptions);

            // Console.WriteLine(response.Value.Choices[0].Message.Content); 
            return new OkObjectResult(response.Value.Choices[0].Message.Content);  
        }  
    }  
}  
