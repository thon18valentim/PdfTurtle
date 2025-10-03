using HtmlAgilityPack;

namespace PdfTurtle.Writer.Models.Document
{
	internal class HeadingElement : HtmlDocumentElement
	{
		public required int Level { get; set; }
		public required HtmlNode Node { get; set; }
		public string Text => Node.InnerText;
	}
}
