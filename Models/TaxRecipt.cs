namespace summa_task.Models
{
    public class TaxReceipt
    {
        public string? CompanyName { get; set; }
        public string? Subtitle { get; set; }
        public string? VatId { get; set; }
        public string? BusinessUnionNumber { get; set; }
        public Contact? Contact { get; set; }
        public string? Date { get; set; }
        public Invoice? Invoice { get; set; }
    }

    public class Contact
    {
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }

    public class Invoice
    {
        public string? Number { get; set; }
        public string? CopyNote { get; set; }
        public Recipient? Recipient { get; set; }
        public Detail[]? Details { get; set; }
        public string? DocumentStatus { get; set; }
        public string? Page { get; set; }
        public Item[]? Items { get; set; }
        public string? Discount { get; set; }
        public string? FinalTotal { get; set; }
        public string? DigitalSignatureNote { get; set; }
    }

    public class Recipient
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? VatOrId { get; set; }
        public string? Email { get; set; }
    }

    public class Detail
    {
        public string? Subject { get; set; }
        public string? CatalogNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? PaymentDue { get; set; }
        public string? Remarks { get; set; }
        public string? Bank { get; set; }
        public string? SecurePaymentLink { get; set; }
    }

    public class Item
    {
        public int Quantity { get; set; }
        public string? Price { get; set; }
        public string? Total { get; set; }
    }
}
