using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class Connection
    {

        private EmptyHandDBEntities dbContext;

        public Connection()
        {
            dbContext = new EmptyHandDBEntities();
        }

        public EmptyHandDBEntities GetContext()
        {
            return dbContext;
        }

    }
}
