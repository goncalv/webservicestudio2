﻿namespace WebServiceStudio
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    internal class NullableGenericProperty : ClassProperty
    {
		public NullableGenericProperty(Type[] possibleTypes, string name, object val)	: base(possibleTypes, name, val)
        {
        }

        protected override void CreateChildren()
        {
        }

        public override object ReadChildren()
        {
            return this.Value;
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (this.Value == null)
            {
                return str;
            }
            return (str + " = " + this.Value.ToString());
        }

        [RefreshProperties(RefreshProperties.All), Editor(typeof(DynamicEditor), typeof(UITypeEditor)), TypeConverter(typeof(DynamicConverter))]
        public object Value
        {
            get
            {
                return base.InternalValue;
            }
            set
            {
                base.InternalValue = value;
                base.TreeNode.Text = this.ToString();
            }
        }
    }
}

