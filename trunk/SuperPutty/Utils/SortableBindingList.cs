using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace SuperPutty.Utils
{
    /// <summary>
    /// http://www.pcreview.co.uk/forums/sort-bindinglist-t3370884.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortableBindingList<T> : BindingList<T>
    {
        protected override bool SupportsSortingCore { get { return true; } }

        protected override bool IsSortedCore
        {
            get
            {
                for (int i = 0; i < Items.Count - 1; ++i)
                {
                    T lhs = Items[i];
                    T rhs = Items[i + 1];
                    PropertyDescriptor property = SortPropertyCore;
                    if (property != null)
                    {
                        object lhsValue = lhs == null ? null : property.GetValue(lhs);
                        object rhsValue = rhs == null ? null : property.GetValue(rhs);
                        int result;
                        if (lhsValue == null)
                        {
                            result = -1;
                        }
                        else if (rhsValue == null)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = Comparer.Default.Compare(lhsValue, rhsValue);
                        }
                        if (SortDirectionCore == ListSortDirection.Descending)
                        {
                            result = -result;
                        }
                        if (result >= 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        private ListSortDirection sortDirection;
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return sortDirection;
            }
        }

        private PropertyDescriptor sortProperty;
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return sortProperty;
            }
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            sortProperty = prop;
            sortDirection = direction;

            List<T> list = (List<T>)Items;
            list.Sort(delegate(T lhs, T rhs)
            {
                if (sortProperty != null)
                {
                    if (Object.ReferenceEquals(lhs, rhs)) return 0;
                    object lhsValue = lhs == null ? null : sortProperty.GetValue(lhs);
                    object rhsValue = rhs == null ? null : sortProperty.GetValue(rhs);
                    int result;
                    if (lhsValue == null)
                    {
                        result = -1;
                    }
                    else if (rhsValue == null)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = Comparer.Default.Compare(lhsValue, rhsValue);
                    }
                    if (sortDirection == ListSortDirection.Descending)
                    {
                        result = -result;
                    }
                    return result;
                }
                else
                {
                    return 0;
                }
            });
        }

        protected override void RemoveSortCore()
        {
            sortDirection = ListSortDirection.Ascending;
            sortProperty = null;
        }
    }
}
