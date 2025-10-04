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
			var pdfDocument = WriteDocument();
			return pdfDocument.GeneratePdf();
		}

		public void WriteToStream(Stream stream)
		{
			var pdfDocument = WriteDocument();
			pdfDocument.GeneratePdf(stream);
		}

		private Document WriteDocument()
		{
			var pdfDocument = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(options.ConvertPageSize());
					page.PageColor(Colors.White);

					page.MarginTop(options.PageMarginTop, Unit.Centimetre);
					page.MarginBottom(options.PageMarginBottom, Unit.Centimetre);
					page.MarginLeft(options.PageMarginLeft, Unit.Centimetre);
					page.MarginRight(options.PageMarginRight, Unit.Centimetre);

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

					if (!string.IsNullOrWhiteSpace(options.FooterText))
					{
						page.Footer()
							.AlignCenter()
							.Text(options.FooterText)
							.FontSize(options.FooterFontSize)
							.Italic();
					}
				});
			});

			return pdfDocument;
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

				case ImageElement img:
					RenderImage(col.Item(), img);
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

		private static void RenderImage(IContainer container, ImageElement image)
		{
			try
			{
				if (string.IsNullOrEmpty(image.Source))
					return;

				byte[] imageData = null;

				if (image.Source.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
				{
					var base64Data = image.Source.Substring(image.Source.IndexOf(",") + 1);
					imageData = Convert.FromBase64String(base64Data);
				}
				else if (File.Exists(image.Source))
				{
					imageData = File.ReadAllBytes(image.Source);
				}
				else if (Uri.IsWellFormedUriString(image.Source, UriKind.Absolute))
				{
					using var client = new HttpClient();
					imageData = client.GetByteArrayAsync(image.Source).Result;
				}

				if (imageData == null || imageData.Length == 0)
					return;

				container = container.AlignCenter();

				// define tamanho, se existir
				if (image.Width.HasValue)
					container = container.Width(image.Width.Value);
				if (image.Height.HasValue)
					container = container.Height(image.Height.Value);

				// largura padrão, caso nada informado
				if (!image.Width.HasValue && !image.Height.HasValue)
					container = container.Width(200);

				container.Image(imageData);
			}
			catch
			{
				// ignora erros de imagem
			}
		}

	}
}
