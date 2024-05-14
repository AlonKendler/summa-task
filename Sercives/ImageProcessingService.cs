using Google.Cloud.Vision.V1;

namespace summa_task.Services
{
    public interface IImageProcessingService
    {
        Task<(AnnotateImageResponse Response, string ExtractedText, string ImageDataUrl)> ProcessImageAsync(byte[] imageBytes);
    }


    public class GoogleVisionImageProcessingService : IImageProcessingService
    {
        public async Task<(AnnotateImageResponse Response, string ExtractedText, string ImageDataUrl)> ProcessImageAsync(byte[] imageBytes)
        {
            var image = Image.FromBytes(imageBytes);
            var client = await ImageAnnotatorClient.CreateAsync();
            var rawResponse = await client.DetectTextAsync(image);

            // Create an AnnotateImageResponse object and initialize the TextAnnotations property
            var response = new AnnotateImageResponse
            {
                TextAnnotations = { rawResponse }
            };

            var extractedText = string.Join(Environment.NewLine, rawResponse.SelectMany(annotation => annotation.Description.Split(new[] { '\n' }, StringSplitOptions.None)));
            var imageDataUrl = ConvertToDataUrl(imageBytes);

            return (response, extractedText, imageDataUrl);
        }

        private static string ConvertToDataUrl(byte[] imageBytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
}