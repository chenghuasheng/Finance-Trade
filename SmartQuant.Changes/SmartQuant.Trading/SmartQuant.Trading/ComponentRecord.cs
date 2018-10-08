using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;

namespace SmartQuant.Trading
{
	public class ComponentRecord
	{
		private const string CATEGORY_NAMING = "Naming";

		private const string CATEGORY_MISC = "Misc";

		private Guid guid;

		private ComponentType componentType;

		private string name;

		private string description;

		private FileInfo file;

		private Type runtimeType;

		private CompilerErrorCollection errors;

		private bool isChanged;

		[Category("Misc"), Description("Component GUID")]
		public Guid GUID
		{
			get
			{
				return this.guid;
			}
		}

		[Category("Misc"), Description("Component type"), ReadOnly(true)]
		public ComponentType ComponentType
		{
			get
			{
				return this.componentType;
			}
		}

		[Category("Naming"), Description("Component name")]
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		[Category("Naming"), Description("Component description")]
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		[Browsable(false)]
		public FileInfo File
		{
			get
			{
				return this.file;
			}
		}

		public bool BuiltIn
		{
			get
			{
				return this.file == null;
			}
		}

		public Type RuntimeType
		{
			get
			{
				return this.runtimeType;
			}
		}

		public bool IsChanged
		{
			get
			{
				return this.isChanged;
			}
		}

		[Browsable(false)]
		public CompilerErrorCollection Errors
		{
			get
			{
				return this.errors;
			}
		}

		internal ComponentRecord(Guid guid, ComponentType componentType, string name, string description, FileInfo file, Type runtimeType, CompilerErrorCollection errors)
		{
			this.guid = guid;
			this.componentType = componentType;
			this.name = name;
			this.description = description;
			this.file = file;
			this.runtimeType = runtimeType;
			this.errors = errors;
			this.isChanged = false;
		}

		internal void SetIsChanged(bool value)
		{
			this.isChanged = value;
		}

		internal void SetGuid(Guid guid)
		{
			this.guid = guid;
		}

		internal void SetComponentType(ComponentType componentType)
		{
			this.componentType = componentType;
		}

		internal void SetName(string name)
		{
			this.name = name;
		}

		internal void SetDescription(string description)
		{
			this.description = description;
		}

		internal void SetRuntimeType(Type runtimeType)
		{
			this.runtimeType = runtimeType;
		}

		internal void SetErrors(CompilerErrorCollection errors)
		{
			this.errors = errors;
		}
	}
}
