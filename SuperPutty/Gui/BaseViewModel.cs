using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using SuperPutty.Utils;

namespace SuperPutty.Gui
{
    #region BaseViewModel
    /// <summary>
    /// Base view model with utilities
    /// </summary>
    public class BaseViewModel : PropertyNotifiableObject
    {

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
