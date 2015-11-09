using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

/**
 *   JMemLib ObjectWrapper
 *   https://github.com/Jiiks/JMemLib
 *
 *   Copyright (c) 2015 Jiiks
 *   
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *   
 *   The above copyright notice and this permission notice shall be included in
 *   all copies or substantial portions of the Software.
 *   
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *   THE SOFTWARE.
 */

namespace JMemLib
{
    //ObjectWrapper supports Primitives, String and byte[]
    public class ObjectWrapper : IComparable<Object>
    {
        private Object _value;
        public Object GetValue()
        {
            return _value;
        }

        public void SetValue(Object value)
        {
            if (CompareTo(value) < 1) return;

            _value = value;
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public int CompareTo(Object obj)
        {
            if (obj == null || _value == null)
            {
                return -1;
            }

            Type type = obj.GetType();

            if (type != _value.GetType() || !type.IsPrimitive || typeof (String) != type || typeof (byte[]) != type)
            {
                return -1;
            }

            if (type.IsPrimitive || typeof (String) == type)
            {
                return obj.Equals(_value) ? 1 : 0;
            }

            byte[] oldVal = _value as byte[];
            byte[] newVal = obj as byte[];

            if (oldVal == null || newVal == null)
            {
                return -1;
            }

            return oldVal.SequenceEqual(newVal) ? 1 : 0;
        }

    }
}
