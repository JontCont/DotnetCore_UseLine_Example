using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3;
using StartFMS_BackendAPI.Extensions;

namespace StartFMS_BackendAPI.Line.WebAPI.Extensions.ChatGPT {
    public static class Chat {

        public static async Task<string> ResponseMessageAsync(string Prompt) {
            var openAiService = new OpenAIService(new OpenAiOptions() {
                ApiKey = Config.GetConfiguration().GetValue<string>("OpenAIServiceOptions:ApiKey"),
            });

            var completionResult =  openAiService.Completions.CreateCompletionAsStream(new CompletionCreateRequest() {
                Prompt = Prompt,
                MaxTokens = 4000
            }, Models.TextDavinciV3);
            
            string str = "";
            await foreach (var completion in completionResult) {
                if (completion.Successful) {
                    Console.Write(completion.Choices.FirstOrDefault()?.Text);
                    str += completion.Choices.FirstOrDefault()?.Text;
                }
                else {
                    if (completion.Error == null) {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
                }
            }

            Console.WriteLine("Complete");
            Console.Write("回應: "+ str);
            return str;
        }//ResponseMessageAsync

    }// class
}
