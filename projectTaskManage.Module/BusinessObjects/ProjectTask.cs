using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using System.ComponentModel.DataAnnotations;

namespace projectTaskManage.Module.BusinessObjects;

[RuleCriteria(
    "ActiveEmployeeOnly",
    DefaultContexts.Save,
    "AssignedTo.IsActive = true",
    CustomMessageTemplate = "Task can be assigned only to an active employee"
)]
[RuleCriteria(
    "AssignOnlyManagersOnCreate",
    DefaultContexts.Save,
    "AssignedTo != null AND AssignedTo.Role.Name = 'Manager'",
    "Only Managers can be assigned when creating a task"
)]




[DefaultClassOptions]
public class ProjectTask : BaseObject,IXafEntityObject
{
    [RuleRequiredField]
    [RuleRequiredField]
    public virtual string Title { get; set; }

    public virtual TaskStatus Status { get; set; }

    public virtual DateTime? DueDate { get; set; }

    //many tasks-> one project
    [RuleRequiredField]
    public virtual Project Project { get; set; }

    //many tasks-> one employee
    [DataSourceCriteria("Roles[Name = 'Manager']")]
    public virtual Employee AssignedTo { get; set; }

    //  Business rule (data-level)
    void IXafEntityObject.OnSaving()
    {
        if (AssignedTo != null && !AssignedTo.IsActive)
        {
            throw new UserFriendlyException(
                "You cannot assign a task to an inactive employee."
            );
        }
    }

    void IXafEntityObject.OnCreated() { }
    void IXafEntityObject.OnLoaded() { }

}
