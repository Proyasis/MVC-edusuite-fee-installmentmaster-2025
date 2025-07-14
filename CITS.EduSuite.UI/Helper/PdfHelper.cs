using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;


namespace CITS.EduSuite.UI
{
    public static class PdfHelper
    {

        public static byte[] GenerateProtectedPdfFromFile(byte[] file, string password)
        {
            byte[] bPDF = null;
            using (MemoryStream input = new MemoryStream(file))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                    bPDF = output.ToArray();
                }
            }
            return bPDF;

        }
    }
}