using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using projectTaskManage.Module.BusinessObjects;

namespace projectTaskManage.Module.Controllers.InvoiceFlow
{
    public class InvoiceWorkflowController
        : ObjectViewController<DetailView, Invoice>
    {
        private SimpleAction submitAction;
        private SimpleAction approveAction;
        private SimpleAction rejectAction;

        protected override void OnActivated()
        {
            base.OnActivated();

            CreateActions();
            UpdateActionState();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateActionState();
        }

        protected override void OnDeactivated()
        {
            submitAction?.Dispose();
            approveAction?.Dispose();
            rejectAction?.Dispose();

            base.OnDeactivated();
        }

        // ==============================
        // ACTION DEFINITIONS
        // ==============================
        private void CreateActions()
        {
            // SUBMIT (CLERK)
            submitAction = new SimpleAction(this, "SubmitInvoice", PredefinedCategory.Edit)
            {
                Caption = "Submit",
                ImageName = "State_Task_Completed"
            };
            submitAction.Execute += SubmitAction_Execute;

            // APPROVE (MANAGER)
            approveAction = new SimpleAction(this, "ApproveInvoice", PredefinedCategory.Edit)
            {
                Caption = "Approve",
                ImageName = "State_Validation_Valid"
            };
            approveAction.Execute += ApproveAction_Execute;

            // REJECT (MANAGER)
            rejectAction = new SimpleAction(this, "RejectInvoice", PredefinedCategory.Edit)
            {
                Caption = "Reject",
                ImageName = "State_Validation_Invalid"
            };
            rejectAction.Execute += RejectAction_Execute;
        }

        // ==============================
        // ACTION VISIBILITY / ENABLE
        // ==============================
        private void UpdateActionState()
        {
            if (ViewCurrentObject == null)
                return;

            var user = SecuritySystem.CurrentUser as PermissionPolicyUser;

            bool isClerk = user?.IsUserInRole("CLERK") == true;
            bool isManager = user?.IsUserInRole("MANAGER") == true;

            // SUBMIT → CLERK + Draft
            submitAction.Active["Role"] =
                isClerk && ViewCurrentObject.Status == InvoiceStatus.Draft;

            // APPROVE → MANAGER + Submitted
            approveAction.Active["Role"] =
                isManager && ViewCurrentObject.Status == InvoiceStatus.Submitted;

            // REJECT → MANAGER + Submitted
            rejectAction.Active["Role"] =
                isManager && ViewCurrentObject.Status == InvoiceStatus.Submitted;
        }

        // ==============================
        // ACTION LOGIC
        // ==============================
        private void SubmitAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject.Status = InvoiceStatus.Submitted;
            ObjectSpace.CommitChanges();
            UpdateActionState();
        }

        private void ApproveAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject.Status = InvoiceStatus.Approved;
            ObjectSpace.CommitChanges();
            UpdateActionState();
        }

        private void RejectAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject.Status = InvoiceStatus.Rejected;
            ObjectSpace.CommitChanges();
            UpdateActionState();
        }
    }
}
