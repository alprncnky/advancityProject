using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWithReact1
{
    public class Provider : IProvider
    {
        private readonly string connectionString;

        public Provider(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public IEnumerable<Sinif> Get()
        {
            IEnumerable<Sinif> sinif = null;

            using (var connection = new SqlConnection(connectionString))
            {
                sinif = connection.Query<Sinif>(@"SELECT ID_Sinif, sinif_adi as S_adi FROM Sinif");
            }

            return sinif;
        }

        public IEnumerable<Ogrenci> GetOgrencis()
        {
            IEnumerable<Ogrenci> ogrenci = null;

            using (var connection = new SqlConnection(connectionString))
            {
                ogrenci = connection.Query<Ogrenci>(@"SELECT ad as Ad, soyad as Soyad FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID");
            }

            return ogrenci;
        }

        public IEnumerable<Ders> GetDers()
        {
            IEnumerable<Ders> ders = null;

            using (var connection = new SqlConnection(connectionString))
            {
                ders = connection.Query<Ders>(@"SELECT ders_adi as Ders_Adi FROM Ders");
            }

            return ders;
        }


        // Ogrenci sayfasinda tablo listeleme - aranan kelimeye gore ve sayfa sayisina bolme islemi yap controllera dondur.
        public Tuple<int, IEnumerable<Ogrenci>, IEnumerable<Sinif>, IEnumerable<Ders>> GetPageOgrenci(string pageNo, string search)
        {
            Tuple<int, IEnumerable<Ogrenci>, IEnumerable<Sinif>, IEnumerable<Ders>> tuple;
            IEnumerable<Ogrenci> ogrenci = null;
            IEnumerable<Sinif> sinif = null;
            IEnumerable<Ders> ders = null;
            int pageNumber = Convert.ToInt32(pageNo);
            int toplamKayit;

            using (var connection = new SqlConnection(connectionString))
            {
                if (search.Length > 0 && search != null)    // search bos ise sql komutuna yazma
                {
                    sinif = connection.Query<Sinif>(@"SELECT Ogrenci.ad, Ogrenci.soyad, Ogrenci.numara FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif WHERE (Ders.ders_adi = @srch) OR (Sinif.sinif_adi = @srch) OR (Ogrenci.ad = @srch)", new { @srch = search });
                    toplamKayit = sinif.Count();
                    // ARANAN DEGERE GORE SAYFALAMA YAP
                    if (toplamKayit / 10 > pageNumber || toplamKayit > 0)    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR. // toplamkayit>0 5 kayit varken 0. sayfada 
                    {
                        var result = connection.QueryMultiple(@"SELECT Ogrenci.ad, Ogrenci.soyad, Ogrenci.numara FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif WHERE (Ders.ders_adi = @srch) OR (Sinif.sinif_adi = @srch) OR (Ogrenci.ad = @srch) order by ID_ogrenci offset @pageNo rows fetch next 10 rows only;
                                                                SELECT Sinif.sinif_adi as S_adi FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif WHERE (Ders.ders_adi = @srch) OR (Sinif.sinif_adi = @srch) OR (Ogrenci.ad = @srch) order by ID_ogrenci offset @pageNo rows fetch next 10 rows only
                                                                SELECT Ders.ders_adi FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif WHERE (Ders.ders_adi = @srch) OR (Sinif.sinif_adi = @srch) OR (Ogrenci.ad = @srch) order by ID_ogrenci offset @pageNo rows fetch next 10 rows only", new { @srch = search, @pageNo = pageNumber * 10 });
                        ogrenci = result.Read<Ogrenci>();
                        sinif = result.Read<Sinif>();
                        ders = result.Read<Ders>();
                        tuple = Tuple.Create(toplamKayit, ogrenci, sinif, ders);
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create(toplamKayit, ogrenci, sinif, ders);
                    }
                }
                else
                {
                    sinif = connection.Query<Sinif>(@"SELECT Ogrenci.ad, Ogrenci.soyad, Ogrenci.numara FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif order by ID_ogrenci");
                    toplamKayit = sinif.Count();
                    // ARAMA DEGERI YOK SADECE SAYFALAMA YAP
                    if (toplamKayit / 10 >= pageNumber || toplamKayit > 0)    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR.
                    {
                        var result = connection.QueryMultiple(@"SELECT Ogrenci.ad, Ogrenci.soyad, Ogrenci.numara FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif order by ID_ogrenci offset @pageNo rows fetch next 10 rows only;
                                                                SELECT Sinif.sinif_adi as S_adi FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif order by ID_ogrenci offset @pageNo rows fetch next 10 rows only
                                                                SELECT Ders.ders_adi FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif order by ID_ogrenci offset @pageNo rows fetch next 10 rows only", new { @pageNo = pageNumber * 10 });
                        ogrenci = result.Read<Ogrenci>();
                        sinif = result.Read<Sinif>();
                        ders = result.Read<Ders>();
                        tuple = Tuple.Create(toplamKayit, ogrenci, sinif, ders);
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create(toplamKayit, ogrenci, sinif, ders);
                    }
                }
            }
            return tuple;
        }


        // Sinif sayfasinda tablo listeleme - aranan kelimeye gore ve sayfa sayisina bolme islemi yap controllera dondur.
        public Tuple<int,IEnumerable<Sinif>,IEnumerable<Ders>> GetPageSinif(string pageNo, string search)
        {
            Tuple<int, IEnumerable<Sinif>, IEnumerable<Ders>> tuple;
            IEnumerable<Sinif> sinif = null;
            IEnumerable<Ders> ders = null;
            int pageNumber = Convert.ToInt32(pageNo);
            int toplamKayit;


            using (var connection = new SqlConnection(connectionString))
            {
                if (search.Length > 0 && search != null)    // search bos ise sql komutuna yazma
                {
                    sinif = connection.Query<Sinif>(@"SELECT Sinif.ID_Sinif FROM Sinif INNER JOIN Ders ON Sinif.ID_Sinif = Ders.FK_sinif_ID WHERE (Ders.ders_adi = @srch or Sinif.sinif_adi = @srch)", new { @srch = search });
                    toplamKayit = sinif.Count();
                    // ARANAN DEGERE GORE SAYFALAMA YAP
                    if (toplamKayit / 10 > pageNumber || toplamKayit > 0)    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR. // toplamkayit>0 5 kayit varken 0. sayfada 
                    {
                        var result = connection.QueryMultiple(@"SELECT Sinif.ID_Sinif, Sinif.sinif_adi as S_Adi FROM Sinif INNER JOIN Ders ON Sinif.ID_Sinif = Ders.FK_sinif_ID WHERE (Ders.ders_adi = @srch) or (Sinif.sinif_adi = @srch) order by ID_Sinif offset @pageNo rows fetch next 10 rows only;
                                                                SELECT Ders.ders_adi FROM Ders INNER JOIN Sinif ON Ders.FK_sinif_ID = Sinif.ID_Sinif WHERE (Ders.ders_adi = @srch) OR (Sinif.sinif_adi = @srch) order by ID_Ders offset @pageNo rows fetch next 10 rows only", new {@srch = search, @pageNo = pageNumber * 10 });
                        sinif = result.Read<Sinif>();
                        ders = result.Read<Ders>();
                        tuple = Tuple.Create(toplamKayit, sinif, ders);
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create(toplamKayit, sinif, ders);
                    }
                }
                else
                {
                    sinif = connection.Query<Sinif>(@"SELECT Sinif.ID_Sinif, Sinif.sinif_adi FROM Sinif INNER JOIN Ders ON Sinif.ID_Sinif = Ders.FK_sinif_ID");
                    toplamKayit = sinif.Count();
                    // ARAMA DEGERI YOK SADECE SAYFALAMA YAP
                    if (toplamKayit / 10 >= pageNumber || toplamKayit > 0)    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR.
                    {
                        var result = connection.QueryMultiple(@"SELECT Sinif.ID_Sinif, Sinif.sinif_adi as S_adi FROM Sinif INNER JOIN Ders ON Sinif.ID_Sinif = Ders.FK_sinif_ID order by ID_Sinif offset @pageNo rows fetch next 10 rows only; SELECT ders_adi FROM Ders order by ID_Ders offset @pageNo rows fetch next 10 rows only", new { @pageNo = pageNumber * 10 });
                        sinif = result.Read<Sinif>();
                        ders = result.Read<Ders>();
                        tuple = Tuple.Create(toplamKayit, sinif, ders);
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create(toplamKayit, sinif, ders);
                    }
                }
            }
            return tuple;
        }


        // Ders sayfasinda tablo listeleme - aranan kelimeye gore ve sayfa sayisina bolme islemi yap controllera dondur.
        public Tuple<string, string, IEnumerable<Ders>> GetPageDers(string pageNo, string search)
        {
            Tuple<string, string, IEnumerable<Ders>> tuple;
            IEnumerable<Ders> ders = null;
            int pageNumber = Convert.ToInt32(pageNo);
            int toplamKayit;

            using (var connection = new SqlConnection(connectionString))
            {
                if (search.Length > 0 && search != null)    // search bos ise sql komutuna yazma
                {
                    ders = connection.Query<Ders>(@"select * from Ders WHERE ders_adi = @srch order by ID_Ders", new { @srch = search });
                    toplamKayit = ders.Count();
                    // ARANAN DEGERE GORE SAYFALAMA YAP
                    if (toplamKayit / 10 > pageNumber || toplamKayit>0 )    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR. // toplamkayit>0 5 kayit varken 0. sayfada 
                    {
                        ders = connection.Query<Ders>(@"select * from Ders WHERE ders_adi = @srch order by ID_Ders offset @pageNo rows fetch next 10 rows only", new { @srch = search, @pageNo = pageNumber * 10 });
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create("0", pageNo.ToString(), ders);
                    }
                }
                else
                {
                    ders = connection.Query<Ders>(@"select * from Ders order by ID_Ders");
                    toplamKayit = ders.Count();
                    // ARAMA DEGERI YOK SADECE SAYFALAMA YAP
                    if (toplamKayit / 10 >= pageNumber || toplamKayit > 0)    // TOPLAM KAYIT SAYISINI ASMICAK SEKILDE YENI SAYFA AL  - ORN: 30 KAYIT VARSA 3 SAYFA OLUR.
                    {
                        ders = connection.Query<Ders>(@"select * from Ders order by ID_Ders offset @pageNo rows fetch next 10 rows only", new { @pageNo = pageNumber * 10 });
                    }
                    else
                    {
                        // daha fazla kayıt yok daha fazla sayfa yok
                        return tuple = Tuple.Create("0", pageNo.ToString(), ders);
                    }
                }                
            }
            string count = toplamKayit.ToString();                      // toplam kayit sayisini dondur
            tuple = Tuple.Create(count, pageNo.ToString(), ders);       // verileri     toplam_kayit_saiyisi | sayfa_numarasi | tablo_verileri seklinde dondur.
            return tuple;
        }


        public Tuple<IEnumerable<Ogrenci>, IEnumerable<Ders>> GetOgrDers()
        {
            Tuple<IEnumerable<Ogrenci>, IEnumerable<Ders>> tuple;
            IEnumerable<Ogrenci> ogrenci = null;
            IEnumerable<Ders> ders = null;

            using (var connection = new SqlConnection(connectionString))
            {
                var result = connection.QueryMultiple(@"SELECT ad as Ad, soyad as Soyad FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID;  SELECT ders_adi as Ders_Adi FROM Ogrenci INNER JOIN Ders ON Ogrenci.ID_ogrenci = Ders.FK_ogrenci_ID");
                ogrenci = result.Read<Ogrenci>();
                ders = result.Read<Ders>();
                tuple = Tuple.Create(ogrenci, ders);
            }

            return tuple;
        }
    }
}
