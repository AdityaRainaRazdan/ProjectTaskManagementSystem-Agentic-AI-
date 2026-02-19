using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.DC;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Xpo;
using PersistentAliasAttribute = DevExpress.ExpressApp.DC.PersistentAliasAttribute;

[DefaultClassOptions]
public class InvoiceLineItem : BaseObject
{

    public virtual string Description { get; set; }

    public virtual decimal Quantity { get; set; }

    public virtual decimal UnitPrice { get; set; }

    [Association("Invoice-LineItems")]
    public virtual Invoice Invoice { get; set; }

    [NotMapped]
    [PersistentAlias("IsNull(Quantity, 0) * IsNull(UnitPrice, 0)")]
    public decimal LineTotal
        => (decimal)(EvaluateAlias(nameof(LineTotal)) ?? 0m);
}
