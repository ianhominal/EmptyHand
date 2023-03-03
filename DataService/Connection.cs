using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public static class Context
    {

        public static void AddGameHeader(GameHeader gH)
        {
            using (EmptyHandDBEntities context = new EmptyHandDBEntities())
            {

                //aca habria que guardar en la bd y esperar que acepte el otro player
                context.GameHeaders.Add(gH);
                context.SaveChanges();

            }
        }


        public static GameHeader GetGameHeader(string guid)
        {
            RefreshGameData(guid);
            using (EmptyHandDBEntities context = new EmptyHandDBEntities())
            {
                Guid gameId;
                if (Guid.TryParse(guid, out gameId))
                {
                    var gameHeader = context.GameHeaders.Include("GameRound").Where(g => g.GameId == gameId)?.FirstOrDefault();

                    return gameHeader;
                }
                else return null;
            }
        }

        public static void RefreshGameData(string guid)
        {
            using (EmptyHandDBEntities context = new EmptyHandDBEntities())
            {
                GameHeader headerEntity = null;
                Guid gameId;
                
                if (Guid.TryParse(guid, out gameId))
                {
                    headerEntity = context.GameHeaders.Where(g => g.GameId == gameId)?.FirstOrDefault();
                }
                if (headerEntity != null)
                {
                    if(headerEntity.GameRound != null)
                    {
                        context.Entry(headerEntity.GameRound).Reload();
                    }
                    context.Entry(headerEntity).Reload();
                }
            }
        }


        public static void UpdateGameEntity(GameHeader gH)
        {
            using (EmptyHandDBEntities context = new EmptyHandDBEntities())
            {
                // Obtiene la entidad GameHeader y la entidad GameRound del contexto
                GameHeader headerEntity = context.GameHeaders.Include("GameRound").SingleOrDefault(g => g.GameId == gH.GameId);

                if (headerEntity != null)
                {
                    // Actualiza la entidad GameHeader con los nuevos valores
                    context.Entry(headerEntity).CurrentValues.SetValues(gH);

                    // Actualiza la entidad GameRound si existe
                    if (headerEntity.GameRound != null && gH.GameRound != null)
                    {
                        context.Entry(headerEntity.GameRound).CurrentValues.SetValues(gH.GameRound);
                    }
                    else if (gH.GameRound != null)
                    {
                        // Crea una nueva entidad GameRound y asóciala al GameHeader
                        headerEntity.GameRound = gH.GameRound;
                    }

                    // Guarda los cambios en la base de datos
                    context.SaveChanges();
                }
            }
        }
    }
}
