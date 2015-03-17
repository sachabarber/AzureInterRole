using System;


namespace InterRoleBroadcast
{
    public static class Logger
    {
        private static Action<string> AddLogEntryAction { get; set; }


        public static void Initialize(Action<string> addLogEntry)
        {
            AddLogEntryAction = addLogEntry;
        }



        public static void AddLogEntry(string entry)
        {
            if (AddLogEntryAction != null)
            {
                AddLogEntryAction(entry);
            }
        }



        public static void AddLogEntry(Exception ex)
        {
            while (ex != null)
            {
                AddLogEntry(ex.ToString());

                ex = ex.InnerException;
            }
        }
    }
}
