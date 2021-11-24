using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbayProject.Models;
using EbayProject.BL;
using EbayProject.Helping_Classes;
using Newtonsoft.Json;

namespace EbayProject.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseEntities de = new DatabaseEntities();
        public ActionResult Index(string msg = "", string color = "black")
        {
            ViewBag.Players = new CardBL().GetActiveCardsList(de).Select(x => x.Player).Distinct().ToList();

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        public ActionResult GraphTest(string msg = "", string color = "black")
        {
            ViewBag.Players = new CardBL().GetActiveCardsList(de).Select(x => x.Player).Distinct().ToList();

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        #region AJAX

        [HttpPost]
        public ActionResult GetCardById(int id, string startDate = "", string endDate = "")
        {
            Card card = new CardBL().GetActiveCardById(id, de);
            if (card.Charts == null)
            {
                card.Charts = "[]";
            }
            List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);

            if (startDate != "" && endDate != "")
            {
                if (chartlist.Count > 0)
                {
                    chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
                }
            }

            double average = 0;
            if(chartlist.Sum(x=> x.price) != 0 & chartlist.Count != 0)
            {
                average = Math.Round(chartlist.Sum(x => x.price) / chartlist.Count, 2);
            }

            if (card != null)
            {
                CardDTO obj = new CardDTO()
                {
                    Id = card.Id,
                    EncId = GeneralPurpose.EncryptId(card.Id),
                    Player = card.Player,
                    Set = card.Set,
                    SalePrice = average.ToString(),
                    Variation = card.Variation,
                    Grade = card.Grade,
                    Link = card.Link
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetSetListByPlayer(string player = "")
        {
            var clist = new CardBL().GetActiveCardListBySearch(player, de).Select(x=> x.Set).Distinct().ToList();
            List<string> sets = new List<string>();
            foreach(string i in clist)
            {
                sets.Add(i);
            }

            return Json(sets, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetVariationListByPlayer(string player = "", string set = "")
        {
            var clist = new CardBL().GetActiveCardsList(de).Where(x=> x.Player == player && x.Set == set).Select(x => x.Variation).Distinct().ToList();
            List<string> variant = new List<string>();
            foreach (string i in clist)
            {
                variant.Add(i);
            }

            return Json(variant, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGradeListByPlayer(string player = "", string set = "", string variation = "")
        {
            var clist = new CardBL().GetActiveCardsList(de).Where(x => x.Player == player && x.Set == set && x.Variation == variation).Select(x => x.Grade).Distinct().ToList();
            List<string> grade = new List<string>();
            foreach (string i in clist)
            {
                grade.Add(i);
            }

            return Json(grade, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPlayerListExcept(string player)
        {
            var plist = new CardBL().GetActiveCardsList(de).Where(x=> x.Player.ToLower() != player.ToLower().Trim()).Select(x => x.Player).Distinct().ToList();
            List<string> playres = new List<string>();
            foreach (string i in plist)
            {
                playres.Add(i);
            }

            return Json(playres, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCardTableData(string player, string set, string variation, string grade)
        {
            Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower() ).FirstOrDefault();

            if (card != null)
            {
                CardDTO obj = new CardDTO()
                {
                    Id = card.Id,
                    EncId = GeneralPurpose.EncryptId(card.Id),
                    Player = card.Player,
                    Set = card.Set,
                    Variation = card.Variation,
                    Grade = card.Grade,
                    Link = card.Link
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult testData(int id)
        {
            Card card = new CardBL().GetActiveCardById(id, de);
            if (card.Charts == null)
            {
                card.Charts = "[]";
            }
            List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);

            List<ChartDTO> cdto = new List<ChartDTO>();

            foreach (ChartDTO i in chartlist)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price = Convert.ToDouble(i.price)
                };

                cdto.Add(obj);
            }

            cdto = cdto.OrderBy(x => x.date).ToList();

            return Json(cdto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SortDateArray(string[] dates)
        {
            if (dates != null)
            {
                Array.Sort(dates);
                //List<ChartDTO> dateslist = new List<ChartDTO>();
                //foreach(string i in dates)
                //{
                //    ChartDTO obj = new ChartDTO()
                //    {
                //        date = i
                //    };
                //    dateslist.Add(obj);
                //}
                //dateslist = dateslist.OrderBy(x => x.date).ToList();

                //string[] finalDates = new string[dateslist.Count()];

                return Json(dates.Distinct(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(dates, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public ActionResult GetChartData(string player, string set, string variation, string grade, string startDate = "", string endDate = "")
        {
            Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower() ).FirstOrDefault();

            if(card.Charts == null)
            {
                card.Charts = "[]";
            }
            List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);
            

            if (startDate != "" && endDate != "")
            {
                if (chartlist.Count > 0)
                {
                    chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
                }
            }

            chartlist = chartlist.OrderBy(x => x.date).ToList();

            List<ChartDTO> cdto = new List<ChartDTO>();
            double avg = 0;
            int cc = 1;
            foreach(ChartDTO i in chartlist)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price = Convert.ToDouble(i.price)
                };

                if(cc == 1)
                {
                    obj.average = Math.Round(Convert.ToDouble(i.price), 2);

                    avg += Convert.ToDouble(i.price);
                }
                else
                {
                    avg += Convert.ToDouble(i.price);
                    obj.average = Math.Round(avg / cc, 2);
                }

                cc++;
                cdto.Add(obj);
            }

            if (cdto.Count > 0)
            {
                cdto[0].Id = card.Id;
                //cdto[0].average = Math.Round(cdto.Sum(x => x.price)/cdto.Count,2);
                
                //if (startDate != "" && endDate != "")
                //{
                //    cdto = cdto.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();

                //    if (cdto.Count == 0)
                //    {
                //        ChartDTO obj = new ChartDTO()
                //        {
                //            Id2 = -1,
                //            Id = card.Id,
                //            average = 0
                //        };
                //        cdto.Add(obj);
                //    }
                //}
            }
            else
            {
                ChartDTO obj = new ChartDTO()
                {
                    Id2 = -1,
                    Id = card.Id,
                    average = 0
                };

                cdto.Add(obj);
            }

            return Json(cdto, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GetChart2Data(string player, string set, string variation, string grade, string player2, string set2, string variation2, string grade2, string startDate = "", string endDate = "")
        //{
        //    Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower() ).FirstOrDefault();
        //    Card card2 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player2.ToLower() && x.Set.ToLower() == set2.ToLower() && x.Variation.ToLower() == variation2.ToLower() && x.Grade.ToLower() == grade2.ToLower() ).FirstOrDefault();

        //    List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);
        //    List<ChartDTO> chartlist2 = JsonConvert.DeserializeObject<List<ChartDTO>>(card2.Charts);

        //    if (startDate != "" && endDate != "")
        //    {
        //        if (chartlist.Count > 0)
        //        {
        //            chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
        //        }

        //        if (chartlist2.Count > 0)
        //        {
        //            chartlist2 = chartlist2.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
        //        }
        //    }

        //    List<ChartDTO> cdto = new List<ChartDTO>();

        //    double priceSum = 0;
        //    int priceCount = 0;

        //    foreach (ChartDTO i in chartlist)
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
        //            price = Convert.ToDouble(i.price)
        //        };

        //        priceSum += Convert.ToDouble(i.price);
        //        priceCount++;

        //        cdto.Add(obj);
        //    }


        //    double priceSum2 = 0;
        //    int priceCount2 = 0;

        //    foreach (ChartDTO i in chartlist2)
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
        //            price2 = Convert.ToDouble(i.price)
        //        };

        //        priceSum2 += Convert.ToDouble(i.price);
        //        priceCount2++;

        //        cdto.Add(obj);
        //    }

        //    if (cdto.Count > 0)
        //    {
        //        cdto = cdto.OrderBy(x => x.date).ToList();

        //        cdto[0].Id = card.Id;

        //        if (priceSum == 0 || priceCount == 0)
        //        {
        //            cdto[0].average = 0;
        //        }
        //        else
        //        {
        //            cdto[0].average = Math.Round(priceSum/priceCount, 2);
        //        }


        //        cdto[0].Id2 = card2.Id;

        //        if(priceSum2 == 0 || priceCount2 == 0)
        //        {
        //            cdto[0].average2 = 0;
        //        }
        //        else
        //        {
        //            cdto[0].average2 = Math.Round(priceSum2/priceCount2, 2);
        //        }

        //        //if (startDate != "" && endDate != "")
        //        //{
        //        //    cdto = cdto.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();

        //        //    if(cdto.Count == 0)
        //        //    {
        //        //        ChartDTO obj = new ChartDTO()
        //        //        {
        //        //            Id = card.Id,
        //        //            average = 0,
        //        //            Id2 = card2.Id,
        //        //            average2 = 0,
        //        //        };

        //        //        cdto.Add(obj);
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            Id = card.Id,
        //            average = 0,
        //            Id2 = card2.Id,
        //            average2 = 0,
        //        };
        //        cdto.Add(obj);
        //    }


        //    return Json(cdto, JsonRequestBehavior.AllowGet);
        //}

        
        [HttpPost]
        public ActionResult GetChart2Data(string player, string set, string variation, string grade, string player2, string set2, string variation2, string grade2, string startDate = "", string endDate = "")
        {
            Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower()).FirstOrDefault();
            Card card2 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player2.ToLower() && x.Set.ToLower() == set2.ToLower() && x.Variation.ToLower() == variation2.ToLower() && x.Grade.ToLower() == grade2.ToLower()).FirstOrDefault();
            if (card.Charts == null)
            {
                card.Charts = "[]";
            }
            if (card2.Charts == null)
            {
                card2.Charts = "[]";
            }
            List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);
            List<ChartDTO> chartlist2 = JsonConvert.DeserializeObject<List<ChartDTO>>(card2.Charts);

            foreach(ChartDTO i in chartlist2)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price2 = Convert.ToDouble(i.price)
                };

                chartlist.Add(obj);
            }

            if (startDate != "" && endDate != "")
            {
                if (chartlist.Count > 0)
                {
                    chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
                }
            }

            chartlist = chartlist.OrderBy(x => x.date).ToList();

            List<ChartDTO> cdto = new List<ChartDTO>();


            double avg = 0;
            double avg2 = 0;
            int cc = 1;

            foreach (ChartDTO i in chartlist)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price = Convert.ToDouble(i.price),
                    price2 = Convert.ToDouble(i.price2)
                };

                if (cc == 1)
                {
                    obj.average = Math.Round(Convert.ToDouble(i.price), 2);

                    avg += Convert.ToDouble(i.price);

                    obj.average2 = Math.Round(Convert.ToDouble(i.price2), 2);

                    avg2 += Convert.ToDouble(i.price2);
                }
                else
                {
                    avg += Convert.ToDouble(i.price);
                    obj.average = Math.Round(avg / cc, 2);

                    avg2 += Convert.ToDouble(i.price2);
                    obj.average2 = Math.Round(avg2 / cc, 2);
                }

                cc++;

                cdto.Add(obj);
            }
            
            if (cdto.Count > 0)
            {
                cdto = cdto.OrderBy(x => x.date).ToList();

                cdto[0].Id = card.Id;
                cdto[0].Id2 = card2.Id;
            }
            else
            {
                ChartDTO obj = new ChartDTO()
                {
                    Id = card.Id,
                    average = 0,
                    Id2 = card2.Id,
                    average2 = 0,
                };
                cdto.Add(obj);
            }


            return Json(cdto, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetChart3Data(string player, string set, string variation, string grade, string player2, string set2, string variation2, string grade2, string player3, string set3, string variation3, string grade3, string startDate = "", string endDate = "")
        {
            Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower()).FirstOrDefault();
            Card card2 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player2.ToLower() && x.Set.ToLower() == set2.ToLower() && x.Variation.ToLower() == variation2.ToLower() && x.Grade.ToLower() == grade2.ToLower()).FirstOrDefault();
            Card card3 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player3.ToLower() && x.Set.ToLower() == set3.ToLower() && x.Variation.ToLower() == variation3.ToLower() && x.Grade.ToLower() == grade3.ToLower()).FirstOrDefault();
            if (card.Charts == null)
            {
                card.Charts = "[]";
            }
            if (card2.Charts == null)
            {
                card2.Charts = "[]";
            }
            if (card3.Charts == null)
            {
                card3.Charts = "[]";
            }
            List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);
            List<ChartDTO> chartlist2 = JsonConvert.DeserializeObject<List<ChartDTO>>(card2.Charts);
            List<ChartDTO> chartlist3 = JsonConvert.DeserializeObject<List<ChartDTO>>(card3.Charts);

            foreach (ChartDTO i in chartlist2)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price2 = Convert.ToDouble(i.price)
                };

                chartlist.Add(obj);
            }

            foreach (ChartDTO i in chartlist3)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price3 = Convert.ToDouble(i.price)
                };

                chartlist.Add(obj);
            }


            if (startDate != "" && endDate != "")
            {
                if (chartlist.Count > 0)
                {
                    chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
                }
            }

            chartlist = chartlist.OrderBy(x => x.date).ToList();
            
            List<ChartDTO> cdto = new List<ChartDTO>();

            double avg = 0;
            double avg2 = 0;
            double avg3 = 0;
            int cc = 1;

            foreach (ChartDTO i in chartlist)
            {
                ChartDTO obj = new ChartDTO()
                {
                    date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
                    price = Convert.ToDouble(i.price),
                    price2 = Convert.ToDouble(i.price2),
                    price3 = Convert.ToDouble(i.price3)
                };

                if (cc == 1)
                {
                    obj.average = Math.Round(Convert.ToDouble(i.price), 2);
                    avg += Convert.ToDouble(i.price);

                    obj.average2 = Math.Round(Convert.ToDouble(i.price2), 2);
                    avg2 += Convert.ToDouble(i.price2);

                    obj.average3 = Math.Round(Convert.ToDouble(i.price3), 2);
                    avg3 += Convert.ToDouble(i.price3);
                }
                else
                {
                    avg += Convert.ToDouble(i.price);
                    obj.average = Math.Round(avg / cc, 2);

                    avg2 += Convert.ToDouble(i.price2);
                    obj.average2 = Math.Round(avg2 / cc, 2);

                    avg3 += Convert.ToDouble(i.price3);
                    obj.average3 = Math.Round(avg3 / cc, 2);
                }

                cc++;
                cdto.Add(obj);
            }


            if (cdto.Count > 0)
            {
                cdto = cdto.OrderBy(x => x.date).ToList();

                cdto[0].Id = card.Id;
                cdto[0].Id2 = card2.Id;
                cdto[0].Id3 = card3.Id;
            }
            else
            {
                ChartDTO obj = new ChartDTO()
                {
                    Id = card.Id,
                    average = 0,
                    Id2 = card2.Id,
                    average2 = 0,
                    Id3 = card3.Id,
                    average3 = 0
                };

                cdto.Add(obj);
            }

            return Json(cdto, JsonRequestBehavior.AllowGet);
        }



        //[HttpPost]
        //public ActionResult GetChart3Data(string player, string set, string variation, string grade, string player2, string set2, string variation2, string grade2, string player3, string set3, string variation3, string grade3, string startDate = "", string endDate = "")
        //{
        //    Card card = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player.ToLower() && x.Set.ToLower() == set.ToLower() && x.Variation.ToLower() == variation.ToLower() && x.Grade.ToLower() == grade.ToLower()).FirstOrDefault();
        //    Card card2 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player2.ToLower() && x.Set.ToLower() == set2.ToLower() && x.Variation.ToLower() == variation2.ToLower() && x.Grade.ToLower() == grade2.ToLower()).FirstOrDefault();
        //    Card card3 = new CardBL().GetActiveCardsList(de).Where(x => x.Player.ToLower() == player3.ToLower() && x.Set.ToLower() == set3.ToLower() && x.Variation.ToLower() == variation3.ToLower() && x.Grade.ToLower() == grade3.ToLower()).FirstOrDefault();

        //    List<ChartDTO> chartlist = JsonConvert.DeserializeObject<List<ChartDTO>>(card.Charts);
        //    List<ChartDTO> chartlist2 = JsonConvert.DeserializeObject<List<ChartDTO>>(card2.Charts);
        //    List<ChartDTO> chartlist3 = JsonConvert.DeserializeObject<List<ChartDTO>>(card3.Charts);

        //    if (startDate != "" && endDate != "")
        //    {
        //        if (chartlist.Count > 0)
        //        {
        //            chartlist = chartlist.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
        //        }

        //        if (chartlist2.Count > 0)
        //        {
        //            chartlist2 = chartlist2.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
        //        }

        //        if (chartlist3.Count > 0)
        //        {
        //            chartlist3 = chartlist3.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();
        //        }
        //    }

        //    List<ChartDTO> cdto = new List<ChartDTO>();

        //    double priceSum = 0;
        //    int priceCount = 0;
        //    foreach (ChartDTO i in chartlist)
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
        //            price = Convert.ToDouble(i.price)
        //        };

        //        priceSum += Convert.ToDouble(i.price);
        //        priceCount++;

        //        cdto.Add(obj);
        //    }


        //    double priceSum2 = 0;
        //    int priceCount2 = 0;

        //    foreach (ChartDTO i in chartlist2)
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
        //            price2 = Convert.ToDouble(i.price)
        //        };

        //        priceSum2 += Convert.ToDouble(i.price);
        //        priceCount2++;

        //        cdto.Add(obj);
        //    }


        //    double priceSum3 = 0;
        //    int priceCount3 = 0;

        //    foreach (ChartDTO i in chartlist3)
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            date = Convert.ToDateTime(i.date).ToString("MM/dd/yyyy"),
        //            price3 = Convert.ToDouble(i.price)
        //        };

        //        priceSum3 += Convert.ToDouble(i.price);
        //        priceCount3++;

        //        cdto.Add(obj);
        //    }


        //    if (cdto.Count > 0)
        //    {
        //        cdto = cdto.OrderBy(x => x.date).ToList();

        //        cdto[0].Id = card.Id;

        //        if (priceSum == 0 || priceCount == 0)
        //        {
        //            cdto[0].average = 0;
        //        }
        //        else
        //        {
        //            cdto[0].average = Math.Round(priceSum/priceCount, 2);
        //        }


        //        cdto[0].Id2 = card2.Id;

        //        if (priceSum2 == 0 || priceCount2 == 0)
        //        {
        //            cdto[0].average2 = 0;
        //        }
        //        else
        //        {
        //            cdto[0].average2 = Math.Round(priceSum2/priceCount2, 2);
        //        }


        //        cdto[0].Id3 = card3.Id;

        //        if (priceSum3 == 0 || priceCount3 == 0)
        //        {
        //            cdto[0].average3 = 0;
        //        }
        //        else
        //        {
        //            cdto[0].average3 = Math.Round(priceSum3/priceCount3, 2);
        //        }

        //        //if (startDate != "" && endDate != "")
        //        //{
        //        //    cdto = cdto.Where(x => Convert.ToDateTime(x.date).Date >= Convert.ToDateTime(startDate).Date && Convert.ToDateTime(x.date).Date <= Convert.ToDateTime(endDate).Date).ToList();

        //        //    if(cdto.Count == 0)
        //        //    {
        //        //        ChartDTO obj = new ChartDTO()
        //        //        {
        //        //            Id = card.Id,
        //        //            average = 0,
        //        //            Id2 = card2.Id,
        //        //            average2 = 0,
        //        //            Id3 = card3.Id,
        //        //            average3 = 0
        //        //        };
        //        //        cdto.Add(obj);
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        ChartDTO obj = new ChartDTO()
        //        {
        //            Id = card.Id,
        //            average = 0,
        //            Id2 = card2.Id,
        //            average2 = 0,
        //            Id3 = card3.Id,
        //            average3 = 0
        //        };

        //        cdto.Add(obj);
        //    }

        //    return Json(cdto, JsonRequestBehavior.AllowGet);
        //}

        #endregion
    }
}