using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Xpo;

namespace projectTaskManage.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class EmployeeFile  : FileAttachment
    {
        [Association("Employee-EmployeeFiles")]
        public virtual Employee Employee { get; set; }
    }
}
