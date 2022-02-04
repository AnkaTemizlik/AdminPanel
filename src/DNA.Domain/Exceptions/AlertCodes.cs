using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Exceptions {

    /*
		foreach (var item in typeof(AlertCodes).GetTypeInfo().DeclaredMembers) {
			if(!item.IsCollectible)
			Console.WriteLine($"\"{item.Name}\": \"{item.Name}\",");
		}     
     */

    public class AlertCodeGroups : Dictionary<int, string> {
        public AlertCodeGroups() {
            this.Add(0, "Informations");
            this.Add(1, "Validations Errors");
            this.Add(2, "General Errors");
            //this.Add(3, "Netsis & E-Adaptor Errors");
            this.Add(4, "E-Logo & GIB Errors");
            this.Add(5, "?");
            this.Add(6, "?");
            this.Add(7, "?");
            this.Add(8, "JOBs, Database, Logging Errors");
            this.Add(9, "Plugin Service Errors");
        }
    }

    /// <summary>
    /// 1** Validations
    /// 2** Startup / Configuration / Auth / Users / EMail Service / Help
    /// 3** NoX / E-Adaptor 
    /// 4** Diyalogo / GIB
    /// 5** 
    /// 6** 
    /// 7** 
    /// 8** JOBs / Database / SQL Server / Logging
    /// 900-999 Plugin exceptions and warnings
    /// </summary>

    public class AlertCodes {
        public static AlertCodeGroups Groups { get; set; } = new AlertCodeGroups();

        public static KeyValue SendInfo = new KeyValue(nameof(SendInfo));
        public static KeyValue SendError = new KeyValue(10, nameof(SendError));
        public static KeyValue UpdateInfo = new KeyValue(nameof(UpdateInfo));
        public static KeyValue UpdateError = new KeyValue(11, nameof(UpdateError));
        public static KeyValue InsertInfo = new KeyValue(nameof(InsertInfo));
        public static KeyValue InsertError = new KeyValue(12, nameof(InsertError));
        public static KeyValue DeleteInfo = new KeyValue(nameof(DeleteInfo));
        public static KeyValue DeleteError = new KeyValue(13, nameof(DeleteError));
        public static KeyValue GetInfo = new KeyValue(nameof(GetInfo));
        public static KeyValue GetError = new KeyValue(14, nameof(GetError));
        public static KeyValue ObjectAlreadyExists = new KeyValue(15, nameof(ObjectAlreadyExists));

        public static KeyValue UndefinedError = new KeyValue(100, nameof(UndefinedError));
        public static KeyValue ControllerIncomingData = new KeyValue(nameof(ControllerIncomingData));
        public static KeyValue NoResponseError = new KeyValue(101, nameof(NoResponseError));
        public static KeyValue DateIsNotInProperRange = new KeyValue(105, nameof(DateIsNotInProperRange));
        public static KeyValue TaxNoCanNotBe11111111111 = new KeyValue(106, nameof(TaxNoCanNotBe11111111111));
        public static KeyValue TaxNumberIsNotProperFormat = new KeyValue(107, nameof(TaxNumberIsNotProperFormat));
        
        public static KeyValue ValueCanNotBeEmpty = new KeyValue(110, nameof(ValueCanNotBeEmpty));
        public static KeyValue ValueCanNotBeZeroOrLess = new KeyValue(111, nameof(ValueCanNotBeZeroOrLess));
        public static KeyValue ValueCanNotBeLessThenZero = new KeyValue(112, nameof(ValueCanNotBeLessThenZero));
        public static KeyValue ValueMustBeProperFormat = new KeyValue(113, nameof(ValueMustBeProperFormat));
        public static KeyValue EntityNotFound = new KeyValue(114, nameof(EntityNotFound));
        public static KeyValue ValueTooBig = new KeyValue(115, nameof(ValueTooBig));

        public static KeyValue GeneralError = new KeyValue(200, nameof(GeneralError));
        public static KeyValue GeneralInfo = new KeyValue(nameof(GeneralInfo));

        public static KeyValue LoginInfo = new KeyValue(nameof(LoginInfo));
        public static KeyValue LoginError = new KeyValue(201, nameof(LoginError));
        public static KeyValue WrongEmailOrPassword = new KeyValue(202, nameof(WrongEmailOrPassword));
        public static KeyValue KeyIsNotValid = new KeyValue(203, nameof(KeyIsNotValid));
        public static KeyValue EmailAddressIsNotConfirmed = new KeyValue(204, nameof(EmailAddressIsNotConfirmed));
        public static KeyValue PasswordIsNotConfirmed = new KeyValue(205, nameof(PasswordIsNotConfirmed));

        public static KeyValue GetUsersError = new KeyValue(220, nameof(GetUsersError));

        public static KeyValue EmailSendError = new KeyValue(280, nameof(EmailSendError));

        //Help
        public static KeyValue GetHelpDocError = new KeyValue(290, nameof(GetHelpDocError));

        public static KeyValue CompleterInfo = new KeyValue(nameof(CompleterInfo));
        public static KeyValue CompleterError = new KeyValue(399, nameof(CompleterError));

        // 8** JOBs / Database / SQL Server / Visitors
        public static KeyValue DatabaseInitiationInfo = new KeyValue(nameof(DatabaseInitiationInfo));
        public static KeyValue DatabaseInitiationError = new KeyValue(800, nameof(DatabaseInitiationError));
        public static KeyValue PluginDatabaseInitiationInfo = new KeyValue(nameof(PluginDatabaseInitiationInfo));
        public static KeyValue PluginDatabaseInitiationError = new KeyValue(801, nameof(PluginDatabaseInitiationError));     
        public static KeyValue PluginStartupManagerInfo = new KeyValue(nameof(PluginStartupManagerInfo));
        public static KeyValue PluginStartupManagerError = new KeyValue(802, nameof(PluginStartupManagerError));
        public static KeyValue NoHostedServiceDefined = new KeyValue(803, nameof(NoHostedServiceDefined));
        public static KeyValue HostedServiceRunning = new KeyValue(nameof(HostedServiceRunning));
        public static KeyValue HostedServiceStoping = new KeyValue(nameof(HostedServiceStoping));
        public static KeyValue HostedServiceStarting = new KeyValue(nameof(HostedServiceStarting));
        public static KeyValue HostedServiceStoped = new KeyValue(804, nameof(HostedServiceStoped));

        public static KeyValue JobCompleted = new KeyValue(nameof(JobCompleted));
        public static KeyValue JobInit = new KeyValue(nameof(JobInit));
        public static KeyValue JobStart = new KeyValue(nameof(JobStart));
        public static KeyValue JobStoped = new KeyValue(805, nameof(JobStoped));
        public static KeyValue JobError = new KeyValue(806, nameof(JobError));
        public static KeyValue JobCanceled = new KeyValue(807, nameof(JobCanceled));

        public static KeyValue LicenseProcessInfo = new KeyValue(nameof(LicenseProcessInfo));
        public static KeyValue LicenseProcessError = new KeyValue(810, nameof(LicenseProcessError));

        public static KeyValue LogListError = new KeyValue(840, nameof(LogListError));
        public static KeyValue EntityListError = new KeyValue(841, nameof(EntityListError));

        public static KeyValue VisitorAlreadyCompleted = new KeyValue(850, nameof(VisitorAlreadyCompleted));

    }

}
