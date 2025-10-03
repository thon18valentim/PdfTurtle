
namespace PdfTurtle.Writer.Interfaces
{
	public interface IPdfDocument
	{
		void Save();
		void Save(string path);
		byte[] Write();
	}
}
