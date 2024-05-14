using Google.Cloud.Vision.V1;

namespace summa_task.Services
{
    public interface IImageProcessingService
    {
        Task<(string ExtractedText, string ImageDataUrl)> ProcessImageAsync(byte[] imageBytes);
    }

    public class GoogleVisionImageProcessingService : IImageProcessingService
    {
        public async Task<(string ExtractedText, string ImageDataUrl)> ProcessImageAsync(byte[] imageBytes)
        {
            var image = Image.FromBytes(imageBytes);
            var client = await ImageAnnotatorClient.CreateAsync();
            var response = await client.DetectTextAsync(image);
            var extractedText = string.Join(Environment.NewLine, response.SelectMany(annotation => annotation.Description.Split(new[] { '\n' }, StringSplitOptions.None)));
            var imageDataUrl = ConvertToDataUrl(imageBytes);

            return (extractedText, imageDataUrl);
        }

        private static string ConvertToDataUrl(byte[] imageBytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
}