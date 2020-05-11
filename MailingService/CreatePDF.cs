using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MailingService
{
    class CreatePDF
    {
        public int dayNo = 0;
        public String MakePDFPresent(List<List<String>> presentlls, String buildingName)
        {
            Console.Write(buildingName);
            String fPath = "";
            using (Document doc = new Document(PageSize.A4))
            {
                String bwd = AppDomain.CurrentDomain.BaseDirectory;
                string cwd = bwd + "DailyAttendance/" + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy");
                if (!Directory.Exists(cwd))
                {
                    Directory.CreateDirectory(cwd);
                }
                if (!Directory.Exists(cwd + "/present"))
                {
                    Directory.CreateDirectory(cwd + "/present");
                }
                fPath = cwd + "/present" + "/" + buildingName.Trim() + " " + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy") + " present.pdf";
                Stream output = new FileStream(fPath, FileMode.Create);
                PdfWriter pdfwriter = PdfWriter.GetInstance(doc, output);

                doc.Open();
                doc.Add(new Chunk(""));

                Paragraph para = new Paragraph("GoodEvening, \nThe following is the list of students staying at hostel \"" + buildingName.Trim() + "\" and has properly mark their " +
                    "attendance. Dated :- " + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy"))
                {
                    Alignment = Element.ALIGN_LEFT
                };
                doc.Add(para);

                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(bwd + "iitmandilogo.png");
                logo.SetAbsolutePosition(470, 780);
                logo.ScaleAbsoluteHeight(50);
                logo.ScaleAbsoluteWidth(100);

                pdfwriter.DirectContent.AddImage(logo);

                PdfPTable table = new PdfPTable(4);
                table.SetTotalWidth(new float[] { 36, 72, 270, 144 });
                table.DefaultCell.Border = 0;
                table.DefaultCell.MinimumHeight = 50;
                table.DefaultCell.BorderColor = BaseColor.WHITE;
                table.LockedWidth = true;
                table.SpacingAfter = 20;
                table.SpacingBefore = 20;

                int no = 1;
                foreach (List<String> preslls in presentlls)
                {
                    table.AddCell(new PdfPCell(new Phrase(no.ToString())));
                    table.AddCell(new PdfPCell(new Phrase(preslls[0])));
                    table.AddCell(new PdfPCell(new Phrase(preslls[1])));
                    table.AddCell(new PdfPCell(new Phrase(preslls[2])));
                    ++no;
                }
                doc.Add(table);
                doc.Close();
            }
            return fPath;
        }
        public String MakePDFAbsent(List<List<String>> absentlls, String buildingName)
        {
            String fPath = "" ;
            using (Document doc = new Document(PageSize.A4))
            {
                String bwd = AppDomain.CurrentDomain.BaseDirectory;
                string cwd = bwd + "DailyAttendance/" + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy");
                if (!Directory.Exists(cwd + "/absent"))
                {
                    Directory.CreateDirectory(cwd + "/absent");
                }
                fPath = cwd + "/absent" + "/" + buildingName.Trim() + " " + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy") + " absent.pdf";
                Stream output = new FileStream(fPath, FileMode.Create);
                PdfWriter pdfwriter = PdfWriter.GetInstance(doc, output);

                doc.Open();
                doc.Add(new Chunk(""));

                Phrase p1Header = new Phrase("Hello World", FontFactory.GetFont("verdana", 15, Font.BOLD));
                Paragraph para = new Paragraph("GoodEvening, \nThe following is the list of students staying at hostel \"" + buildingName.Trim() + "\" and has NOT properly mark their " +
                    "attendance. Dated :- " + DateTime.Now.AddDays(this.dayNo).ToString("dd-MM-yyyy"))
                {
                    Alignment = Element.ALIGN_LEFT
                };
                doc.Add(para);

                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(bwd + "iitmandilogo.png");
                logo.SetAbsolutePosition(470, 780);
                logo.ScaleAbsoluteHeight(50);
                logo.ScaleAbsoluteWidth(100);

                pdfwriter.DirectContent.AddImage(logo);

                PdfPTable table = new PdfPTable(7);
                table.SetTotalWidth(new float[] { 36, 54, 198, 90, 54, 54, 36 });
                table.DefaultCell.Border = 0;
                table.DefaultCell.MinimumHeight = 50;
                table.DefaultCell.BorderColor = BaseColor.WHITE;
                table.LockedWidth = true;
                table.SpacingAfter = 20;
                table.SpacingBefore = 20;

                int no = 1;
                foreach (List<String> abslls in absentlls)
                {
                    table.AddCell(new PdfPCell(new Phrase(no.ToString())));
                    table.AddCell(new PdfPCell(new Phrase(abslls[0])));
                    table.AddCell(new PdfPCell(new Phrase(abslls[1])));
                    table.AddCell(new PdfPCell(new Phrase(abslls[2])));
                    table.AddCell(new PdfPCell(new Phrase(abslls[3])));
                    table.AddCell(new PdfPCell(new Phrase(abslls[4])));
                    table.AddCell(new PdfPCell(new Phrase(abslls[5])));
                    ++no;
                }
                doc.Add(table);
                doc.Close();
            }
            return fPath;
        }
    }
}
