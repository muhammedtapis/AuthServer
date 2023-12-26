using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    //bu sınıf appsettingste vereceğimiz client bilgilerini karşılamak için oluşturuldu.
    //appsettingsteki isimler ile burdaki property isimleri aynı olmayınca MAPLEMİYOR!!!
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        // www.miniapi1.com www.miniapi2.com
        public List<string> Audiences { get; set; } //apilerden hangilerine erişecek onu belirlicez, apileri listeleyeceğimiz alan.
    }
}