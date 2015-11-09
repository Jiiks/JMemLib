using System;
using System.Text;
using System.Threading;

/**
 *   JMemLib Pointer
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
    public class Pointer
    {

        private int _address;
        private IntPtr _handle;
        private bool _alive, _stopped;

        public Pointer(Int32 address, IntPtr handle, bool alive = true)
        {
            _address = address;
            _handle = handle;
            _alive = alive;
        }

        public byte[] ReadByteArray(int bufferSize, int offset)
        {
            return ReadByteArray(bufferSize, new[] {offset});
        }

        public byte[] ReadByteArray(int bufferSize, int[] offsets)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[bufferSize];

            Native.ReadProcessMemory((int) _handle, _address, buffer, buffer.Length, ref bytesRead);

            if (offsets.Length < 1)
            {
                return buffer;
            }

            if (offsets.Length < 2)
            {
                Native.ReadProcessMemory((int) _handle, _address + offsets[0], buffer, buffer.Length, ref bytesRead);
                return buffer;
            }

            int tAddress = BitConverter.ToInt32(buffer, 0);

            foreach (int offset in offsets)
            {
                Native.ReadProcessMemory((int)_handle, tAddress + offset, buffer, buffer.Length, ref bytesRead);
                tAddress = BitConverter.ToInt32(buffer, 0);
            }

            return buffer;
        }

        public void Write(int value, int offset)
        {
            Write(BitConverter.GetBytes(value), offset);
        }

        public void Write(String value, int offset, Encoding encoding)
        {
            Write(encoding.GetBytes(value), offset);
        }

        public void Write(byte[] bytes, int offset)
        {
            int bytesWritten = 0;
            Native.WriteProcessMemory((int) _handle, _address + offset, bytes, bytes.Length, ref bytesWritten);
        }

        public void ReadListener(int bufferSize, int[] offsets,bool isBackground, ObjectWrapper objectWrapper)
        {
            Thread t = new Thread(() =>
            {
                while (!_stopped)
                {

                    if (_alive)
                    {
                        byte[] bytes = ReadByteArray(bufferSize, offsets);
                        objectWrapper.SetValue(bytes);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }

                    Thread.Sleep(10);

                }
            }) { IsBackground = isBackground };
            t.Start();
        }

        public void Pause()
        {
            _alive = false;
        }

        public void Resume()
        {
            _alive = true;
        }

        public void Stop()
        {
            _stopped = true;
        }
    }
}
