using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;

namespace SuperPutty.Utils
{

    #region PropertyNotifiableObject
    /// <summary>
    /// Cool way to do INotifyPropertyChanged w/out code generation techniques (ide tool, postsharp)
    /// http://stackoverflow.com/questions/1315621/implementing-inotifypropertychanged-does-a-better-way-exist
    /// </summary>
    public class PropertyNotifiableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs evt)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            SynchronizationContext context = this.Context;
            if (handler != null)
            {
                if (context != null && context.IsWaitNotificationRequired())
                {
                    context.Post((x) => handler(this, evt), null);
                }
                else
                {
                    handler(this, evt);
                }
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

        public SynchronizationContext Context { get; set; }
    }
    #endregion
}
