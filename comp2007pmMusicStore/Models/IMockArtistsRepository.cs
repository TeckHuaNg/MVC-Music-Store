using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comp2007pmMusicStore.Models
{
    public interface IMockArtistsRepository
    {
        IQueryable<Artist> Artists { get; }
        Artist Save(Artist artist);
        void Delete(Artist artist);

    }
}
