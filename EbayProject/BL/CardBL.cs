using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbayProject.Models;
using EbayProject.DAL;

namespace EbayProject.BL
{
    public class CardBL
    {

        public List<Card> GetAllCardsList(DatabaseEntities de)
        {
            return new CardDAL().GetAllCardsList(de);
        }

        public List<Card> GetActiveCardsList(DatabaseEntities de)
        {
            return new CardDAL().GetActiveCardsList(de);
        }

        public Card GetCardById(int id, DatabaseEntities de)
        {
            return new CardDAL().GetCardById(id, de);
        }

        public Card GetActiveCardById(int id, DatabaseEntities de)
        {
            return new CardDAL().GetActiveCardById(id, de);
        }

        public List<Card> GetActiveCardListBySearch(string player, DatabaseEntities de)
        {
            return new CardDAL().GetActiveCardListBySearch(player, de);
        }

        
        public bool AddCard(Card Card, DatabaseEntities de)
        {
            if (Card.Player == "" || Card.Set == "" || Card.Variation == "" || Card.Grade == "" || Card.Player == null || Card.Set == null || Card.Variation == null || Card.Grade == null)
            {
                return false;
            }
            else
            {
                return new CardDAL().AddCard(Card, de);
            }
        }


        public bool UpdateCard(Card Card, DatabaseEntities de)
        {
            if (Card.Player == "" || Card.Set == "" || Card.Variation == "" || Card.Grade == "" || Card.Player == null || Card.Set == null || Card.Variation == null || Card.Grade == null)
            {
                return false;
            }
            else
            {
                return new CardDAL().UpdateCard(Card, de);
            }
        }

        public bool DeleteCard(int id, DatabaseEntities de)
        {
            return new CardDAL().DeleteCard(id, de);
        }

    }
}