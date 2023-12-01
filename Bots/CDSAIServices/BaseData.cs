using System;
using System.Collections.Generic;
using System.Linq;
using CDSAIServices.Domain;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Options;

namespace CDSAIServices
{
    public static class BaseData
    {
        public const string UnityTenantName = "Unity";
        public const string CantinaTenantName = "Cantina";

        public static List<string> Tenants => new()
        {
            CantinaTenantName,
            UnityTenantName
        };

        public const string CreateVMAction = "Create VM";
        public const string AddAppToVMAction = "Add App to VM";
        public const string CheckVMStatusAction = "Check VM Status";
        public const string DeleteVMAction = "Delete VM";
        public const string AddIPToStorageAccount = "Get SAS Token Uri for Container";
        public const string TestOpenAI = "Test Open AI";
        public const string BackupAndRestoreDatabase = "Backup and Restore Database";
        //DC Change
        //Add const string for Add DC

        public static List<string> Actions => new()
        {
            CreateVMAction,
            AddAppToVMAction,
            CheckVMStatusAction,
            DeleteVMAction,
            BackupAndRestoreDatabase
        };

        public static List<string> AdminActions
        {
            get
            {
                var actions = Actions;
                //actions.Add(BaseData.DeleteVMAction);
                actions.Add(AddIPToStorageAccount);
                actions.Add(TestOpenAI);
                //DC Change
                //Add DC Action to actions
                return actions;
            }
        }

        public static IList<Choice> EnumChoices<T>()
        {
            var enumChoices = Enum.GetNames(typeof(T)).ToList();
            enumChoices.Remove("None");
            return ChoiceFactory.ToChoices(enumChoices);
        }
    }
}
