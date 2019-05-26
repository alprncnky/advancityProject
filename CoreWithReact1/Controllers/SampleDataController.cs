using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreWithReact1.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IProvider Provider;

        public SampleDataController(IProvider Provider)
        {
            this.Provider = Provider;
        }

        // Get: api/SampleData/Get
        [HttpGet("[action]")]
        public IEnumerable<Sinif> Get()
        {
            return Provider.Get();
        }

        // Get: api/SampleData/GetOgrencis
        [HttpGet("[action]")]
        public IEnumerable<Ogrenci> GetOgrencis()
        {
            return Provider.GetOgrencis();
        }

        // Get: api/SampleData/GetDers
        [HttpGet("[action]")]
        public IEnumerable<Ders> GetDers()
        {
            return Provider.GetDers();
        }

        // OGRENCI HTTP POST SAYFA SAYISI
        [HttpPost]
        [Route("postOgrenci")]
        public Tuple<int, IEnumerable<Ogrenci>, IEnumerable<Sinif>, IEnumerable<Ders>> PostOgrenci([FromBody]PageNoModel pageNoModel)
        {
            return Provider.GetPageOgrenci(pageNoModel.PageNo, pageNoModel.Search);
        }

        // SINIF HTTP POST SAYFA SAYISI
        [HttpPost]
        [Route("postSinif")]
        public Tuple<int, IEnumerable<Sinif>, IEnumerable<Ders>> PostSinif([FromBody]PageNoModel pageNoModel)
        {
            return Provider.GetPageSinif(pageNoModel.PageNo, pageNoModel.Search);
        }

        // DERS HTTP POST SAYFA SAYISI
        [HttpPost]
        [Route("postPage")]
        public Tuple<string, string, IEnumerable<Ders>> Postfunction([FromBody]PageNoModel pageNoModel)
        {
            return Provider.GetPageDers(pageNoModel.PageNo, pageNoModel.Search);
        }
    }
}
