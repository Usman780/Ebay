using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbayProject.Models;

namespace EbayProject.DAL
{
    public class CardDAL
    {

        public List<Card> GetAllCardsList(DatabaseEntities de)
        {
            //return de.Cards.ToList();
            return de.sp_GetAllCards().ToList();
        }

        public List<Card> GetActiveCardsList(DatabaseEntities de)
        {
            //return de.Cards.Where(x => x.IsActive == 1).ToList();
            return de.sp_GetActiveCards().ToList();
        }

        public Card GetCardById(int id, DatabaseEntities de)
        {
            //return de.Cards.Where(x => x.Id == id).FirstOrDefault();
            return de.sp_GetCardById(id).FirstOrDefault();
        }

        public Card GetActiveCardById(int id, DatabaseEntities de)
        {
            //return de.Cards.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
            return de.sp_GetActiveCardById(id).FirstOrDefault();
        }

        public List<Card> GetActiveCardListBySearch(string player, DatabaseEntities de)
        {
            //return de.Cards.Where(x => x.Player == player && x.IsActive == 1).ToList();
            return de.sp_GetActiveCardsBySearch(player).ToList();
        }

        public bool AddCard(Card Card, DatabaseEntities de)
        {
            try
            {
                de.Cards.Add(Card);
                //de.sp_AddUpdateCard("Insert", Card.Id, Card.ImgPath, Card.Player, Card.Set, Card.Variation, Card.Grade, Card.SalePrice, Card.CardDate, Card.Link, Card.Charts, Card.IsActive, Card.CreatedAt);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCard(Card Card, DatabaseEntities de)
        {
            try
            {
                de.Entry(Card).State = System.Data.Entity.EntityState.Modified;
                //de.sp_AddUpdateCard("Update", Card.Id, Card.ImgPath, Card.Player, Card.Set, Card.Variation, Card.Grade, Card.SalePrice, Card.CardDate, Card.Link, Card.Charts, Card.IsActive, Card.CreatedAt);
                de.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCard(int id, DatabaseEntities de)
        {
            try
            {
                de.Cards.Remove(de.Cards.Where(x => x.Id == id).FirstOrDefault());
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}