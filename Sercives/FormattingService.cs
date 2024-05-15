using OpenAI_API;
using OpenAI_API.Models;


namespace summa_task.Services
{
    public class FormattingService
    {
        private readonly OpenAIAPI _openAiClient;

        public FormattingService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI API key is not configured.");
            _openAiClient = new OpenAIAPI(apiKey);
        }

        public async Task<string> FormatInputAsync(string input)
        {
            // Create a new conversation
            var chat = _openAiClient.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0.7;

            // Set the system message with instructions
            chat.AppendSystemMessage("You are a text formatter. You will receive input and format it in JSON. For context, the input is an OCR of Google Vision output. They are Hebrew tax receipts.");

            // Provide the expected schema as a system message
            chat.AppendSystemMessage(
                "The expected schema is:\n" +
                "{\n" +
                "  \"company_name\": \"string\",\n" +
                "  \"subtitle\": \"string\",\n" +
                "  \"vat_id\": \"string\",\n" +
                "  \"business_union_number\": \"string\",\n" +
                "  \"contact\": {\n" +
                "    \"phone\": \"string\",\n" +
                "    \"fax\": \"string\",\n" +
                "    \"email\": \"string\",\n" +
                "    \"website\": \"string\"\n" +
                "  },\n" +
                "  \"date\": \"string\",\n" +
                "  \"invoice\": {\n" +
                "    \"number\": \"string\",\n" +
                "    \"copy_note\": \"string\",\n" +
                "    \"recipient\": {\n" +
                "      \"name\": \"string\",\n" +
                "      \"address\": \"string\",\n" +
                "      \"vat_or_id\": \"string\",\n" +
                "      \"email\": \"string\"\n" +
                "    },\n" +
                "    \"details\": [\n" +
                "      {\n" +
                "        \"subject\": \"string\",\n" +
                "        \"catalog_number\": \"string\",\n" +
                "        \"item_description\": \"string\",\n" +
                "        \"payment_due\": \"string\",\n" +
                "        \"remarks\": \"string\",\n" +
                "        \"bank\": \"string\",\n" +
                "        \"secure_payment_link\": \"string\"\n" +
                "      }\n" +
                "    ],\n" +
                "    \"document_status\": \"string\",\n" +
                "    \"page\": \"string\",\n" +
                "    \"items\": [\n" +
                "      {\n" +
                "        \"quantity\": \"number\",\n" +
                "        \"price\": \"string\",\n" +
                "        \"total\": \"string\"\n" +
                "      }\n" +
                "    ],\n" +
                "    \"discount\": \"string\",\n" +
                "    \"final_total\": \"string\",\n" +
                "    \"digital_signature_note\": \"string\"\n" +
                "  }\n" +
                "}");

            // Add the user input
            chat.AppendUserInput($"Input: {input}");

            // Get the response
            var response = await chat.GetResponseFromChatbotAsync();
            return response;
        }
    }
}
