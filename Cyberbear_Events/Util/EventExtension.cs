using System;
using System.ComponentModel;

namespace MandrakeEvents.Util
{   
    //taken from old SPIPware
    public static class EventExtensions
    {   
        /// <summary>Raises the event (on the UI thread if available).</summary>
        /// <param name="multicastDelegate">The event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <returns>The return value of the event invocation or null if none.</returns>
        /// <summary>
        /// Safely raises any EventHandler event asynchronously.
        /// </summary>
        /// <param name="sender">The object raising the event (usually this).</param>
        /// <param name="e">The EventArgs for this event.</param>
        public static void Raise(this MulticastDelegate thisEvent, object sender,
            EventArgs e)
        {
            EventHandler uiMethod;
            ISynchronizeInvoke target;
            AsyncCallback callback = new AsyncCallback(EndAsynchronousEvent);

            foreach (Delegate d in thisEvent.GetInvocationList())
            {
                uiMethod = d as EventHandler;
                if (uiMethod != null)
                {
                    target = d.Target as ISynchronizeInvoke;
                    if (target != null) target.BeginInvoke(uiMethod, new[] { sender, e });
                    else uiMethod.BeginInvoke(sender, e, callback, uiMethod);
                }
            }
        }

        private static void EndAsynchronousEvent(IAsyncResult result)
        {
            ((EventHandler)result.AsyncState).EndInvoke(result);
        }
    }
}
