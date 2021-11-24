using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbayProject.Models;
using EbayProject.BL;
using System.Security.Claims;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EbayProject.Helping_Classes
{
    public class GeneralPurpose
    {
        DatabaseEntities de = new DatabaseEntities();
        public User ValidateLoggedinUser()
        {
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal; // Get the claims values
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            User loggedInUser = new UserBL().GetActiveUserById(Convert.ToInt32(userId), de);

            return loggedInUser;
        }

        public bool ValidateEmail(string email = "", int id = -1)
        {
            int emailCount = 0;

            if (id != -1)
            {
                emailCount = new UserBL().GetActiveUsersList(de).Where(x => x.Email.ToLower() == email.ToLower() && x.Id != id).Count();
            }
            else
            {
                emailCount = new UserBL().GetActiveUsersList(de).Where(x => x.Email.ToLower() == email.ToLower()).Count();
            }

            if (emailCount > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string EncryptId(int id)
        {
            string stringToEncrypt = id.ToString();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static int DecryptId(string EncId)
        {
            if (EncId.Contains(' '))
            {
                EncId = EncId.Replace(' ', '+');
            }
            int id = -1;
            id = Decrypt(HttpUtility.UrlDecode(EncId));
            string str = "";
            if (id == 0)
            {
                str = HttpUtility.UrlEncode(EncId);
                id = Decrypt(HttpUtility.UrlDecode(str));
            }
            return id;
        }

        public static int Decrypt(string EncId)
        {
            byte[] inputByteArray = new byte[EncId.Length + 1];
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(EncId);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return Convert.ToInt32(encoding.GetString(ms.ToArray()));
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static string GenerateErrorLog(List<string> message, string path)
        {
            try
            {
                SLDocument sl = new SLDocument();

                sl.AddWorksheet("sheet1");

                SLStyle style1 = sl.CreateStyle();
                style1.Font.FontSize = 12;
                style1.Font.Bold = true;

                SLStyle bgColor = sl.CreateStyle();
                bgColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Pink, System.Drawing.Color.Pink);


                sl.SetCellValue("A1", "Error List (" + DateTime.Now.ToString("MM/dd/yyyy H:mm") + ")");
                sl.SetCellStyle("A1", style1);
                sl.MergeWorksheetCells(1, 1, 1, 5);
                sl.FreezePanes(1, 5);
                
                int row = 2;
                foreach (string msg in message)
                {
                    if (msg.Contains("~"))
                    {
                        sl.SetCellValue(row, 1, string.Format(msg.Substring(1)));
                        sl.MergeWorksheetCells(row, 1, row, 5);
                        sl.SetCellStyle(row, 1, row, 5, bgColor);
                    }
                    else
                    {
                        sl.SetCellValue(row, 1, string.Format(msg));
                        sl.MergeWorksheetCells(row, 1, row, 5);
                    }

                    row++;
                }

                sl.SaveAs(path);

                return "1";
            }
            catch
            {
                return "0";
            }
        }

    }
}