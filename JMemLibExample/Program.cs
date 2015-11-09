using System;
using System.Diagnostics;
using JMemLib;

/**
 *   JMemLib Example 1    
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

namespace JMemLibExample
{
    class Program
    {
        private static readonly String[] Stats = {"Vitality", "Attunement", "Endurance", "Strength", "Dexterity", "Resistance", "Intelligence", "Faith"};
        private static readonly int[] Offsets =  {0x38,       0x40,         0x48,        0x50,       0x58,        0x80,         0x60,           0x68   };

        private const int BaseOffset = 0x00EE29E8;
        private static readonly int[] PlayerBaseOffset = { 0x18, 0x228, 0x26C, 0x14 };

        //This example reads player stats in Dark Souls 1 using JMemLib
        static void Main()
        {

            Console.WriteLine("JMemLib Example 1");
            Console.WriteLine("Dark Souls Stat Reader");

            Process darkSoulsProcess = null;
            while (darkSoulsProcess == null)
            {
                Process[] processes = Process.GetProcessesByName("DARKSOULS");
                if (processes.Length < 1) continue;
                darkSoulsProcess = processes[0];
            }

            IntPtr handle = Native.GetHandle(Native.PROCESS_VM_READ, darkSoulsProcess.Id);

            int baseAddress = (int)darkSoulsProcess.MainModule.BaseAddress + BaseOffset;

            Pointer gameBasePointer = new Pointer(baseAddress, handle);

            int playerBaseAddress = 0;

            //If playerBaseAddress is 0 then were not ingame
            while (playerBaseAddress == 0)
            {
                byte[] bytes = gameBasePointer.ReadByteArray(4, PlayerBaseOffset);
                playerBaseAddress = BitConverter.ToInt32(bytes, 0);
            }

            Pointer playerBasePointer = new Pointer(playerBaseAddress, handle);

            for (int i = 0; i < Stats.Length; i++)
            {
                int index = i;
                ObjectWrapper wrapper = new ObjectWrapper();
                wrapper.PropertyChanged += (sender, args) =>
                {
                    byte[] value = (byte[]) wrapper.GetValue();
                    Console.WriteLine(Stats[index] + ": " + BitConverter.ToInt32(value, 0));
                };
                playerBasePointer.ReadListener(4, new []{Offsets[i]}, true, wrapper);
            }

            Console.Read();
        }
    }
}
