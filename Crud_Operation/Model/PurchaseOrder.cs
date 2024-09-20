namespace Crud_Operation.Model
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string PoId { get; set; }
        public string Reference { get; set; }
        public DateTime? PoDate { get; set; }
        public string EventName { get; set; }
        public DateTime? EventDate { get; set; }
        public string EventTime { get; set; }
        public string Venue { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string Seats { get; set; }
        public int Qty { get; set; }
        public decimal Face { get; set; }
        public decimal Cost { get; set; }
        public bool EDelivery { get; set; }
        public string InternalNotes { get; set; }
        public string ExternalNotes { get; set; }
        public string ProcurementNotes { get; set; }
        public bool InHand { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? AvailableDate { get; set; }
        public string TrackingNumber { get; set; }
        public string PurchaseStatus { get; set; }
    }
}
