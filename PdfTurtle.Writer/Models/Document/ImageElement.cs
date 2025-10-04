
namespace PdfTurtle.Writer.Models.Document
{
	public class ImageElement(string source, float? width = null, float? height = null) : HtmlDocumentElement
	{
		public string Source { get; } = source;
		public float? Width { get; } = width;
		public float? Height { get; } = height;
	}
}
