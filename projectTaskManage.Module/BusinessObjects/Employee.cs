using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DocumentFormat.OpenXml.Vml;

namespace  projectTaskManage.Module.BusinessObjects;

[DefaultClassOptions]
public class Employee : BaseObject
{
    public Employee()
    {
        Files = new ObservableCollection<EmployeeFile>();
        Tasks = new ObservableCollection<ProjectTask>();
        IsActive = true;
    }

    [Required]
    public virtual string FullName { get; set; }

    [RuleRegularExpression(
    "ValidEmail",
    DefaultContexts.Save,
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    CustomMessageTemplate = "Invalid email address"
)]

    [EmailAddress]
    public virtual string Email { get; set; }

    public virtual bool IsActive { get; set; } = true;

    [RuleRequiredField]
    public virtual Organization Organization { get; set; }

    // Link to XAF User
    //public virtual PermissionPolicyUser User { get; set; }
    [ImageEditor(
        ListViewImageEditorMode = ImageEditorMode.PictureEdit,
        DetailViewImageEditorMode = ImageEditorMode.PictureEdit
        )]
    public virtual byte[] Photo { get; set; }

    [DevExpress.Xpo.Association("Employee-EmployeeFiles")]
    public virtual ObservableCollection<EmployeeFile> Files { get; set; }

    public virtual EmployeeDesignation Designation { get; set; }
    public virtual ObservableCollection<ProjectTask> Tasks { get; set; }
    public virtual ApplicationUser User { get; set; }

}
