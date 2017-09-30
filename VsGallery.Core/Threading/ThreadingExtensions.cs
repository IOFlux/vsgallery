﻿using System;
using System.ComponentModel;


namespace VsGallery.Core.Threading
{
    /// <summary>
    /// See: http://stackoverflow.com/questions/18881808/raising-events-on-separate-thread
    /// </summary>
    public static class ThreadingExtensions
    {
        /// <summary>
        /// Extension method which marshals events back onto the main thread for either WPF or Winforms
        /// </summary>
        /// <param name="multicast"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Raise(this MulticastDelegate multicast, object sender, EventArgs args)
        {
            foreach (Delegate del in multicast.GetInvocationList())
            {
                // Maybe its WinForms?
                if (del.Target is ISynchronizeInvoke syncTarget && syncTarget.InvokeRequired)
                {
                    // WinForms target which requires marshaling
                    syncTarget.BeginInvoke(del, new object[] { sender, args });
                }
                else
                {
                    // Just do it.
                    del.DynamicInvoke(sender, args);
                }
            }
        }

        /// <summary>
        /// Extension method which marshals actions back onto the main thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void Raise<T>(this Action<T> action, T args)
        {
            // Maybe its WinForms?
            if (action.Target is ISynchronizeInvoke syncTarget && syncTarget.InvokeRequired)
            {
                // WinForms target which requires marshaling
                syncTarget.BeginInvoke(action, new object[] { args });
            }
            else
            {
                // Just do it.
                action.DynamicInvoke(args);
            }
        }
    }
}
