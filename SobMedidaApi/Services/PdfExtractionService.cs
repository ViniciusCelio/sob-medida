using UglyToad.PdfPig;
using System.Text;

namespace SobMedidaApi.Services
{
    public class PdfExtractionService
    {
        public string ExtractText(Stream pdfStream)
        {
            var memoryStream = new MemoryStream();
            pdfStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using var document = PdfDocument.Open(memoryStream.ToArray());
            var sb = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                var words = page.GetWords()
                    .OrderBy(w => -w.BoundingBox.Top)
                    .ThenBy(w => w.BoundingBox.Left)
                    .ToList();

                double? lastTop = null;
                foreach (var word in words)
                {
                    var currentTop = Math.Round(word.BoundingBox.Top, 0);

                    if (lastTop.HasValue && Math.Abs(lastTop.Value - currentTop) > 2)
                        sb.AppendLine();

                    sb.Append(word.Text + " ");
                    lastTop = currentTop;
                }

                sb.AppendLine();
                sb.AppendLine("---PAGE BREAK---");
            }

            return sb.ToString().Trim();
        }
    }
}