using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SuperPutty.Gui
{

    #region BaseViewModel
    /// <summary>
    /// Cool way to do INotifyPropertyChanged w/out code generation techniques (ide tool, postsharp)
    /// http://stackoverflow.com/questions/1315621/implementing-inotifypropertychanged-does-a-better-way-exist
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs evt)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, evt);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }

        /// <summary>
        /// Clear the old list and replace with new but only fire an single Refresh event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newData"></param>
        public static void UpdateList<T>(BindingList<T> list, List<T> newData)
        {
            // save old value, disable notifications, manipulate list
            bool raiseEvents = list.RaiseListChangedEvents;

            list.RaiseListChangedEvents = false;
            list.Clear();
            foreach (T item in newData)
            {
                list.Add(item);
            }

            // restore then fire single reset event
            list.RaiseListChangedEvents = raiseEvents;
            list.ResetBindings();
        }
    }
    #endregion
}
