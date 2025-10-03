using HtmlAgilityPack;
using PdfTurtle.Writer.Interfaces;
using PdfTurtle.Writer.Models.Document;
using PdfTurtle.Writer.Models.Settings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfTurtle.Writer.HtmlRenderer
{
	public class PdfHtmlDocument(
		List<HtmlDocumentElement> elements,
		PdfOptions options,
		SignatureOptions signatureOptions,
		HeadingOptions headingOptions) : IPdfDocument
	{
		public void Save()
		{
			var documentBytes = Write();

			var id = Guid.NewGuid().ToString();
			var downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
			var path = Path.Combine(downloadsFolder, $"turtle_file_{id}.pdf");

			File.WriteAllBytes(path, documentBytes);
		}

		public void Save(string path)
		{
			var documentBytes = Write();
			File.WriteAllBytes(path, documentBytes);
		}

		public byte[] Write()
		{
			var pdfDocument = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(options.ConvertPageSize());
					page.PageColor(Colors.White);
					page.Margin(2, Unit.Centimetre);

					page.Content().Column(col =>
					{
						foreach (var el in elements)
							RenderElement(col, el);

						col.Item().Row(row =>
						{
							row.Spacing(options.SignatureSpacing);

							foreach (var el in elements)
							{
								if (el is SignatureElement signatureElement)
								{
									row.RelativeItem().Column(c => RenderSignatureField(c, signatureElement.Label));
								}
							}
						});
					});
				});
			});

			return pdfDocument.GeneratePdf();
		}

		public void WriteToStream(Stream stream)
		{
			var pdfDocument = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(options.ConvertPageSize());
					page.PageColor(Colors.White);
					page.Margin(2, Unit.Centimetre);

					page.Content().Column(col =>
					{
						foreach (var el in elements)
							RenderElement(col, el);

						col.Item().Row(row =>
						{
							row.Spacing(options.SignatureSpacing);

							foreach (var el in elements)
							{
								if (el is SignatureElement signatureElement)
								{
									row.RelativeItem().Column(c => RenderSignatureField(c, signatureElement.Label));
								}
							}
						});
					});
				});
			});

			pdfDocument.GeneratePdf(stream);
		}

		private void RenderElement(ColumnDescriptor col, HtmlDocumentElement element)
		{
			switch (element)
			{
				case ParagraphElement p:
					col.Item().Text(txt =>
					{
						txt.DefaultTextStyle(x => x.FontSize(options.DefaultFontSize).LineHeight(options.ParagraphLineHeight));
						txt.Justify();

						RenderInline(p.Node, txt);
					});
					col.Spacing(options.ParagraphSpacing);
					break;

				case HeadingElement h:
					var size = h.Level switch
					{
						1 => headingOptions.H1Size,
						2 => headingOptions.H2Size,
						3 => headingOptions.H3Size,
						_ => options.DefaultFontSize
					};
					col.Item().Text(h.Text)
						.FontSize(size)
						.Bold()
						.LineHeight(headingOptions.HeadingLineHeight)
						.AlignCenter();
					col.Spacing(headingOptions.HeadingSpacing);
					break;

				case HorizontalRuleElement:
					col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
					col.Spacing(options.ParagraphSpacing);
					break;

				case SignatureElement sig:
					break;
			}
		}

		private static void RenderInline(HtmlNode parent, TextDescriptor txt)
		{
			foreach (var child in parent.ChildNodes)
			{
				if (child.NodeType == HtmlNodeType.Text)
				{
					var text = child.InnerText;
					if (!string.IsNullOrWhiteSpace(text))
						txt.Span(text.Trim() + " ");
				}
				else
				{
					switch (child.Name.ToLower())
					{
						case "b":
						case "strong":
							txt.Span(child.InnerText.Trim() + " ").Bold();
							break;

						case "i":
						case "em":
							txt.Span(child.InnerText.Trim() + " ").Italic();
							break;

						case "u":
							txt.Span(child.InnerText.Trim() + " ").Underline();
							break;

						default:
							RenderInline(child, txt);
							break;
					}
				}
			}
		}

		private void RenderSignatureField(ColumnDescriptor parent, string label)
		{
			parent.Item().Column(c =>
			{
				c.Spacing(signatureOptions.SignatureSpacing);

				c.Item().Text(label)
					.FontSize(signatureOptions.FontSize)
					.SemiBold()
					.AlignCenter();

				c.Item().Height(signatureOptions.LineHeight)
					.Background(Colors.Black);
			});
		}
	}
}
