﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EbayProject.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DatabaseEntities : DbContext
    {
        public DatabaseEntities()
            : base("name=DatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<Card> sp_AddUpdateCard(string statementType, Nullable<int> id, string imgPath, string player, string set, string variation, string grade, string salePrice, string cardDate, string link, string charts, Nullable<int> isActive, Nullable<System.DateTime> createdAt)
        {
            var statementTypeParameter = statementType != null ?
                new ObjectParameter("StatementType", statementType) :
                new ObjectParameter("StatementType", typeof(string));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var imgPathParameter = imgPath != null ?
                new ObjectParameter("ImgPath", imgPath) :
                new ObjectParameter("ImgPath", typeof(string));
    
            var playerParameter = player != null ?
                new ObjectParameter("Player", player) :
                new ObjectParameter("Player", typeof(string));
    
            var setParameter = set != null ?
                new ObjectParameter("Set", set) :
                new ObjectParameter("Set", typeof(string));
    
            var variationParameter = variation != null ?
                new ObjectParameter("Variation", variation) :
                new ObjectParameter("Variation", typeof(string));
    
            var gradeParameter = grade != null ?
                new ObjectParameter("Grade", grade) :
                new ObjectParameter("Grade", typeof(string));
    
            var salePriceParameter = salePrice != null ?
                new ObjectParameter("SalePrice", salePrice) :
                new ObjectParameter("SalePrice", typeof(string));
    
            var cardDateParameter = cardDate != null ?
                new ObjectParameter("CardDate", cardDate) :
                new ObjectParameter("CardDate", typeof(string));
    
            var linkParameter = link != null ?
                new ObjectParameter("Link", link) :
                new ObjectParameter("Link", typeof(string));
    
            var chartsParameter = charts != null ?
                new ObjectParameter("Charts", charts) :
                new ObjectParameter("Charts", typeof(string));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(int));
    
            var createdAtParameter = createdAt.HasValue ?
                new ObjectParameter("CreatedAt", createdAt) :
                new ObjectParameter("CreatedAt", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_AddUpdateCard", statementTypeParameter, idParameter, imgPathParameter, playerParameter, setParameter, variationParameter, gradeParameter, salePriceParameter, cardDateParameter, linkParameter, chartsParameter, isActiveParameter, createdAtParameter);
        }
    
        public virtual ObjectResult<Card> sp_AddUpdateCard(string statementType, Nullable<int> id, string imgPath, string player, string set, string variation, string grade, string salePrice, string cardDate, string link, string charts, Nullable<int> isActive, Nullable<System.DateTime> createdAt, MergeOption mergeOption)
        {
            var statementTypeParameter = statementType != null ?
                new ObjectParameter("StatementType", statementType) :
                new ObjectParameter("StatementType", typeof(string));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var imgPathParameter = imgPath != null ?
                new ObjectParameter("ImgPath", imgPath) :
                new ObjectParameter("ImgPath", typeof(string));
    
            var playerParameter = player != null ?
                new ObjectParameter("Player", player) :
                new ObjectParameter("Player", typeof(string));
    
            var setParameter = set != null ?
                new ObjectParameter("Set", set) :
                new ObjectParameter("Set", typeof(string));
    
            var variationParameter = variation != null ?
                new ObjectParameter("Variation", variation) :
                new ObjectParameter("Variation", typeof(string));
    
            var gradeParameter = grade != null ?
                new ObjectParameter("Grade", grade) :
                new ObjectParameter("Grade", typeof(string));
    
            var salePriceParameter = salePrice != null ?
                new ObjectParameter("SalePrice", salePrice) :
                new ObjectParameter("SalePrice", typeof(string));
    
            var cardDateParameter = cardDate != null ?
                new ObjectParameter("CardDate", cardDate) :
                new ObjectParameter("CardDate", typeof(string));
    
            var linkParameter = link != null ?
                new ObjectParameter("Link", link) :
                new ObjectParameter("Link", typeof(string));
    
            var chartsParameter = charts != null ?
                new ObjectParameter("Charts", charts) :
                new ObjectParameter("Charts", typeof(string));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(int));
    
            var createdAtParameter = createdAt.HasValue ?
                new ObjectParameter("CreatedAt", createdAt) :
                new ObjectParameter("CreatedAt", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_AddUpdateCard", mergeOption, statementTypeParameter, idParameter, imgPathParameter, playerParameter, setParameter, variationParameter, gradeParameter, salePriceParameter, cardDateParameter, linkParameter, chartsParameter, isActiveParameter, createdAtParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCardById(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCardById", idParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCardById(Nullable<int> id, MergeOption mergeOption)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCardById", mergeOption, idParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCards()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCards");
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCards(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCards", mergeOption);
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCardsBySearch(string player)
        {
            var playerParameter = player != null ?
                new ObjectParameter("Player", player) :
                new ObjectParameter("Player", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCardsBySearch", playerParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetActiveCardsBySearch(string player, MergeOption mergeOption)
        {
            var playerParameter = player != null ?
                new ObjectParameter("Player", player) :
                new ObjectParameter("Player", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetActiveCardsBySearch", mergeOption, playerParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetAllCards()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetAllCards");
        }
    
        public virtual ObjectResult<Card> sp_GetAllCards(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetAllCards", mergeOption);
        }
    
        public virtual ObjectResult<Card> sp_GetCardById(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetCardById", idParameter);
        }
    
        public virtual ObjectResult<Card> sp_GetCardById(Nullable<int> id, MergeOption mergeOption)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Card>("sp_GetCardById", mergeOption, idParameter);
        }
    }
}
