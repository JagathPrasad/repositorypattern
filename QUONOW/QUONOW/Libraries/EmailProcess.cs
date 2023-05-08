using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUONOW.Models;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Libraries;
namespace QUONOW.Libraries
{
    public class EmailProcess
    {

        UnitOfWork unit;
        public EmailProcess()
        {
            QUONOWEntities db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }


        public void CheckSendEmail()
        {
            try
            {
                var getEmailList = this.unit._EmailRepository.SelectAll()
                                   .Where(x => x.IsSent == false && x.IsDeleted == false)
                                   .OrderByDescending(x => x.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }

        }
    }
}