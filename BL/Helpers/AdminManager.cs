using System.Runtime.CompilerServices;

namespace Helpers
{
    /// <summary>
    /// Internal BL manager for all Application's Clock logic policies
    /// </summary>
    internal static class AdminManager // Stage 4
    {
        #region Stage 4
        private static readonly DalApi.IDal s_dal = Factory.Get; // Stage 4
        #endregion Stage 4

        #region Stage 5
        internal static event Action? ConfigUpdatedObservers; // Prepared for stage 5 - for config update observers
        internal static event Action? ClockUpdatedObservers;  // Prepared for stage 5 - for clock update observers
        #endregion Stage 5

        #region Stage 4
        /// <summary>
        /// Property for providing/setting current configuration variable value for any BL class that may need it
        /// </summary>
        internal static TimeSpan RiskRange
        {
            get => s_dal.Config.RiskRange;
            set
            {
                s_dal.Config.RiskRange = value;
                ConfigUpdatedObservers?.Invoke(); // Stage 5
            }
        }

        /// <summary>
        /// Property for providing current application's clock value for any BL class that may need it
        /// </summary>
        internal static DateTime Now => s_dal.Config.Clock; // Stage 4
        #endregion Stage 4

        internal static void ResetDB() // Stage 4
        {
            lock (BlMutex) // Stage 7
            {
                s_dal.ResetDB();
                AdminManager.UpdateClock(AdminManager.Now); // Stage 5 - Update UI Label
                AdminManager.RiskRange = AdminManager.RiskRange; // Stage 5 - Update PL
            }
        }

        internal static void InitializeDB() // Stage 4
        {
            lock (BlMutex) // Stage 7
            {
                DalTest.Initialization.Do();
                AdminManager.UpdateClock(AdminManager.Now); // Stage 5 - Update UI Label
                AdminManager.RiskRange = AdminManager.RiskRange; // Stage 5 - Update PL
            }
        }

        private static Task? _periodicTask = null;

        /// <summary>
        /// Method to perform application's clock from any BL class as may be required
        /// </summary>
        /// <param name="newClock">Updated clock value</param>
        internal static void UpdateClock(DateTime newClock) // Stage 4-7
        {
            var oldClock = s_dal.Config.Clock; // Stage 4
            s_dal.Config.Clock = newClock; // Stage 4

            // TO_DO: Add periodic logic here, e.g. student updates
            if (_periodicTask is null || _periodicTask.IsCompleted)
                _periodicTask = Task.Run(() => CallManager.CheckCallStatuses(oldClock, newClock));
            // Notify observers
            ClockUpdatedObservers?.Invoke(); // Prepared for stage 5
        }

        #region Stage 7 base
        /// <summary>
        /// Mutex for ensuring mutual exclusion while the simulator is running
        /// </summary>
        internal static readonly object BlMutex = new();

        /// <summary>
        /// The thread of the simulator
        /// </summary>
        private static volatile Thread? s_thread;

        public static Thread? getThread()
        {
            return s_thread;
        }

        /// <summary>
        /// The interval for clock updating (in minutes per second)
        /// </summary>
        private static int s_interval = 1;

        /// <summary>
        /// The flag that indicates whether the simulator is running
        /// </summary>
        private static volatile bool s_stop = false;

        [MethodImpl(MethodImplOptions.Synchronized)] // Stage 7
        public static void ThrowOnSimulatorIsRunning()
        {
            if (s_thread is not null && Thread.CurrentThread != s_thread)
                throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
        }

        [MethodImpl(MethodImplOptions.Synchronized)] // Stage 7
        //internal static void Start(int interval)
        //{
        //    if (s_thread is null)
        //    {
        //        s_interval = interval;
        //        s_stop = false;
        //        s_thread = new(clockRunner) { Name = "ClockRunner" };
        //        s_thread.Start();
        //    }
        //}

        internal static void Start(int interval)
        {
            if (s_thread is null)
            {
                s_interval = interval;
                s_stop = false;
                s_thread = new(clockRunner) { Name = "ClockRunner" };
                Console.WriteLine($"Simulateur démarré avec Thread ID: {s_thread.ManagedThreadId}");
                s_thread.Start();
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)] // Stage 7
        internal static void Stop()
        {
            if (s_thread is not null)
            {
                s_stop = true;
                s_thread.Interrupt(); // Awake a sleeping thread
                s_thread.Name = "ClockRunner stopped";
                s_thread = null;
            }
        }

        private static Task? _simulateTask = null;

        private static void clockRunner()
        {
            while (!s_stop)
            {
                UpdateClock(Now.AddMinutes(s_interval));

                // TO_DO: Add simulation logic here (e.g., course registration simulation)
                if (_simulateTask is null || _simulateTask.IsCompleted) // Stage 7
                    _simulateTask = Task.Run(() => VolunteerManager.SimulateCallHandling());

                try
                {
                    Thread.Sleep(1000); // 1 second
                }
                catch (ThreadInterruptedException) { }
            }
        }
        #endregion Stage 7 base
    }
}
