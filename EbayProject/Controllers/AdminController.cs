using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbayProject.Models;
using EbayProject.BL;
using EbayProject.Helping_Classes;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Interop.Excel;
using Worksheet = DocumentFormat.OpenXml.Spreadsheet.Worksheet;
using System.Globalization;

namespace EbayProject.Controllers
{
    [ValidationFilter(Role = 1)]
    public class AdminController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly DatabaseEntities de = new DatabaseEntities();

        public ActionResult Index(string msg = "", string color = "black", string way = "1")
        {
            ViewBag.ttlCard = new CardBL().GetActiveCardsList(de).Count();

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }


        #region Manage Cards
        public ActionResult ViewCards(string msg = "", string color = "black", string way = "1")
        {

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }


        [HttpPost]
        public ActionResult PostAddCard(HttpPostedFileBase cardFile, Card _card, string way = "")
        {

            Card obj = new Card()
            {
                Player = _card.Player.Trim(),
                Set = _card.Set.Trim(),
                Variation = _card.Variation.Trim(),
                Grade = _card.Grade.Trim(),
                SalePrice = _card.SalePrice,
                CardDate = Convert.ToDateTime(_card.CardDate).ToString("dd/MM/yyyy"),
                Link = _card.Link,
                Charts = _card.Charts,
                IsActive = 1,
                CreatedAt = DateTime.Now
            };

            if (cardFile != null && cardFile.ContentLength > 0)
            {
                string ReceiptExt = Path.GetExtension(cardFile.FileName);

                if (ReceiptExt.ToLower().Equals(".png") || ReceiptExt.ToLower().Equals(".jpg") || ReceiptExt.ToLower().Equals(".jpeg"))
                {
                    string Filename = Path.GetFileNameWithoutExtension(cardFile.FileName) + DateTime.Now.Ticks.ToString() + ReceiptExt; 

                    obj.ImgPath = "../Content/CardImages/" + Filename;

                    Filename = Path.Combine(Server.MapPath("../Content/CardImages/"), Filename);

                    cardFile.SaveAs(Filename);

                }
                else
                {
                    return RedirectToAction("ViewCards", "Admin", new { msg = "Only .png/.jpg/.jpeg files allowed", color = "red", way = way });
                }
            }
            else
            {
                obj.ImgPath = null;
            }

            bool chkCard = new CardBL().AddCard(obj, de);

            if (chkCard)
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Record inserted successfully!", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }


        [HttpPost]
        public ActionResult PostUpdateCard(HttpPostedFileBase cardFile, Card _card, string way = "")
        {
            Card card = new CardBL().GetActiveCardById(_card.Id, de);
            card.Player = _card.Player.Trim();
            card.Set = _card.Set.Trim();
            card.Variation = _card.Variation.Trim();
            card.Grade = _card.Grade.Trim();
            card.SalePrice = _card.SalePrice;
            card.CardDate = Convert.ToDateTime(_card.CardDate).ToString("dd/MM/yyyy");
            card.Link = _card.Link;
            card.Charts = _card.Charts;

            if (cardFile != null && cardFile.ContentLength > 0)
            {
                string ImagePath = Server.MapPath("../Content/CardImages/" + card.ImgPath);

                if (Directory.Exists(ImagePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(ImagePath);
                    try
                    {
                        foreach (FileInfo fi in dir.GetFiles())
                            fi.Delete();
                    }
                    catch
                    {
                        
                    }
                }

                string ReceiptExt = Path.GetExtension(cardFile.FileName);

                if (ReceiptExt.ToLower().Equals(".png") || ReceiptExt.ToLower().Equals(".jpg") || ReceiptExt.ToLower().Equals(".jpeg"))
                {
                    string Filename = Path.GetFileNameWithoutExtension(cardFile.FileName) + DateTime.Now.Ticks.ToString() + ReceiptExt;

                    card.ImgPath = "../Content/CardImages/" + Filename;

                    Filename = Path.Combine(Server.MapPath("../Content/CardImages/"), Filename);

                    cardFile.SaveAs(Filename);

                }
                else
                {
                    return RedirectToAction("ViewCards", "Admin", new { msg = "Only .png/.jpg/.jpeg files allowed", color = "red", way = way });
                }
            }
            

            bool chkCard = new CardBL().UpdateCard(card, de);

            if (chkCard)
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Record updated successfully!", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        public ActionResult DeleteCard(int id, string way = "")
        {
            Card card = new CardBL().GetActiveCardById(id, de);
            card.IsActive = 0;

            bool chkCard = new CardBL().UpdateCard(card, de);

            if (chkCard)
            {
                //string ImagePath = Server.MapPath("../Content/CardImages/" + card.ImgPath);

                //if (Directory.Exists(ImagePath))
                //{
                //    DirectoryInfo dir = new DirectoryInfo(ImagePath);
                //    try
                //    {
                //        foreach (FileInfo fi in dir.GetFiles())
                //            fi.Delete();
                //    }
                //    catch
                //    {

                //    }
                //}

                return RedirectToAction("ViewCards", "Admin", new { msg = "Record deleted successfully!", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [HttpPost]
        public ActionResult DeleteAllCard(string Pw, string way = "")
        {
            User user = new UserBL().GetActiveUsersList(de).Where(x => x.Email.Trim().ToLower() == gp.ValidateLoggedinUser().Email.Trim().ToLower() && StringCipher.Decrypt(x.Password).Equals(Pw)).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("ViewCards", "Admin", new { msg = "Incorrect Password!", color = "red", way = way });
            }

            List<Card> clist = new CardBL().GetActiveCardsList(de);

            foreach(Card c in clist)
            {
                bool chkCard = new CardBL().DeleteCard(c.Id, de);
            }

            return RedirectToAction("ViewCards", "Admin", new { msg = "Record cleared successfully!", color = "green", way = way });
        }

        public ActionResult UploadCard(string msg = "", string color = "black", string way = "1")
        {

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        public ActionResult PostUploadCardExcel(HttpPostedFileBase cardFile, string way = "")
        {

            if (cardFile!= null && cardFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(cardFile.FileName);

                if (extension.ToLower() == ".csv")
                {
                    return RedirectToAction("UploadCard", "Admin", new { msg = "Only .xlsx files allowed, Save File as .xlsx then try to upload", color = "red", way = way });
                }

                if (extension.ToLower() == ".xlsx")
                {
                    string Filename = "Uploaded File" + DateTime.Now.Ticks + extension;

                    Filename = Path.Combine(Server.MapPath("../Content/CSV/"), Filename);

                    cardFile.SaveAs(Filename);

                    //string check = "";
                    //if (extension.ToLower() == ".csv")
                    //{
                    //    string finalPath = Path.Combine(Server.MapPath("../Content/CSV/"), "Uplaoded Cards " + DateTime.Now.Ticks + ".xlsx");
                        
                    //    Application app = new Application();
                    //    Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Open(Filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //    wb.SaveAs(finalPath, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //    wb.Close();
                    //    app.Quit();

                    //    check = AddCardViaExcel(finalPath);
                    //}
                        
                    string check = AddCardViaExcel(Filename);


                    return RedirectToAction("UploadCard", "Admin", new { msg = check, color = "green", way = way });
                }
                else
                {
                    return RedirectToAction("UploadCard", "Admin", new { msg = "Only .xlsx/.csv files allowed", color = "red", way = way });
                }
            }
            else
            {
                return RedirectToAction("UploadCard", "Admin", new { message = "Empty files not allowed", color = "red", way = way });
            }

        }

        #region Add Cards Through CSV

        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        private static Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            try
            {
                Row row = GetRow(worksheet, rowIndex);

                if (row == null)
                    return null;

                return row.Elements<Cell>().Where(c => string.Compare
                       (c.CellReference.Value, columnName +
                       rowIndex, true) == 0).First();
            }
            catch
            {
                return null;
            }
        }

        //row wise insertion
        public string AddCardViaExcel(string path)
        {
            try
            {
                List<string> errorMsg = new List<string>();

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;
                        var cells = sheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                        var rows = sheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Row>();
                        Console.WriteLine("Row count = {0}", rows.LongCount());
                        Console.WriteLine("Cell count = {0}", cells.LongCount());
                        int rcount = rows.Count();
                        int ccount = cells.Count();


                        int i = 0;

                        char cellref = 'A';
                        int cellcount = 2;

                        foreach (Row row in rows)
                        {
                            Card c = new Card();

                            if (i == 9999)
                            {
                                break;
                            }

                            if (i > 0)
                            {
                                for (int j = 0; j <= 7; j++)
                                {
                                    string str;

                                    Cell cell = GetCell(sheet, cellref.ToString(), (uint)cellcount);
                                    if (cell != null)
                                    {
                                        if (cell.InnerText == "")
                                        {
                                            str = null;
                                        }
                                        else
                                        {
                                            float f;

                                            if (cell.DataType != null)
                                            {
                                                if (cell.DataType == CellValues.SharedString)
                                                {
                                                    int ssid = Convert.ToInt32(cell.CellValue.Text);
                                                    str = sst.ChildElements[ssid].InnerText;
                                                }
                                                else
                                                {
                                                    str = cell.CellValue.Text;
                                                }
                                            }
                                            else if (float.TryParse(cell.InnerText, out f))
                                            {
                                                str = cell.InnerText;
                                            }
                                            else
                                            {
                                                str = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str = null;
                                    }

                                    switch (j)
                                    {
                                        case 0:
                                            c.Player = str;
                                            break;
                                        case 1:
                                            c.Set = str;
                                            break;
                                        case 2:
                                            c.Variation = str;
                                            break;
                                        case 3:
                                            c.Grade = str;
                                            break;
                                        case 4:
                                            if (str != null)
                                            {
                                                try
                                                {
                                                    double newprice = Convert.ToDouble(str);
                                                    c.SalePrice = newprice.ToString();
                                                }
                                                catch
                                                {
                                                    c.SalePrice = null;
                                                }
                                            }
                                            break;
                                        case 5:
                                            if (str != null)
                                            {
                                                try
                                                {
                                                    DateTime dDate;
                                                    string finalDate = null;
                                                    if (DateTime.TryParseExact(str, "dd/MM/yyyy", null, DateTimeStyles.None, out dDate))
                                                    {
                                                        finalDate = string.Format("{0:MM/dd/yyyy}", dDate);
                                                    }

                                                    c.CardDate = Convert.ToDateTime(finalDate).ToString("MM/dd/yyyy");
                                                }
                                                catch
                                                {
                                                    c.CardDate = null;
                                                }
                                            }
                                            break;
                                        case 6:
                                            c.Link = str;
                                            break;
                                        case 7:
                                            c.Charts = str;
                                            break;
                                    }

                                    cellref++;

                                }


                                if (c.Player != null && c.Set != null && c.Variation != null && c.Grade != null)
                                {
                                    c.IsActive = 1;
                                    c.CreatedAt = DateTime.Now;

                                    bool chkCard = new CardBL().AddCard(c, de);
                                    if (!chkCard)
                                    {
                                        errorMsg.Add("~Data insertion failed at row # " + (i + 1));
                                    }
                                }
                                else
                                {
                                    errorMsg.Add("Required filed is empty at row # " + (i + 1));

                                    //if (c.Player == null)
                                    //{
                                    //    errorMsg.Add("Player name is empty at row # " + (i + 1));
                                    //}

                                    //if (c.Set == null)
                                    //{
                                    //    errorMsg.Add("Set is empty at row # " + (i + 1));
                                    //}

                                    //if (c.Variation == null)
                                    //{
                                    //    errorMsg.Add("Variation is empty at row # " + (i + 1));
                                    //}

                                    //if (c.Player == null)
                                    //{
                                    //    errorMsg.Add("Grade is empty at row # " + (i + 1));
                                    //}
                                }

                                cellref = 'A';
                                cellcount++;
                            }

                            i++;
                        }
                    }
                }

                if (errorMsg.Count == 0)
                {
                    return "Cards record uploaded successfully";
                }
                else
                {
                    string logPath = Server.MapPath("~/Content/ErrorLogs/Cards_Error_Log.xlsx");
                    return GeneralPurpose.GenerateErrorLog(errorMsg, logPath);
                }
            }
            catch(Exception ex)
            {
                return "Somethings' wrong";
            }
        }

        #endregion

        #endregion


        #region AJAX

        [HttpPost]
        public ActionResult GetCardList()
        {
            List<Card> clist = new CardBL().GetActiveCardsList(de).OrderBy(x => x.Player).ToList();

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null && sortColumnName != "0")
            {
                if (sortDirection == "asc")
                {
                    clist = clist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
                else
                {
                    clist = clist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
            }

            int totalrows = clist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                clist = clist.Where(x =>
                    x.Player.ToLower().Contains(searchValue.ToLower()) ||
                    x.Set.ToLower().Contains(searchValue.ToLower()) ||
                    x.Variation.ToLower().Contains(searchValue.ToLower()) ||
                    x.Grade.ToLower().Contains(searchValue.ToLower()) ||
                    (x.SalePrice!= null && x.SalePrice.ToLower().Contains(searchValue.ToLower())) ||
                    x.Charts.Contains(searchValue)
                ).ToList();
            }

            int totalrowsafterfilterinig = clist.Count();


            // pagination
            clist = clist.Skip(start).Take(length).ToList();

            List<CardDTO> cdto = new List<CardDTO>();

            foreach (Card c in clist)
            {
                CardDTO obj = new CardDTO()
                {
                    Id = c.Id,
                    EncId = GeneralPurpose.EncryptId(c.Id),
                    //ImgPath = c.ImgPath,
                    Player = c.Player,
                    Set = c.Set,
                    Variation = c.Variation,
                    Grade = c.Grade,
                    SalePrice = c.SalePrice,
                    CardDate = c.CardDate,
                    //Link = c.Link,
                    //Charts = c.Charts
                };

                if(obj.CardDate == "" || obj.CardDate == null)
                {
                    obj.CardDate = "---";
                }

                if (obj.SalePrice == "" || obj.SalePrice == null)
                {
                    obj.SalePrice = "--";
                }

                cdto.Add(obj);
            }

            return Json(new { data = cdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCardById(int id)
        {
            Card c = new CardBL().GetActiveCardById(id, de);


            CardDTO obj = new CardDTO()
            {
                Id = c.Id,
                EncId = GeneralPurpose.EncryptId(c.Id),
                ImgPath = c.ImgPath,
                Player = c.Player,
                Set = c.Set,
                Variation = c.Variation,
                Grade = c.Grade,
                SalePrice = c.SalePrice,
                //CardDate = Convert.ToDateTime(c.CardDate).ToString("yyy-MM-dd"),
                Link = c.Link,
                Charts = c.Charts
            };

            try
            {
                DateTime dDate;
                string finalDate = null;
                if (DateTime.TryParseExact(c.CardDate, "dd/MM/yyyy", null, DateTimeStyles.None, out dDate))
                {
                    finalDate = string.Format("{0:MM/dd/yyyy}", dDate);
                }

                obj.CardDate = Convert.ToDateTime(finalDate).ToString("yyy-MM-dd");
            }
            catch
            {
                obj.CardDate = null;
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public FileResult DownloadReport(string fileName = "")
        {
            var FileVirtualPath = "~/Content/ErrorLogs/" + fileName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }
    }
}