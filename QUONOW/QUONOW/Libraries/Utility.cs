using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Models;

namespace QUONOW.Libraries
{
    public class Utility
    {
        QUONOWEntities db;
        UnitOfWork unit;
        public Utility()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }


        public dynamic GetUserDetailsByToken(string strtoken)
        {
            dynamic userDetails = null;
            try
            {
                userDetails = unit._userRepository.SelectAll().Where(x => x.Token.ToUpper() == strtoken.ToUpper()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return userDetails;
        }


        public int GetProductPrice(Guid productId)
        {
            try
            {
                return Convert.ToInt32(this.unit._productRepository.SelectAll().Where(x => x.Id == productId).FirstOrDefault().Price);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}