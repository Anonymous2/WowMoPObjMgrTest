﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WowMoPObjMgrTest
{
    [StructLayout(LayoutKind.Sequential)]
    struct Descriptor
    {
        public IntPtr m_name; // ptr
        public uint m_size;
        public MIRROR_FLAGS m_flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DynamicDescriptor
    {
        public IntPtr m_name; // ptr
        public MIRROR_FLAGS m_flags;
    }

    class DescriptorsDumper
    {
        // offsets for 5.0.4.16016
        const int g_baseObjDescriptors = 0x01029260;
        const int g_baseItemDescriptors = 0x01028F28;
        const int g_baseContainerDescriptors = 0x01028868;
        const int g_baseUnitDescriptors = 0x01027AF0;
        const int g_basePlayerDescriptors = 0x01021E78;
        const int g_baseGameObjectDescriptors = 0x0101C158;
        const int g_baseDynamicObjectDescriptors = 0x0101C018;
        const int g_baseCorpseDescriptors = 0x0101BE28;
        const int g_baseAreaTriggerDescriptors = 0x0101BBB4;
        const int g_baseSceneObjectDescriptors = 0x0101BB84;

        const int g_baseItemDynamicDescriptors = 0x01028BEC;
        const int g_baseUnitDynamicDescriptors = 0x010273B8;
        const int g_basePlayerDynamicDescriptors = 0x0101C200;

        int[] descriptors =
        {
            g_baseObjDescriptors,
            g_baseItemDescriptors,
            g_baseContainerDescriptors,
            g_baseUnitDescriptors,
            g_basePlayerDescriptors,
            g_baseGameObjectDescriptors,
            g_baseDynamicObjectDescriptors,
            g_baseCorpseDescriptors,
            g_baseAreaTriggerDescriptors,
            g_baseSceneObjectDescriptors
        };

        int[] dynamicDescriptors =
        {
            g_baseItemDynamicDescriptors,
            g_baseUnitDynamicDescriptors,
            g_basePlayerDynamicDescriptors
        };

        public DescriptorsDumper()
        {
            var sw = new StreamWriter("descriptors.txt");

            foreach (int address in descriptors)
            {
                int i = 0;

                string currentPrefix = String.Empty;

                while (true)
                {
                    Descriptor d = Memory.Read<Descriptor>(Memory.BaseAddress + (address + i * Marshal.SizeOf(typeof(Descriptor))));

                    i++;

                    string n = Memory.ReadString(d.m_name, 255);

                    if (currentPrefix == String.Empty)
                        currentPrefix = Regex.Match(n, @"[a-zA-Z]+(?=::)").Value;

                    if (currentPrefix != String.Empty && !n.StartsWith(currentPrefix))
                        break;

                    sw.WriteLine("{0}\t{1}\t{2}", n, d.m_size, d.m_flags);
                }

                currentPrefix = String.Empty;

                sw.WriteLine();
            }

            foreach (int address in dynamicDescriptors)
            {
                int i = 0;

                string currentPrefix = String.Empty;

                while (true)
                {
                    DynamicDescriptor d = Memory.Read<DynamicDescriptor>(Memory.BaseAddress + (address + i * Marshal.SizeOf(typeof(DynamicDescriptor))));

                    i++;

                    string n = Memory.ReadString(d.m_name, 255);

                    if (currentPrefix == String.Empty)
                        currentPrefix = Regex.Match(n, @"[a-zA-Z]+(?=::)").Value;

                    if (currentPrefix != String.Empty && !n.StartsWith(currentPrefix))
                        break;

                    sw.WriteLine("{0}\t{1}", n, d.m_flags);
                }

                currentPrefix = String.Empty;

                sw.WriteLine();
            }

            sw.Close();
        }
    }
}
