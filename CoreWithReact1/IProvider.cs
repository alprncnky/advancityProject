using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithReact1
{
    public interface IProvider
    {
        IEnumerable<Sinif> Get();

        IEnumerable<Ogrenci> GetOgrencis();

        IEnumerable<Ders> GetDers();

        Tuple<int, IEnumerable<Ogrenci>, IEnumerable<Sinif>, IEnumerable<Ders>> GetPageOgrenci(string pageNo, string search);

        Tuple<int, IEnumerable<Sinif>, IEnumerable<Ders>> GetPageSinif(string pageNo, string search);

        Tuple<string, string, IEnumerable<Ders>> GetPageDers(string pageNo, string search);

        Tuple<IEnumerable<Ogrenci>, IEnumerable<Ders>> GetOgrDers();


    }
}
