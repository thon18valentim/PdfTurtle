using HtmlAgilityPack;

namespace PdfTurtle.Writer.Models.Document
{
	internal class SignatureElement : HtmlDocumentElement
	{
		public required string Label { get; set; }
		public required HtmlNode Node { get; set; }
	}
}
