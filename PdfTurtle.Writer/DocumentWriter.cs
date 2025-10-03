using PdfTurtle.Writer.HtmlRenderer;
using PdfTurtle.Writer.Models.Document;
using PdfTurtle.Writer.Models.Settings;
using QuestPDF.Infrastructure;

namespace PdfTurtle.Writer
{
	public class DocumentWriter
	{
		private readonly List<HtmlDocumentElement> _elements;
		private readonly PdfOptions _options = new();
		private readonly SignatureOptions _signatureOptions = new();
		private readonly HeadingOptions _headingOptions = new();

		private DocumentWriter(List<HtmlDocumentElement> elements)
		{
			QuestPDF.Settings.License = LicenseType.Community;
			_elements = elements;
		}

		public static DocumentWriter FromHtml(string html)
		{
			var elements = HtmlParser.Parse(html);
			return new DocumentWriter(elements);
		}

		public DocumentWriter WithDefaults(Action<PdfOptions> configure)
		{
			configure(_options);
			return this;
		}

		public DocumentWriter CustomizeSignature(Action<SignatureOptions> configure)
		{
			configure(_signatureOptions);
			return this;
		}

		public DocumentWriter CustomizeHeading(Action<HeadingOptions> configure)
		{
			configure(_headingOptions);
			return this;
		}

		public PdfHtmlDocument Build()
		{
			return new PdfHtmlDocument(_elements, _options, _signatureOptions, _headingOptions);
		}
	}
}
