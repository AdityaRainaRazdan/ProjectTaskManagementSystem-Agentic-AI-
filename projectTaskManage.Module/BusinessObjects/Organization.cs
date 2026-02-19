using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace projectTaskManage.Module.BusinessObjects;

[DefaultClassOptions]
public class Organization : BaseObject
{
    public Organization() { Projects = new ObservableCollection<Project>(); }

    [RuleRequiredField]
    public virtual string Name { get; set; }

    public virtual string Code { get; set; }

    [DevExpress.Xpo.Association("Organization-Employees")]
    public virtual IList<Employee>Employees { get; set; }=new ObservableCollection<Employee>();

    //one organization -> Many Projects
    [DevExpress.Xpo.Association("Organization-Projects")]
    public virtual ObservableCollection<Project> Projects { get; set; }
}
