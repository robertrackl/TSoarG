﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TSoar.MemberPages.ManageSettings
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="SoarDB")]
	public partial class ManageSettingsDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertaspnet_Role(aspnet_Role instance);
    partial void Updateaspnet_Role(aspnet_Role instance);
    partial void Deleteaspnet_Role(aspnet_Role instance);
    partial void InsertSETTING(SETTING instance);
    partial void UpdateSETTING(SETTING instance);
    partial void DeleteSETTING(SETTING instance);
    partial void InsertSETTINGSROLESBRIDGE(SETTINGSROLESBRIDGE instance);
    partial void UpdateSETTINGSROLESBRIDGE(SETTINGSROLESBRIDGE instance);
    partial void DeleteSETTINGSROLESBRIDGE(SETTINGSROLESBRIDGE instance);
    #endregion
		
		public ManageSettingsDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["SqlConn"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public ManageSettingsDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManageSettingsDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManageSettingsDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManageSettingsDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<aspnet_Role> aspnet_Roles
		{
			get
			{
				return this.GetTable<aspnet_Role>();
			}
		}
		
		public System.Data.Linq.Table<SETTING> SETTINGs
		{
			get
			{
				return this.GetTable<SETTING>();
			}
		}
		
		public System.Data.Linq.Table<SETTINGSROLESBRIDGE> SETTINGSROLESBRIDGEs
		{
			get
			{
				return this.GetTable<SETTINGSROLESBRIDGE>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_ManageSettings")]
		public ISingleResult<sp_ManageSettingsResult> sp_ManageSettings([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(256)")] string sUser)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), sUser);
			return ((ISingleResult<sp_ManageSettingsResult>)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.aspnet_Roles")]
	public partial class aspnet_Role : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _ApplicationId;
		
		private System.Guid _RoleId;
		
		private string _RoleName;
		
		private string _LoweredRoleName;
		
		private string _Description;
		
		private EntitySet<SETTINGSROLESBRIDGE> _SETTINGSROLESBRIDGEs;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnApplicationIdChanging(System.Guid value);
    partial void OnApplicationIdChanged();
    partial void OnRoleIdChanging(System.Guid value);
    partial void OnRoleIdChanged();
    partial void OnRoleNameChanging(string value);
    partial void OnRoleNameChanged();
    partial void OnLoweredRoleNameChanging(string value);
    partial void OnLoweredRoleNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    #endregion
		
		public aspnet_Role()
		{
			this._SETTINGSROLESBRIDGEs = new EntitySet<SETTINGSROLESBRIDGE>(new Action<SETTINGSROLESBRIDGE>(this.attach_SETTINGSROLESBRIDGEs), new Action<SETTINGSROLESBRIDGE>(this.detach_SETTINGSROLESBRIDGEs));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ApplicationId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ApplicationId
		{
			get
			{
				return this._ApplicationId;
			}
			set
			{
				if ((this._ApplicationId != value))
				{
					this.OnApplicationIdChanging(value);
					this.SendPropertyChanging();
					this._ApplicationId = value;
					this.SendPropertyChanged("ApplicationId");
					this.OnApplicationIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RoleId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid RoleId
		{
			get
			{
				return this._RoleId;
			}
			set
			{
				if ((this._RoleId != value))
				{
					this.OnRoleIdChanging(value);
					this.SendPropertyChanging();
					this._RoleId = value;
					this.SendPropertyChanged("RoleId");
					this.OnRoleIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RoleName", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string RoleName
		{
			get
			{
				return this._RoleName;
			}
			set
			{
				if ((this._RoleName != value))
				{
					this.OnRoleNameChanging(value);
					this.SendPropertyChanging();
					this._RoleName = value;
					this.SendPropertyChanged("RoleName");
					this.OnRoleNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LoweredRoleName", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string LoweredRoleName
		{
			get
			{
				return this._LoweredRoleName;
			}
			set
			{
				if ((this._LoweredRoleName != value))
				{
					this.OnLoweredRoleNameChanging(value);
					this.SendPropertyChanging();
					this._LoweredRoleName = value;
					this.SendPropertyChanged("LoweredRoleName");
					this.OnLoweredRoleNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(256)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="aspnet_Role_SETTINGSROLESBRIDGE", Storage="_SETTINGSROLESBRIDGEs", ThisKey="RoleId", OtherKey="uiRole")]
		public EntitySet<SETTINGSROLESBRIDGE> SETTINGSROLESBRIDGEs
		{
			get
			{
				return this._SETTINGSROLESBRIDGEs;
			}
			set
			{
				this._SETTINGSROLESBRIDGEs.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_SETTINGSROLESBRIDGEs(SETTINGSROLESBRIDGE entity)
		{
			this.SendPropertyChanging();
			entity.aspnet_Role = this;
		}
		
		private void detach_SETTINGSROLESBRIDGEs(SETTINGSROLESBRIDGE entity)
		{
			this.SendPropertyChanging();
			entity.aspnet_Role = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SETTINGS")]
	public partial class SETTING : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _sSettingName;
		
		private string _sExplanation;
		
		private string _sSettingValue;
		
		private string _sInTable;
		
		private string _sComments;
		
		private bool _bUserSelectable;
		
		private EntitySet<SETTINGSROLESBRIDGE> _SETTINGSROLESBRIDGEs;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnsSettingNameChanging(string value);
    partial void OnsSettingNameChanged();
    partial void OnsExplanationChanging(string value);
    partial void OnsExplanationChanged();
    partial void OnsSettingValueChanging(string value);
    partial void OnsSettingValueChanged();
    partial void OnsInTableChanging(string value);
    partial void OnsInTableChanged();
    partial void OnsCommentsChanging(string value);
    partial void OnsCommentsChanged();
    partial void OnbUserSelectableChanging(bool value);
    partial void OnbUserSelectableChanged();
    #endregion
		
		public SETTING()
		{
			this._SETTINGSROLESBRIDGEs = new EntitySet<SETTINGSROLESBRIDGE>(new Action<SETTINGSROLESBRIDGE>(this.attach_SETTINGSROLESBRIDGEs), new Action<SETTINGSROLESBRIDGE>(this.detach_SETTINGSROLESBRIDGEs));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sSettingName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string sSettingName
		{
			get
			{
				return this._sSettingName;
			}
			set
			{
				if ((this._sSettingName != value))
				{
					this.OnsSettingNameChanging(value);
					this.SendPropertyChanging();
					this._sSettingName = value;
					this.SendPropertyChanged("sSettingName");
					this.OnsSettingNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sExplanation", DbType="NVarChar(MAX)")]
		public string sExplanation
		{
			get
			{
				return this._sExplanation;
			}
			set
			{
				if ((this._sExplanation != value))
				{
					this.OnsExplanationChanging(value);
					this.SendPropertyChanging();
					this._sExplanation = value;
					this.SendPropertyChanged("sExplanation");
					this.OnsExplanationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sSettingValue", DbType="NVarChar(MAX)")]
		public string sSettingValue
		{
			get
			{
				return this._sSettingValue;
			}
			set
			{
				if ((this._sSettingValue != value))
				{
					this.OnsSettingValueChanging(value);
					this.SendPropertyChanging();
					this._sSettingValue = value;
					this.SendPropertyChanged("sSettingValue");
					this.OnsSettingValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sInTable", DbType="NVarChar(MAX)")]
		public string sInTable
		{
			get
			{
				return this._sInTable;
			}
			set
			{
				if ((this._sInTable != value))
				{
					this.OnsInTableChanging(value);
					this.SendPropertyChanging();
					this._sInTable = value;
					this.SendPropertyChanged("sInTable");
					this.OnsInTableChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sComments", DbType="NVarChar(MAX)")]
		public string sComments
		{
			get
			{
				return this._sComments;
			}
			set
			{
				if ((this._sComments != value))
				{
					this.OnsCommentsChanging(value);
					this.SendPropertyChanging();
					this._sComments = value;
					this.SendPropertyChanged("sComments");
					this.OnsCommentsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_bUserSelectable", DbType="Bit NOT NULL")]
		public bool bUserSelectable
		{
			get
			{
				return this._bUserSelectable;
			}
			set
			{
				if ((this._bUserSelectable != value))
				{
					this.OnbUserSelectableChanging(value);
					this.SendPropertyChanging();
					this._bUserSelectable = value;
					this.SendPropertyChanged("bUserSelectable");
					this.OnbUserSelectableChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SETTING_SETTINGSROLESBRIDGE", Storage="_SETTINGSROLESBRIDGEs", ThisKey="ID", OtherKey="iSetting")]
		public EntitySet<SETTINGSROLESBRIDGE> SETTINGSROLESBRIDGEs
		{
			get
			{
				return this._SETTINGSROLESBRIDGEs;
			}
			set
			{
				this._SETTINGSROLESBRIDGEs.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_SETTINGSROLESBRIDGEs(SETTINGSROLESBRIDGE entity)
		{
			this.SendPropertyChanging();
			entity.SETTING = this;
		}
		
		private void detach_SETTINGSROLESBRIDGEs(SETTINGSROLESBRIDGE entity)
		{
			this.SendPropertyChanging();
			entity.SETTING = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SETTINGSROLESBRIDGE")]
	public partial class SETTINGSROLESBRIDGE : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.Nullable<System.DateTime> _PiTRecordEntered;
		
		private System.Nullable<int> _iRecordEnteredBy;
		
		private int _iSetting;
		
		private System.Guid _uiRole;
		
		private string _sComments;
		
		private EntityRef<aspnet_Role> _aspnet_Role;
		
		private EntityRef<SETTING> _SETTING;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnPiTRecordEnteredChanging(System.Nullable<System.DateTime> value);
    partial void OnPiTRecordEnteredChanged();
    partial void OniRecordEnteredByChanging(System.Nullable<int> value);
    partial void OniRecordEnteredByChanged();
    partial void OniSettingChanging(int value);
    partial void OniSettingChanged();
    partial void OnuiRoleChanging(System.Guid value);
    partial void OnuiRoleChanged();
    partial void OnsCommentsChanging(string value);
    partial void OnsCommentsChanged();
    #endregion
		
		public SETTINGSROLESBRIDGE()
		{
			this._aspnet_Role = default(EntityRef<aspnet_Role>);
			this._SETTING = default(EntityRef<SETTING>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PiTRecordEntered", DbType="DateTime2")]
		public System.Nullable<System.DateTime> PiTRecordEntered
		{
			get
			{
				return this._PiTRecordEntered;
			}
			set
			{
				if ((this._PiTRecordEntered != value))
				{
					this.OnPiTRecordEnteredChanging(value);
					this.SendPropertyChanging();
					this._PiTRecordEntered = value;
					this.SendPropertyChanged("PiTRecordEntered");
					this.OnPiTRecordEnteredChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_iRecordEnteredBy", DbType="Int")]
		public System.Nullable<int> iRecordEnteredBy
		{
			get
			{
				return this._iRecordEnteredBy;
			}
			set
			{
				if ((this._iRecordEnteredBy != value))
				{
					this.OniRecordEnteredByChanging(value);
					this.SendPropertyChanging();
					this._iRecordEnteredBy = value;
					this.SendPropertyChanged("iRecordEnteredBy");
					this.OniRecordEnteredByChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_iSetting", DbType="Int NOT NULL")]
		public int iSetting
		{
			get
			{
				return this._iSetting;
			}
			set
			{
				if ((this._iSetting != value))
				{
					if (this._SETTING.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OniSettingChanging(value);
					this.SendPropertyChanging();
					this._iSetting = value;
					this.SendPropertyChanged("iSetting");
					this.OniSettingChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_uiRole", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid uiRole
		{
			get
			{
				return this._uiRole;
			}
			set
			{
				if ((this._uiRole != value))
				{
					if (this._aspnet_Role.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnuiRoleChanging(value);
					this.SendPropertyChanging();
					this._uiRole = value;
					this.SendPropertyChanged("uiRole");
					this.OnuiRoleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sComments", DbType="NVarChar(MAX)")]
		public string sComments
		{
			get
			{
				return this._sComments;
			}
			set
			{
				if ((this._sComments != value))
				{
					this.OnsCommentsChanging(value);
					this.SendPropertyChanging();
					this._sComments = value;
					this.SendPropertyChanged("sComments");
					this.OnsCommentsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="aspnet_Role_SETTINGSROLESBRIDGE", Storage="_aspnet_Role", ThisKey="uiRole", OtherKey="RoleId", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public aspnet_Role aspnet_Role
		{
			get
			{
				return this._aspnet_Role.Entity;
			}
			set
			{
				aspnet_Role previousValue = this._aspnet_Role.Entity;
				if (((previousValue != value) 
							|| (this._aspnet_Role.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._aspnet_Role.Entity = null;
						previousValue.SETTINGSROLESBRIDGEs.Remove(this);
					}
					this._aspnet_Role.Entity = value;
					if ((value != null))
					{
						value.SETTINGSROLESBRIDGEs.Add(this);
						this._uiRole = value.RoleId;
					}
					else
					{
						this._uiRole = default(System.Guid);
					}
					this.SendPropertyChanged("aspnet_Role");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SETTING_SETTINGSROLESBRIDGE", Storage="_SETTING", ThisKey="iSetting", OtherKey="ID", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public SETTING SETTING
		{
			get
			{
				return this._SETTING.Entity;
			}
			set
			{
				SETTING previousValue = this._SETTING.Entity;
				if (((previousValue != value) 
							|| (this._SETTING.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._SETTING.Entity = null;
						previousValue.SETTINGSROLESBRIDGEs.Remove(this);
					}
					this._SETTING.Entity = value;
					if ((value != null))
					{
						value.SETTINGSROLESBRIDGEs.Add(this);
						this._iSetting = value.ID;
					}
					else
					{
						this._iSetting = default(int);
					}
					this.SendPropertyChanged("SETTING");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	public partial class sp_ManageSettingsResult
	{
		
		private int _ID;
		
		private string _sSettingName;
		
		private string _sExplanation;
		
		private string _sSettingValue;
		
		private char _cSettingType;
		
		private string _sSelectStmnt;
		
		private System.Nullable<decimal> _dLow;
		
		private System.Nullable<decimal> _dHigh;
		
		private string _sDataValueField;
		
		private string _sDataTextField;
		
		public sp_ManageSettingsResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="Int NOT NULL")]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sSettingName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string sSettingName
		{
			get
			{
				return this._sSettingName;
			}
			set
			{
				if ((this._sSettingName != value))
				{
					this._sSettingName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sExplanation", DbType="NVarChar(MAX)")]
		public string sExplanation
		{
			get
			{
				return this._sExplanation;
			}
			set
			{
				if ((this._sExplanation != value))
				{
					this._sExplanation = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sSettingValue", DbType="NVarChar(MAX)")]
		public string sSettingValue
		{
			get
			{
				return this._sSettingValue;
			}
			set
			{
				if ((this._sSettingValue != value))
				{
					this._sSettingValue = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_cSettingType", DbType="NChar(1) NOT NULL")]
		public char cSettingType
		{
			get
			{
				return this._cSettingType;
			}
			set
			{
				if ((this._cSettingType != value))
				{
					this._cSettingType = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sSelectStmnt", DbType="NVarChar(MAX)")]
		public string sSelectStmnt
		{
			get
			{
				return this._sSelectStmnt;
			}
			set
			{
				if ((this._sSelectStmnt != value))
				{
					this._sSelectStmnt = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dLow", DbType="Decimal(12,4)")]
		public System.Nullable<decimal> dLow
		{
			get
			{
				return this._dLow;
			}
			set
			{
				if ((this._dLow != value))
				{
					this._dLow = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dHigh", DbType="Decimal(12,4)")]
		public System.Nullable<decimal> dHigh
		{
			get
			{
				return this._dHigh;
			}
			set
			{
				if ((this._dHigh != value))
				{
					this._dHigh = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sDataValueField", DbType="NVarChar(50)")]
		public string sDataValueField
		{
			get
			{
				return this._sDataValueField;
			}
			set
			{
				if ((this._sDataValueField != value))
				{
					this._sDataValueField = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sDataTextField", DbType="NVarChar(MAX)")]
		public string sDataTextField
		{
			get
			{
				return this._sDataTextField;
			}
			set
			{
				if ((this._sDataTextField != value))
				{
					this._sDataTextField = value;
				}
			}
		}
	}
}
#pragma warning restore 1591