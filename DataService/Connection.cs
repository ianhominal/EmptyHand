using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class Context
    {

        private EmptyHandDBEntities dbContext;
        public Context()
        {
            dbContext = new EmptyHandDBEntities();

            //dbContext.Configuration.ProxyCreationEnabled = false;
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }

        public EmptyHandDBEntities GetContext()
        {
            return dbContext;
        }

        public void AddGameHeader(GameHeader gH)
        {
            //aca habria que guardar en la bd y esperar que acepte el otro player
            dbContext.GameHeaders.Add(gH);
            SaveChanges();
        }
        public EmptyHandDBEntities UpdateContext()
        {
            dbContext = new EmptyHandDBEntities();
            return dbContext;
        }


        public GameHeader GetGameHeader(string guid)
        {
            Guid gameId;
            if (Guid.TryParse(guid, out gameId))
            {
                return dbContext.GameHeaders.Where(g => g.GameId == gameId)?.FirstOrDefault();
            }
            else return null;
        }

        public void SaveChanges()
        { 
            dbContext.SaveChanges();
        }

    }
}
