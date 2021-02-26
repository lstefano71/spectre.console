namespace Spectre.Console
{
    /// <summary>
    /// Represents a context that can be used to interact with a <see cref="UpdatePanel"/>.
    /// </summary>
    /// <typeparam name="T">f.</typeparam>
    public class UpdatePanelContext<T>
        where T : struct
    {
        private readonly ProgressContext _context;
        private readonly ProgressTask _task;
        private readonly string _updatePanelStateKey;

        internal UpdatePanelContext(ProgressContext context, ProgressTask task, string updatePanelStateKey)
        {
            _context = context;
            _task = task;
            _updatePanelStateKey = updatePanelStateKey;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public T Status
        {
            get => _task.State.Get<T>(_updatePanelStateKey);
        }

        /// <summary>
        /// Refreshes the status.
        /// </summary>
        public void Refresh()
        {
            _context.Refresh();
        }

        /// <summary>
        /// Sets the current state.
        /// </summary>
        /// <param name="status">The new state.</param>
        public void SetStatus(T status)
        {
            _task.State.Update<T>(_updatePanelStateKey, _ => status);
        }
    }
}