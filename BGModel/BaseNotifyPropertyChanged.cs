using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace BGModel
{
    public class BaseNotifyPropertyChanged:  INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public void RaisePropertyChanged(string Propertyname)
        {
            PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(Propertyname);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, propertyChangedEventArgs);
            }
        }
        
        /*
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged == null)
                return;

            if (Application.Current?.Dispatcher?.CheckAccess() == true)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PropertyChanged(this,
                        new PropertyChangedEventArgs(propertyName));
                }));
            }
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            //if (PropertyChanged != null)
            //{
            //    PropertyChanged(this, e);
            //}
        }
        */
    }

    public class ValidationUtility : IDataErrorInfo
    {
        public string Error
        {
            get { return _error; }
        }

        public string _error;

        public string this[string columnName]
        {
            get
            {
                Type tp = this.GetType();
                PropertyInfo pi = tp.GetProperty(columnName);
                var value = pi.GetValue(this, null);
                object[] Attributes = pi.GetCustomAttributes(false);
                if (Attributes != null && Attributes.Length > 0)
                {
                    foreach (object attribute in Attributes)
                    {
                        if (attribute is ValidationAttribute)
                        {
                            ValidationAttribute vAttribute = attribute as ValidationAttribute;
                            if (!vAttribute.IsValid(value))
                            {
                                _error = vAttribute.ErrorMessage;
                                return _error;
                            }
                        }
                    }
                }
                return null;
            }
        }
    }
}
