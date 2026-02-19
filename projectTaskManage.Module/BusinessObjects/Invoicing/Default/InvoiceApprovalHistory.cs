using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using projectTaskManage.Module.BusinessObjects;

[DefaultClassOptions]

public class InvoiceApprovalHistory : BaseObject
{
    public InvoiceApprovalHistory() { }

    public virtual DateTime ActionDate { get; set; } = DateTime.Now;

    public virtual string Action { get; set; }

    public virtual string Comment { get; set; }
    public virtual Employee PerformedBy { get; set; }

    public virtual Invoice Invoice { get; set; }
}
