namespace IptvManagement.Domain.Enums;
public enum SubscriptionStatus { Draft=0, Active=1, Paused=2, Blocked=3, Cancelled=4, Expired=5 }
public enum PaymentStatus { Open=0, Pending=1, Paid=2, Failed=3, Refunded=4, Cancelled=5 }
public enum InvoiceStatus { Draft=0, Issued=1, Paid=2, Overdue=3, Cancelled=4 }
public enum PaymentMethod { BankTransfer=0, Cash=1, Card=2, Sepa=3, Online=4 }
public enum DeviceType { Unknown=0, SmartTv=1, Mobile=2, Tablet=3, Browser=4, SetTopBox=5 }
public enum UserRole { Administrator=0, Mitarbeiter=1, Reseller=2, NurLesen=3 }
