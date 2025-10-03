using PdfTurtle.Writer;
using PdfTurtle.Writer.Enums;

var circuit = "KART GT LTDA";
var address = "RUA FULANO DE TAL, n234";
var cnpj = "00.000.000-00";

var clientName = "Othon Valentim";
var document = "12218363909";
var phone = "(41) 99237-8001";
var birthDate = "28-08-2001";
var email = "email";

var htmlContent = "<p>Hello world</p>";
var html = $@"
				<h2>CONTRATO DE LOCAÇÃO</h2>
				<p>Pelo presente instrumento particular de locação, de um lado, <b>{circuit.ToUpper()}</b>, doravante denominado LOCADORA, com sede em <strong>{address}</strong>, inscrito no CNPJ sob o nº <strong>{cnpj}</strong>; e, de outro lado, o(a) Sr(a). <strong>{clientName}</strong>, portador(a) do CPF nº <strong>{document}</strong>, nascido(a) em <strong>{birthDate}</strong>, residente e domiciliado(a), com e-mail <strong>{email}</strong> e telefone <strong>{phone}</strong>, doravante denominado(a) LOCATÁRIO(A), têm entre si, justas e contratadas, as seguintes condições:</p>
				<p>O presente contrato tem por objeto a locação de um kart, destinado exclusivamente à prática de lazer e/ou participação em competições organizadas, observando-se rigorosamente todas as normas de segurança exigidas pelas autoridades competentes e regulamentações específicas aplicáveis.</p>
						
				<hr />

				{htmlContent}
							
				<hr />

				<signature label='Kartódromo' />
				<signature label='Locatário' />
			";

var doc = DocumentWriter.FromHtml(html)
	.WithDefaults(opt =>
	{
		opt.PageSize = EnumPageSizes.A4;
		opt.DefaultFontSize = 10;
		opt.ParagraphSpacing = 8;
		opt.ParagraphLineHeight = 1.4f;
	})
	.CustomizeHeading(h =>
	{
		h.H1Size = 16;
		h.H2Size = 14;
		h.H3Size = 12;
		h.HeadingLineHeight = 1.2f;
	})
	.CustomizeSignature(s =>
	{
		s.LineHeight = 1.5f;
		s.SignatureSpacing = 15f;
	})
	.Build();

doc.Save();

Console.WriteLine("PDF Generated successfully");