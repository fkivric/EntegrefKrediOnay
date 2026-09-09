using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class
{

    public class Musteri
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class CustomerAddress
        {
            public string City { get; set; }
            public string County { get; set; }
            public string Region { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }

        public class Customer
        {
            public string CustomerCode { get; set; }
            public string CustomerIdentityNumber { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public DateTime CustomerBirthday { get; set; }
            public string CustomerPhone { get; set; }
            public string Email { get; set; }
            public bool CustomerCorparate { get; set; }
            public string CustomerWatP { get; set; }
            public string CustomerWatNo { get; set; }
            public CustomerAddress CustomerAddress { get; set; }
        }

        public class CreateCustomer
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public List<string> results { get; set; }
        }
        public class CustomerCode
        {
            public string customerCode { get; set; }
        }
        public class MusteriVar
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public List<string> results { get; set; }
        }
        public class MusteriYok
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public List<Validation> validations { get; set; }
        }
        public class MusteriHata
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
        }
        public class Validation
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }
        public class MusteriUpdate
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string results { get; set; }
            public string message { get; set; }
            public string internalMessage { get; set; }
            public List<Validation> validations { get; set; }
        }
    }
}
