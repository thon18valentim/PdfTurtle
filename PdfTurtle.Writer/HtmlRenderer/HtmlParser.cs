using HtmlAgilityPack;
using PdfTurtle.Writer.Models.Document;

namespace PdfTurtle.Writer.HtmlRenderer
{
	internal class HtmlParser
	{
		public static List<HtmlDocumentElement> Parse(string html)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var elements = new List<HtmlDocumentElement>();
			foreach (var node in doc.DocumentNode.ChildNodes)
			{
				if (node.NodeType != HtmlNodeType.Element)
					continue;

				switch (node.Name.ToLower())
				{
					case "p":
						elements.Add(new ParagraphElement { Node = node });
						break;

					case "h1":
						elements.Add(new HeadingElement { Level = 1, Node = node });
						break;

					case "h2":
						elements.Add(new HeadingElement { Level = 2, Node = node });
						break;

					case "h3":
						elements.Add(new HeadingElement { Level = 3, Node = node });
						break;

					case "hr":
						elements.Add(new HorizontalRuleElement());
						break;

					case "img":
						var src = node.GetAttributeValue("src", string.Empty);
						var widthAttr = node.GetAttributeValue("width", string.Empty);
						var heightAttr = node.GetAttributeValue("height", string.Empty);

						float? width = null;
						float? height = null;

						if (float.TryParse(widthAttr, out var w))
							width = w;
						if (float.TryParse(heightAttr, out var h))
							height = h;

						elements.Add(new ImageElement(src, width, height));
						break;

					case "signature":
						var label = node.GetAttributeValue("label", "Assinatura");
						elements.Add(new SignatureElement { Label = label, Node = node });
						break;
				}
			}

			return elements;
		}
	}
}
