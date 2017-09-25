using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace CreatePDF
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            CreatePDF();
        }

        public static void CreatePDF(){
            //设置添加内容
            string pdfName = "阿苏卫2017年8月报表";//文件名
            string name = "阿苏卫填埋场";
            string location = "116.3454,40.15811,90";
            string year = "2009.9.9";
            string note = "  阿苏卫垃圾填埋场从1986年开始修建，1994年投入运营，占地26公顷，位于北京市西北郊。阿苏卫填埋场原来只负责处理北京市东城区和西城区的垃圾，每天处理1200吨。但后来每天处理垃圾量达到3500吨，这些垃圾包括来自朝阳区、顺义区和昌平区的商业垃圾。" +
                 "\n  垃圾填埋场紧邻二德庄、阿苏卫、百善、牛房圈四村。" +
                 "\n  2011年12月28日，昌平区举行“阿苏卫循环经济园”周边村庄搬迁安置房项目奠基仪式，此举标志着“阿苏卫循环经济园”项目正式启动。";
            iTextSharp.text.Image img1 = iTextSharp.text.Image.getInstance(@"reportimg/asw.jpg");
            img1.Alignment = iTextSharp.text.Image.MIDDLE;
            iTextSharp.text.Image img21 = iTextSharp.text.Image.getInstance(@"reportimg/chart.jpg");
            img21.scaleAbsolute(400, 270);//图片固定400*270保证排版不乱！！！
            img21.Alignment = iTextSharp.text.Image.MIDDLE;
            iTextSharp.text.Image img22 = iTextSharp.text.Image.getInstance(@"reportimg/chart1.jpg");
            img22.scaleAbsolute(400, 270);
            img22.Alignment = iTextSharp.text.Image.MIDDLE;
            iTextSharp.text.Image img8 = iTextSharp.text.Image.getInstance(@"reportimg/2d.jpg");
            img8.scaleAbsolute(400, 400);
            img8.Alignment = iTextSharp.text.Image.MIDDLE;
            iTextSharp.text.Image img9 = iTextSharp.text.Image.getInstance(@"reportimg/3d.jpg");
            img9.scaleAbsolute(400, 400);
            img9.Alignment = iTextSharp.text.Image.MIDDLE;

            // step 1: creation of a document-object
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            try
            {
                PdfWriter writer = PdfWriter.getInstance(document, new FileStream(pdfName + ".pdf", FileMode.Create));
                document.Open();
                //设置字体--添加字体库
                BaseFont bfHei = BaseFont.createFont(@"simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//黑体
                BaseFont bfSun = BaseFont.createFont(@"simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//新宋体
                iTextSharp.text.Font chapterFont = new iTextSharp.text.Font(bfHei, 18); //FontFactory.getFont(FontFactory.HELVETICA, 24, iTextSharp.text.Font.NORMAL, new iTextSharp.text.Color(255, 0, 0));
                iTextSharp.text.Font sectionFont = new iTextSharp.text.Font(bfSun, 16); //FontFactory.getFont(FontFactory.HELVETICA, 20, iTextSharp.text.Font.NORMAL, new iTextSharp.text.Color(0, 0, 255));
                iTextSharp.text.Font contentFont = new iTextSharp.text.Font(bfSun, 12);
                //--------------------1填埋场简介-----------------------
                Paragraph cTitle = new Paragraph("填埋场简介", chapterFont);
                Chapter chapter = new Chapter(cTitle, 1);
                document.Add(chapter);
                //--------------------1填埋场简介--表格
                Table table = new Table(2);
                table.BorderWidth = 1;
                table.BorderColor = new iTextSharp.text.Color(0, 0, 255);
                table.Padding = 5;
                table.Spacing = 5;
                //table.DefaultHorizontalAlignment = Element.ALIGN_CENTER;
                Cell cell = new Cell("", contentFont);//图片
                cell.Rowspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Add(img1);
                //cell.BorderColor = new iTextSharp.text.Color(255, 0, 0);
                table.addCell(cell);
                table.addCell(new Cell("名称:\n" + name, contentFont));//名称
                table.addCell(new Cell("位置:\n" + location, contentFont));//位置
                table.addCell(new Cell("建成年份:\n" + year, contentFont));//建成年份
                cell = new Cell("简介:\n" + note, contentFont);//简介
                cell.Colspan = 2;
                cell.BackgroundColor = new iTextSharp.text.Color(0xC0, 0xC0, 0xC0);
                table.addCell(cell);
                document.Add(table);
                //--------------------2基本信息-------------------------
                cTitle = new Paragraph("基本信息", chapterFont);
                chapter = new Chapter(cTitle, 2);
                //--------------------2基本信息--img21
                Paragraph sTitle = new Paragraph("填埋场数量按行政区划分", sectionFont);
                Section section = chapter.addSection(sTitle, 2);//2表示处于那一章节
                Paragraph paraImg = new Paragraph();
                paraImg.Add(img21);
                section.Add(paraImg);
                //--------------------2基本信息--img22
                sTitle = new Paragraph("填埋场数量按管理单位分", sectionFont);
                section = chapter.addSection(sTitle, 2);
                paraImg = new Paragraph();
                paraImg.Add(img22);
                section.Add(paraImg);
                document.Add(chapter);
                //--------------------3投资信息-------------------------
                cTitle = new Paragraph("投资信息", chapterFont);
                chapter = new Chapter(cTitle, 3);
                //--------------------3投资信息--img31
                sTitle = new Paragraph("库容统计", sectionFont);
                section = chapter.addSection(sTitle, 2);
                paraImg = new Paragraph();
                paraImg.Add(img21);
                section.Add(paraImg);
                //--------------------3投资信息--img32
                sTitle = new Paragraph("当前投资情况", sectionFont);
                section = chapter.addSection(sTitle, 2);
                paraImg = new Paragraph();
                paraImg.Add(img22);
                section.Add(paraImg);
                //--------------------3投资信息--img33
                sTitle = new Paragraph("垃圾处理量", sectionFont);
                section = chapter.addSection(sTitle, 2);
                paraImg = new Paragraph();
                paraImg.Add(img21);
                section.Add(paraImg);
                //--------------------3投资信息--img34
                sTitle = new Paragraph("当前库容利用率", sectionFont);
                section = chapter.addSection(sTitle, 2);
                paraImg = new Paragraph();
                paraImg.Add(img22);
                section.Add(paraImg);
                document.Add(chapter);
                //--------------------4运营信息-------------------------
                cTitle = new Paragraph("运营信息（当月）", chapterFont);
                chapter = new Chapter(cTitle, 4);
                document.Add(chapter);
                int NumColumnsYY = 6;
                PdfPTable datatableYY = new PdfPTable(NumColumnsYY);
                datatableYY.DefaultCell.Padding = 3;
                float[] headerwidthsYY = { 9, 8, 8, 10, 8, 11 }; // percentage
                datatableYY.setWidths(headerwidthsYY);
                //datatable.WidthPercentage = 100; // percentage
                datatableYY.DefaultCell.BorderWidth = 2;
                datatableYY.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                datatableYY.addCell(new Phrase("序号", sectionFont));
                datatableYY.addCell(new Phrase("数据1", sectionFont));
                datatableYY.addCell(new Phrase("数据2", sectionFont));
                datatableYY.addCell(new Phrase("数据3", sectionFont));
                datatableYY.addCell(new Phrase("数据4", sectionFont));
                datatableYY.addCell(new Phrase("数据5", sectionFont));
                datatableYY.HeaderRows = 1;  // this is the end of the table header
                datatableYY.DefaultCell.BorderWidth = 1;
                int maxYY = 26;
                for (int i = 1; i < maxYY; i++)
                {
                    if (i % 2 == 1)
                    {
                        datatableYY.DefaultCell.GrayFill = 0.9f;
                    }
                    for (int x = 0; x < NumColumnsYY; x++)
                    {
                        datatableYY.addCell(i + "-" + x);
                    }
                    if (i % 2 == 1)
                    {
                        datatableYY.DefaultCell.GrayFill = 0.0f;
                    }
                }
                document.Add(datatableYY);
                //--------------------5当前库容-------------------------
                cTitle = new Paragraph("当前库容（当前）", chapterFont);
                chapter = new Chapter(cTitle, 5);
                document.Add(chapter);
                document.Add(img21);
                //--------------------6库容利用率-----------------------
                cTitle = new Paragraph("库容利用率", chapterFont);
                chapter = new Chapter(cTitle, 6);
                document.Add(chapter);
                document.Add(img21);
                //--------------------7堆体安全预警---------------------
                cTitle = new Paragraph("堆体安全预警", chapterFont);
                chapter = new Chapter(cTitle, 7);
                document.Add(chapter);
                int NumColumnsYJ = 6;
                PdfPTable datatableYJ = new PdfPTable(NumColumnsYJ);
                datatableYJ.DefaultCell.Padding = 3;
                float[] headerwidthsYJ = { 9, 8, 8, 10, 8, 11 }; // percentage
                datatableYJ.setWidths(headerwidthsYJ);
                //datatable.WidthPercentage = 100; // percentage
                datatableYJ.DefaultCell.BorderWidth = 2;
                datatableYJ.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                datatableYJ.addCell(new Phrase("序号", sectionFont));
                datatableYJ.addCell(new Phrase("数据1", sectionFont));
                datatableYJ.addCell(new Phrase("数据2", sectionFont));
                datatableYJ.addCell(new Phrase("数据3", sectionFont));
                datatableYJ.addCell(new Phrase("数据4", sectionFont));
                datatableYJ.addCell(new Phrase("数据5", sectionFont));
                datatableYJ.HeaderRows = 1;  // this is the end of the table header
                datatableYJ.DefaultCell.BorderWidth = 1;
                int maxYJ = 56;
                for (int i = 1; i < maxYJ; i++)
                {
                    if (i % 2 == 1)
                    {
                        datatableYJ.DefaultCell.GrayFill = 0.9f;
                    }
                    for (int x = 0; x < NumColumnsYJ; x++)
                    {
                        datatableYJ.addCell(i + "-" + x);
                    }
                    if (i % 2 == 1)
                    {
                        datatableYJ.DefaultCell.GrayFill = 0.0f;
                    }
                }
                document.Add(datatableYJ);
                //--------------------8二维设计图-----------------------
                cTitle = new Paragraph("二维设计图", chapterFont);
                chapter = new Chapter(cTitle, 8);
                document.Add(chapter);
                document.Add(img8);
                //--------------------9三维设计图-----------------------
                cTitle = new Paragraph("三维设计图", chapterFont);
                chapter = new Chapter(cTitle, 9);
                document.Add(chapter);
                document.Add(img9);
            }
            catch (Exception de)
            {
                Console.Error.WriteLine(de.StackTrace);
            }
            document.Close();
            Console.WriteLine("create success!");
            Console.Read();
        }
    }
}
