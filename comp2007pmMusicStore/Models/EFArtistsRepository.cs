using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace comp2007pmMusicStore.Models
{
    public class EFArtistsRepository : IMockArtistsRepository
    {
        // db conn moved here from ArtistsController
        private MusicStoreModel db = new MusicStoreModel();

        public IQueryable<Artist> Artists { get { return db.Artists; } }

        public void Delete(Artist artist)
        {
            db.Artists.Remove(artist);
            db.SaveChanges();
        }

        public Artist Save(Artist artist)
        {
            if (artist.ArtistId != null)
            {
                db.Entry(artist).State = EntityState.Modified;
            }
            else
            {
                db.Artists.Add(artist);
            }

            db.SaveChanges();
            return artist;
        }
    }
}