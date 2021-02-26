using System;
using System.Threading.Tasks;
using Spectre.Console.Rendering;

namespace Spectre.Console
{
    /// <summary>
    /// Represents an update panel display.
    /// </summary>
    public class UpdatePanel
    {
        private const string UpdatePanelStateKey = "UpdatePanel";

        private readonly IAnsiConsole _console;

        /// <summary>
        /// Gets or sets a value indicating whether or not status
        /// should auto refresh. Defaults to <c>true</c>.
        /// </summary>
        public bool AutoRefresh { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePanel"/> class.
        /// </summary>
        /// <param name="console">The console.</param>
        public UpdatePanel(IAnsiConsole console)
        {
            _console = console;
        }

        /// <summary>
        /// Starts a new status display.
        /// </summary>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="renderFunc">The action to execute to generate the renderable object.</param>
        /// <param name="fallbackRenderFunc">The action to execute to generate the renderable object for non-interactive consoles.</param>
        public void Start<TState>(
            Func<UpdatePanelContext<TState>, Task> action,
            Func<TState, IRenderable> renderFunc,
            Func<TState, IRenderable>? fallbackRenderFunc = null)
            where TState : struct, IEquatable<TState>
        {
            var task = StartAsync(
                ctx =>
            {
                action(ctx);
                return Task.CompletedTask;
            }, renderFunc);

            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Starts a new status display.
        /// </summary>
        /// <typeparam name="TOut">The result type of task.</typeparam>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <param name="func">The action to execute.</param>
        /// <param name="renderFunc">The action to execute to generate the renderable object.</param>
        /// <param name="fallbackRenderFunc">The action to execute to generate the renderable object for non-interactive consoles.</param>
        /// <returns>The result.</returns>
        public TOut Start<TOut, TState>(
            Func<UpdatePanelContext<TState>, TOut> func,
            Func<TState, IRenderable> renderFunc,
            Func<TState, IRenderable>? fallbackRenderFunc = null)
            where TState : struct
        {
            var task = StartAsync(ctx => Task.FromResult(func(ctx)), renderFunc, fallbackRenderFunc);
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Starts a new status display.
        /// </summary>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="renderFunc">The action to execute to generate the renderable object.</param>
        /// <param name="fallbackRenderFunc">The action to execute to generate the renderable object for non-interactive consoles.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync<TState>(
            Func<UpdatePanelContext<TState>, Task> action,
            Func<TState, IRenderable> renderFunc,
            Func<TState, IRenderable>? fallbackRenderFunc = null)
            where TState : struct
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _ = await StartAsync<object?, TState>(
                async statusContext =>
                {
                    await action(statusContext).ConfigureAwait(false);
                    return default;
                }, renderFunc, fallbackRenderFunc).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts a new status display and returns a result.
        /// </summary>
        /// <typeparam name="TOut">The result type of task.</typeparam>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <param name="func">The action to execute.</param>
        /// <param name="renderFunc">The action to execute to generate the renderable object.</param>
        /// <param name="fallbackRenderFunc">The action to execute to generate the renderable object for non-interactive consoles.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public async Task<TOut> StartAsync<TOut, TState>(
            Func<UpdatePanelContext<TState>, Task<TOut>> func,
            Func<TState, IRenderable> renderFunc,
            Func<TState, IRenderable>? fallbackRenderFunc = null)
            where TState : struct
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var progress = new Progress(_console)
            {
                FallbackRenderer = new FallbackUpdatePanelRenderer<TState>(UpdatePanelStateKey, fallbackRenderFunc ?? renderFunc), AutoClear = true, AutoRefresh = AutoRefresh,
            };

            progress.Columns(new ProgressColumn[] { new DynamicColumn<TState>(renderFunc, UpdatePanelStateKey) });

            return await progress.StartAsync(async ctx =>
            {
                var statusContext =
                    new UpdatePanelContext<TState>(ctx, ctx.AddTask("update-panel-column"), UpdatePanelStateKey);
                return await func(statusContext).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}