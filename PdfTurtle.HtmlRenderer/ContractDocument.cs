using HtmlAgilityPack;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfTurtle.HtmlRenderer
{
	public class ContractDocument
	{
		public static byte[] FromHtml(string html)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var pdfDocument = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(2, Unit.Centimetre);
					page.DefaultTextStyle(x => x.FontSize(12));

					page.Content().Column(col =>
					{
						foreach (var node in doc.DocumentNode.ChildNodes)
						{
							RenderNode(node, col);
						}

						col.Item().Row(row =>
						{
							row.Spacing(20);

							row.RelativeItem().Column(c => RenderSignatureField(c, "Kartódromo"));
							row.RelativeItem().Column(c => RenderSignatureField(c, "Locatário"));
							row.RelativeItem().Column(c => RenderSignatureField(c, "Testemunha 2"));
						});
					});
				});
			});

			return pdfDocument.GeneratePdf();
		}

		private static void RenderNode(HtmlNode node, ColumnDescriptor col)
		{
			switch (node.Name.ToLower())
			{
				case "h1":
					col.Item().Text(node.InnerText)
						.FontSize(16)
						.Bold()
						.LineHeight(1.2f);
					col.Spacing(10);
					break;

				case "h2":
					col.Item().Text(node.InnerText)
						.FontSize(14)
						.Bold()
						.LineHeight(1.2f);
					col.Spacing(8);
					break;

				case "h3":
					col.Item().Text(node.InnerText)
						.FontSize(12)
						.SemiBold()
						.LineHeight(1.2f);
					col.Spacing(6);
					break;

				case "p":
					col.Item().Text(txt =>
					{
						txt.DefaultTextStyle(x => x.FontSize(8).LineHeight(1.4f));
						txt.Justify();

						RenderInline(node, txt);
					});
					col.Spacing(8);
					break;

				case "br":
					col.Item().Text("");
					break;

				case "hr":
					col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
					col.Spacing(10);
					break;

				default:
					if (node.NodeType == HtmlNodeType.Text)
					{
						var text = node.InnerText.Trim();
						if (!string.IsNullOrWhiteSpace(text))
							col.Item().Text(text).Justify();
					}
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

		private static void RenderSignatureField(ColumnDescriptor parent, string label, float height = 1.5f, float spacing = 15f)
		{
			parent.Item().Column(c =>
			{
				c.Spacing(spacing);

				c.Item().Text(label)
					.FontSize(10)
					.SemiBold()
					.AlignCenter();

				c.Item().Height(height)
					.Background(Colors.Black);
			});
		}
	}
}
