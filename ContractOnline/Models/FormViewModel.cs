using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ContractOnline.Models
{
    public class DeviceViewModel
    {
        public string Model { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class FormViewModel
    {
        public FormViewModel()
        {
            CreatedTiime = DateTime.Now;
        }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string IdType { get; set; }
        public DateTime DoB { get; set; }
        public string IdFirstName { get; set; }
        public string IdLastName { get; set; }
        public string DriverLicenceNo5a { get; set; }
        public string DriverLicenceNo5b { get; set; }
        public string PassportNo { get; set; }
        public string PasspotCountry { get; set; }
        //product & service panel
        public DeviceViewModel Router { get; set; }
        public DeviceViewModel AP { get; set; }
        public string InstallDate { get; set; }
        public string InstallTime { get; set; }
        public double Discount { get; set; }
        public string Signature { get; set; }
        public DateTime CreatedTiime { get; set; }
        private string _fileName = null;
        public string FileName
        {
            get
            {
                return string.Format(@"{0}_{1:ddMMyyyy}.pdf", BusinessName,CreatedTiime);
            }
        }


        public double OriginalPrice
        {
            get
            {
                return Router.Price * Router.Quantity + AP.Price * AP.Quantity;
            }
        }

        public double ActualPrice
        {
            get
            {
                return OriginalPrice - Discount;
            }
        }

        public void ToPDF(string filePath)
        {

            //color
            var mainGreen = new BaseColor(92, 184, 92);
            var ftHeader = FontFactory.GetFont("Helvetica Neue", 20, Font.BOLD, BaseColor.WHITE);
            var ftTitle = FontFactory.GetFont("Helvetica Neue", 18, Font.BOLD, mainGreen);
            var ftContext = FontFactory.GetFont("Arial Black", 18, Font.NORMAL, BaseColor.BLACK);

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            Document doc = new Document(new Rectangle(PageSize.A4), 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            PdfContentByte context = writer.DirectContent;


            //header
            context.Rectangle(0, doc.PageSize.Height - 50, doc.PageSize.Width, doc.PageSize.Height);
            context.SetColorStroke(mainGreen);
            context.SetColorFill(mainGreen);
            context.FillStroke();

            var logo = Image.GetInstance(ConfigurationManager.AppSettings["logoFile"]);
            logo.SetAbsolutePosition(100, doc.PageSize.Height - 40);
            logo.ScaleToFit(30, 30);
            context.AddImage(logo);
            var headerText = new Phrase("Greenspot Product & Service Application", ftHeader);
            ColumnText.ShowTextAligned(context, Element.ALIGN_CENTER, headerText, doc.PageSize.Width / 2 + 30, doc.PageSize.Height - 35, 0);

            //Your content
            var content = new Paragraph();
            content.SetLeading(0, 2.0f);
            content.IndentationLeft = 10f;

            //general
            content.Add(new Phrase("\n\n\nYour Detail\n", ftTitle));
            content.Add(new Phrase(BusinessName + "\n", ftContext));
            content.Add(new Phrase(Address + "\n", ftContext));
            content.Add(new Phrase(Title + " " + FirstName + " " + LastName + "\n", ftContext));
            content.Add(new Phrase(Phone + ", " + Mobile + ", " + Email + "\n", ftContext));
            //id
            content.Add(new Phrase("\nID - " + IdType + "\n", ftTitle));
            content.Add(new Phrase(IdFirstName + " " + IdLastName + "," + DoB.ToString("dd MMM yyyy") + "\n", ftContext));
            if (IdType.ToLower().Contains("passport"))
            {
                content.Add(new Phrase(PassportNo + ", " + PasspotCountry + "\n", ftContext));
            }
            else
            {
                content.Add(new Phrase(DriverLicenceNo5a + ", " + DriverLicenceNo5b + "\n", ftContext));
            }

            //Product & Service
            content.Add(new Phrase("\nProducts & Services", ftTitle));
            var devices = new PdfPTable(3);
            //devices.DefaultCell.Border = Rectangle.NO_BORDER;
            devices.HorizontalAlignment = 0;
            devices.AddCell(new Phrase("Model", ftContext));
            devices.AddCell(new Phrase("Price", ftContext));
            devices.AddCell(new Phrase("Qty.", ftContext));
            if (Router.Quantity > 0)
            {
                devices.AddCell(new Phrase(Router.Model, ftContext));
                devices.AddCell(new Phrase(Router.Price.ToString("$0.00"), ftContext));
                devices.AddCell(new Phrase(Router.Quantity.ToString(), ftContext));
            }
            if (AP.Quantity > 0)
            {
                devices.AddCell(new Phrase(AP.Model, ftContext));
                devices.AddCell(new Phrase(AP.Price.ToString("$0.00"), ftContext));
                devices.AddCell(new Phrase(AP.Quantity.ToString(), ftContext));
            }
            content.Add(devices);

            //installation
            content.Add(new Phrase("\nInstallation\n", ftTitle));
            content.Add(new Phrase(InstallDate + " " + InstallTime, ftContext));

            //price
            content.Add(new Phrase("\n\nPrice", ftTitle));
            var price = new PdfPTable(5);
            price.DefaultCell.Border = Rectangle.NO_BORDER;
            price.HorizontalAlignment = 0;
            price.AddCell(new Phrase("Price", ftContext));
            price.AddCell(new Phrase("", ftContext));
            price.AddCell(new Phrase("Discount", ftContext));
            price.AddCell(new Phrase("", ftContext));
            price.AddCell(new Phrase("", ftContext));

            price.AddCell(new Phrase(OriginalPrice.ToString("$0.00"), ftContext));
            price.AddCell(new Phrase("-", ftContext));
            price.AddCell(new Phrase(Discount.ToString("$0.00"), ftContext));
            price.AddCell(new Phrase("=", ftContext));
            price.AddCell(new Phrase(ActualPrice.ToString("$0.00"), ftContext));
            content.Add(price);

            doc.Add(content);


            //signature
            var imgSign = Image.GetInstance(Utilities.Base64ToImage(Signature), 
                System.Drawing.Imaging.ImageFormat.Png);
            imgSign.SetAbsolutePosition(380, 100);
            imgSign.ScaleToFit(200, 100);
            context.AddImage(imgSign);

            var lblDate = new Phrase(CreatedTiime.ToString("dd MMM, yyy"), ftContext);
            ColumnText.ShowTextAligned(context, Element.ALIGN_CENTER, lblDate, doc.PageSize.Width - 120, 80, 0);


            //footer
            context.Rectangle(0, 0, doc.PageSize.Width, 10);
            context.SetColorStroke(mainGreen);
            context.SetColorFill(mainGreen);
            context.FillStroke();

            doc.Close();
        }
    }



}