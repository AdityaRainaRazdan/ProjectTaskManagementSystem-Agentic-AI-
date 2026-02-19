using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;

namespace projectTaskManage.Module.BusinessObjects;

[DefaultClassOptions]
[RuleCriteria(
        "EndDateAfterStartDate",
        DefaultContexts.Save,
        "EndDate >= StartDate",
        CustomMessageTemplate = "End Date cannot be earlier than Start Date"
    )]
public class Project : BaseObject
{
    public Project() { Tasks = new ObservableCollection<ProjectTask>(); }
     
    [RuleRequiredField]
    public virtual string Name { get; set; }

    public virtual DateTime StartDate { get; set; }

    public virtual DateTime? EndDate { get; set; }

    [Required]
    public virtual Organization Organization { get; set; }

    public virtual Employee ProjectManager { get; set; }

    public virtual ObservableCollection<ProjectTask> Tasks { get; set; }

    [PersistentAlias("Tasks.Count")]
    [VisibleInListView(true)]
    [VisibleInDetailView(true)]
    [Browsable(true)]
    public int TotalTasks => Tasks?.Count ?? 0;

}
