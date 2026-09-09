using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class
{

    public class Satis
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class CustomerAddress
        {
            [DisplayName("İl")]
            public string City { get; set; }
            [DisplayName("İşçe")]
            public string County { get; set; }
            [DisplayName("Mahalle")]
            public string Region { get; set; }
            [DisplayName("Tam Adres")]
            public string Address { get; set; }
            [DisplayName("Adres Telefonu")]
            public string Phone { get; set; }
        }
        public class Customer
        {
            [DisplayName("Müşteri Rapid Kodu")]
            public string CustomerCode { get; set; }
            [DisplayName("TC Nosu")]
            public string CustomerIdentityNumber { get; set; }
            [DisplayName("Müşteri Adı")]
            public string CustomerFirstName { get; set; }
            [DisplayName("Müşteri Soyadı")]
            public string CustomerLastName { get; set; }
            [DisplayName("Müşteri Doğım Tarihi")]
            public DateTime CustomerBirthday { get; set; }
            [DisplayName("Müşteri Telefonu")]
            public string CustomerPhone { get; set; }
            [DisplayName("Müşteri E-Mail")]
            public string Email { get; set; }
            [DisplayName("Şirket")]
            public bool CustomerCorparate { get; set; }
            [DisplayName("Müşteri Vergi Dairesi")]
            public string CustomerWatP { get; set; }
            [DisplayName("Müşteri Vergi Nosu")]
            public string CustomerWatNo { get; set; }
            [DisplayName("Müşteri Adresi")]
            public CustomerAddress CustomerAddress { get; set; }
        }
        public class DeliverAddress
        {
            [DisplayName("İl")]
            public string City { get; set; }
            [DisplayName("İşçe")]
            public string County { get; set; }
            [DisplayName("Mahalle")]
            public string Region { get; set; }
            [DisplayName("Tam Adres")]
            public string Address { get; set; }
            [DisplayName("Adres Telefonu")]
            public string Phone { get; set; }
        }
        public class Instalment
        {
            [DisplayName("Taksit Tarihi")]
            public DateTime instalmentFixDate { get; set; }
            [DisplayName("Taksit Tutarı")]
            public decimal instalmentAmount { get; set; }
        }
        public class Payment
        {
            [DisplayName("Ödeme Şekli")]
            public string PaymentTypeCode { get; set; }
            [DisplayName("Ödeme Tutarı")]
            public decimal PaymentAmount { get; set; }
        }
        public class Product
        {
            [DisplayName("Ürün Kodu")]
            public string ItemCode { get; set; }
            [DisplayName("Adet")]
            public int Quantity { get; set; }
            [DisplayName("Net Fiyat")]
            public decimal PriceWithTax { get; set; }
            [DisplayName("Iskonto")]
            public decimal ListDisAmount { get; set; }
            [DisplayName("Taksit Sayısı")]
            public int InstalmentCount { get; set; }
            [DisplayName("Fiyat Tipi")]
            public string PriceVal { get; set; }
            [DisplayName("Satıcı Kodu")]
            public string SalesmenVal { get; set; }
        }
        public class CreaditSales
        {
            [DisplayName("Müşteri Kodu")]
            public string CustomerCode { get; set; }
            [DisplayName("İşlendiği Mağaza Kodu")]
            public string StoreCode { get; set; }
            [DisplayName("Satış Mağaza Kodu")]
            public string StoreWareHouseCode { get; set; }
            [DisplayName("Satış Tarihi")]
            public DateTime OrderDate { get; set; }
            [DisplayName("Satış Nosu")]
            public string OrderNumber { get; set; }
            [DisplayName("Kargo Kodu")]
            public string CargoNumber { get; set; }
            [DisplayName("Kargo Firma Adı")]
            public string CargoVal { get; set; }
            [DisplayName("Ürünler")]
            public List<Product> Products { get; set; }
            [DisplayName("Ödemeler")]
            public List<Payment> Payments { get; set; }
            [DisplayName("Taksitler")]
            public List<Instalment> Instalments { get; set; }
            [DisplayName("Satış Iskontosu")]
            public int DiscountAmount { get; set; }
            [DisplayName("Ödeme Para Birimi")]
            public string CurrencyCode { get; set; }
            [DisplayName("Teslimat Adresi")]
            public DeliverAddress DeliverAddress { get; set; }
        }
        public class PaymentSales
        {

            [DisplayName("Müşteri Kodu")]
            public string CustomerCode { get; set; }
            [DisplayName("İşlendiği Mağaza Kodu")]
            public string StoreCode { get; set; }
            [DisplayName("Satış Mağaza Kodu")]
            public string StoreWareHouseCode { get; set; }
            [DisplayName("Satış Tarihi")]
            public DateTime OrderDate { get; set; }
            [DisplayName("Satış Nosu")]
            public string OrderNumber { get; set; }
            [DisplayName("")]
            public string CargoNumber { get; set; }
            [DisplayName("")]
            public string CargoVal { get; set; }
            [DisplayName("Ürünler")]
            public List<Product> Products { get; set; }
            [DisplayName("Ödemeler")]
            public List<Payment> Payments { get; set; }
            [DisplayName("Satış Iskontosu")]
            public int DiscountAmount { get; set; }
            [DisplayName("Ödeme Para Birimi")]
            public string CurrencyCode { get; set; }
            [DisplayName("Teslimat Adresi")]
            public DeliverAddress DeliverAddress { get; set; }
        }
        public class SALESINVESTIGATION
        {
            public long SAINGTSALID { get; set; }

            public string SAINGTPOSTSOCODE { get; set; }

            public string SAINGTSOCODE { get; set; } = EntegreFDLL.Class.Entegref.GetLogins.userID;

            public DateTime SAINGTDATETIME { get; set; } = DateTime.Now;

            public bool? SAINGTISDONE { get; set; } = false;

            public bool? SAINGTSALSTS { get; set; }

            public string SAINGTSALNOTES { get; set; }

            public long SAINGTCURORWRTRID { get; set; }

            public short? SAINGTWORKSTS { get; set; }

        }
        public class CustomerInsert
        {
            public string CustomerCode { get; set; }
            public long CURID { get; set; }
        }
        public class MainRood
        {
            [DisplayName("Müşteri Kodu")]
            public string CustomerCode { get; set; }
            [DisplayName("Müşteri Bilgileri")]
            public List<Customer> Customers { get; set; }
            [DisplayName("Taksitli Satış Bilgileri")]
            public List<CreaditSales> CreaditSales { get; set; }
            [DisplayName("Peşin Satış Bilgileri")]
            public List<PaymentSales> PaymentSales { get; set; }
        }
        public class SALES
        {
            public long SALID { get; set; }

            public string SALCOMPANY { get; set; }

            public string SALDIVISON { get; set; }

            public long SALCURID { get; set; }

            public DateTime SALDATE { get; set; }

            public decimal SALAMOUNT { get; set; }

            public string SALCHVAL { get; set; }

            public string SALSALEKIND { get; set; }

            public string SALSHIPKIND { get; set; }

            public string SALINSDIV { get; set; }

            public byte? SALINSSTS { get; set; }

            public short? SALINSVAL { get; set; }

            public bool? SALINS { get; set; }

            public byte? SALCREDITCHECK { get; set; }

            public bool? SALISONDELIVERY { get; set; }

            public int? SALCANID { get; set; }

            public long? SALCANSALID { get; set; }

            public long? SALCHANGEID { get; set; }

            public string SALSTS { get; set; }

            public string SALUSEFIELD1 { get; set; }

            public string SALUSEFIELD2 { get; set; }

            public string SALUSEFIELD3 { get; set; }

            public string SALSOCODE { get; set; }

            public DateTime SALDATETIME { get; set; }

            public short? SALCONTRACTDIV { get; set; }

            public bool SALCONTRACTKIND { get; set; }

            public string SALCUROTHERVAL { get; set; }

            public string SALCONSULTATOR { get; set; }

            public string SALPREFORMNO { get; set; }

            public bool? SALINSDEMAND { get; set; }

        }
        public class Satislem
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public List<string> results { get; set; }
        }
        public class Hata400
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
        }
        public class SatisHata
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public List<Validation> validations { get; set; }
        }
        public class Validation
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }
    }
}
