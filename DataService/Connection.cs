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

        public GameHeader GetGameHeader(string guid)
        {
            var gameId = Guid.Parse(guid);
            return dbContext.GameHeaders.Where(g => g.GameId == gameId)?.FirstOrDefault();
        }

        public void SaveChanges()
        { dbContext.SaveChanges(); }

    }
}
