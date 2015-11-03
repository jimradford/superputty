/*
 * Copyright (c) 2009 - 2015 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

namespace SuperPutty.Utils
{
    /// <summary>
    /// http://www.pcreview.co.uk/forums/sort-bindinglist-t3370884.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortableBindingList<T> : BindingList<T>, IList<T>
    {
        protected override bool SupportsSortingCore => true;

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
        protected override ListSortDirection SortDirectionCore => sortDirection;

        private PropertyDescriptor sortProperty;
        protected override PropertyDescriptor SortPropertyCore => sortProperty;

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

        /// <summary>Overrides call to EndNew fired when an item is added to the collection</summary>
        /// <param name="itemIndex">The index number of the newly added item</param>
        public override void EndNew(int itemIndex)
        {            
            // Check to see if the item is added to the end of the list,
            // and if so, re-sort the list.
            if (sortProperty != null && itemIndex == this.Count - 1)
                ApplySortCore(this.sortProperty, this.sortDirection);

            base.EndNew(itemIndex);                      
        }

        /// <summary>Serialize the collection</summary>
        /// <returns>An XML formatted string containing the collection</returns>
        public string SerializeXML()
        {
            XmlSerializer xs = new XmlSerializer(base.Items.GetType());

            using (StringWriter text = new StringWriter())
            {
                xs.Serialize(text, Items);
                return text.ToString();
            }        
        }

        /// <summary>Deserialize An XML string and add the items to the collection</summary>
        /// <param name="xml">A string containing the XML generated by <seealso cref="SerializeXML"/></param>
        public void DeserializeXML(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer xs = new XmlSerializer(Items.GetType());
                using (StringReader reader = new StringReader(xml))
                {
                    var collection = xs.Deserialize(reader) as ICollection<T>;
                    foreach (T itm in collection)
                    {
                        Add(itm);
                    }
                }
            }
        }
    }    
}
