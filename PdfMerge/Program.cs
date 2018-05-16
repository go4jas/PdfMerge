using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = @"C:\Users\jaspreet.bhangoo\source\repos\PdfMerge\PdfMerge\src\";
            string outputFile = @"C:\Users\jaspreet.bhangoo\source\repos\PdfMerge\PdfMerge\dest\out.pdf";

            List<string> sourceFiles = new List<string>();

            foreach (string file in Directory.EnumerateFiles(sourcePath, "*.pdf"))
            {
                sourceFiles.Add(file);
            }

            MergePdfFiles(outputFile, sourceFiles);
        }

        private static void MergePdfFiles(string outputFile, List<string> sourceFiles)
        {
            using (var writer = new PdfWriter(new FileStream(outputFile, FileMode.OpenOrCreate)))
            {
                using (var outputDocument = new PdfDocument(writer))
                {
                    outputDocument.InitializeOutlines();
                    var rootOutline = outputDocument.GetOutlines(false);
                    PdfOutline rootO = rootOutline.AddOutline("Root");
                    int pages = 1;
                    int count = 1;
                    foreach (var inputFile in sourceFiles)
                    {
                        using (var inputDoc = new PdfDocument(new PdfReader((inputFile))))
                        {
                            for (int i = 1; i <= inputDoc.GetNumberOfPages(); i++)
                            {
                                var newp = outputDocument.AddNewPage();
                                var canvas = new PdfCanvas(newp);
                                var origPage = inputDoc.GetPage(i);
                                var copy = origPage.CopyAsFormXObject(outputDocument);
                                canvas.AddXObject(copy, 0, 0);
                                copy.Flush();
                                origPage = null;
                                canvas.Release();
                                newp.Flush();
                                writer.Flush();
                                canvas = null;
                                newp = null;
                            }

                            var subPages = inputDoc.GetNumberOfPages();
                            //PdfOutline pdfOutine = inputDoc.GetOutlines(false);
                            /*var link1 = rootO.AddOutline(count.ToString());
                            link1.AddDestination(PdfExplicitDestination.CreateFit(outputDocument.GetPage(pages)));
                            pages += subPages;*/

                            PdfOutline pdfOutine = inputDoc.GetOutlines(false);

                            //pdfOutine.GetAllChildren().FirstOrDefault().AddOutline;
                            foreach (var aOutline in pdfOutine.GetAllChildren())
                            {
                                var link1 = rootO.AddOutline(aOutline.GetTitle(), aOutline.pos);
                                link1.AddDestination(PdfExplicitDestination.CreateFit(outputDocument.GetPage(pages)));
                            }
                            pages += subPages;


                            count++;
                        }
                    }
                }
            }

        }
    }
}
