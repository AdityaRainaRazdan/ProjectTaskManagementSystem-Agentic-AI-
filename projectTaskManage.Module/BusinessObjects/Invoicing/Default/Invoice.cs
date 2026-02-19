using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Xpo;
using Microsoft.EntityFrameworkCore;
using projectTaskManage.Module.BusinessObjects;

[DefaultClassOptions]
[Browsable(true)]
public class Invoice : BaseObject
{
    //public Invoice() { }
    //[ReadOnly(true)]
    //[Size(20)]
    public virtual string InvoiceNumber { get; set; }

    public virtual DateTime InvoiceDate { get; set; } = DateTime.Now;

    [ReadOnly(true)]
    public virtual InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    [Association("Invoice-LineItems")]
    public virtual ICollection<InvoiceLineItem> LineItems { get; set; }
    = new ObservableCollection<InvoiceLineItem>();

    [Association("Invoice-ApprovalHistory")]
    public virtual ICollection<InvoiceApprovalHistory> ApprovalHistory { get; set; }
        = new ObservableCollection<InvoiceApprovalHistory>();


    [NotMapped]
    [Precision(18, 2)]
    public decimal SubTotal
    {
        get
        {
            if (LineItems == null || LineItems.Count == 0)
                return 0m;

            return LineItems.Sum(x =>
                (x.Quantity) * (x.UnitPrice));
        }
    }


    [NotMapped]
    [Precision(18, 2)]
    public decimal Tax => SubTotal * 0.18m;

    [NotMapped]
    [Precision(18, 2)]
    public decimal Total => SubTotal + Tax;


    //[Browsable(false)]
    public virtual ApplicationUser CreatedBy { get; set; }

    //public override void OnSaving()
    //{
    //    base.OnSaving();

    //    if (ObjectSpace.IsNewObject(this) && string.IsNullOrEmpty(InvoiceNumber))
    //    {
    //        var year = DateTime.Now.Year;

    //        var maxSeq = ObjectSpace
    //            .GetObjects<Invoice>()
    //            .Where(i => i.InvoiceNumber.StartsWith($"INV-{year}"))
    //            .Max(i => (int?)i.SequenceNumber) ?? 0;

    //        SequenceNumber = maxSeq + 1;
    //        InvoiceNumber = $"INV-{year}-{SequenceNumber:D4}";
    //    }
    //}
}
