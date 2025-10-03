using PdfTurtle.Writer.Enums;
using QuestPDF.Helpers;

namespace PdfTurtle.Writer.Models.Settings
{
	public class PdfOptions
	{
		public EnumPageSizes PageSize { get; set; } = EnumPageSizes.A4;
		public float DefaultFontSize { get; set; } = 12;
		public float ParagraphSpacing { get; set; } = 10;
		public float ParagraphLineHeight { get; set; } = 1.5f;
		public float SignatureSpacing { get; set; } = 20;
		public string? FooterText { get; set; } = "Página gerada pelo PdfTurtle © 2025";
		public float FooterFontSize { get; set; } = 10;

		public PageSize ConvertPageSize()
		{
			return PageSize switch
			{
				EnumPageSizes.A4 => PageSizes.A4,
				_ => throw new NotImplementedException()
			};
		}

		public static PageSize ConvertPageSize(EnumPageSizes pageSize)
		{
			return pageSize switch
			{
				EnumPageSizes.A4 => PageSizes.A4,
				_ => throw new NotImplementedException()
			};
		}
	}
}
