using HtmlAgilityPack;

namespace PdfTurtle.Writer.Models.Document
{
	internal class ParagraphElement : HtmlDocumentElement
	{
		public required HtmlNode Node { get; set; }
		public string Text => Node.InnerText;
	}
}
