using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Media.Imaging;

namespace FinancialTracker.Utils
{
    public class ReportGenerator
    {
        public static void CreateReport(string pdfFilePath, string totalExpensesText, BitmapImage chartImage)
        {
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath, FileMode.Create));
                doc.Open();

                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell(new Phrase($"Total Expenses: ${totalExpensesText}"));
                cell.Border = Rectangle.NO_BORDER;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = 50f;
                table.AddCell(cell);

                PdfPCell chartCell = new PdfPCell();
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(chartImage));

                string tempImagePath = Path.GetTempFileName() + ".jpg";

                using (var stream = new FileStream(tempImagePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                Image image = Image.GetInstance(tempImagePath);
                image.ScaleAbsolute(500f, 400f);
                chartCell.AddElement(image);
                table.AddCell(chartCell);

                doc.Add(table);
                File.Delete(tempImagePath);
                doc.Close();
        }
    }
}

