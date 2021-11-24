using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbayProject.Models;
using EbayProject.DAL;

namespace EbayProject.BL
{
    public class UserBL
    {
        public List<User> GetAllUsersList(DatabaseEntities de)
        {
            return new UserDAL().GetAllUsersList(de);
        }

        public List<User> GetActiveUsersList(DatabaseEntities de)
        {
            return new UserDAL().GetActiveUsersList(de);
        }

        public User GetUserById(int id, DatabaseEntities de)
        {
            return new UserDAL().GetUserById(id, de);
        }

        public User GetActiveUserById(int id, DatabaseEntities de)
        {
            return new UserDAL().GetActiveUserById(id, de);
        }

        public bool AddUser(User user, DatabaseEntities de)
        {
            if (user.Name == "" || user.Contact == "" || user.Email == "" || user.Password == "" || user.Name == null || user.Contact == null || user.Email == null || user.Password == null)
            {
                return false;
            }
            else
            {
                return new UserDAL().AddUser(user, de);
            }
        }


        public bool UpdateUser(User user, DatabaseEntities de)
        {
            if (user.Name == "" || user.Contact == "" || user.Email == "" || user.Password == "" || user.Name == null || user.Contact == null || user.Email == null || user.Password == null)
            {
                return false;
            }
            else
            {
                return new UserDAL().UpdateUser(user, de);
            }
        }

        public bool DeleteUser(int id, DatabaseEntities de)
        {
            return new UserDAL().DeleteUser(id, de);
        }

    }
}