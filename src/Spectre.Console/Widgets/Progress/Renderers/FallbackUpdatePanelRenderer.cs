using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Rendering;

namespace Spectre.Console
{
    /// <summary>
    /// Fallback renderer for the update panel. The actual content will be supplied by the user.
    /// </summary>
    /// <typeparam name="TState">The type of state.</typeparam>
    internal class FallbackUpdatePanelRenderer<TState> : ProgressRenderer
        where TState : struct
    {
        private readonly string _updatePanelStateKey;
        private readonly Func<TState, IRenderable> _fallbackRenderFunc;

        private readonly object _lock;
        private IRenderable? _renderable;
        private TState _lastStatus;

        public FallbackUpdatePanelRenderer(string updatePanelStateKey, Func<TState, IRenderable> fallbackRenderFunc)
        {
            _updatePanelStateKey = updatePanelStateKey;
            _fallbackRenderFunc = fallbackRenderFunc;
            _lock = new object();
        }

        public override TimeSpan RefreshRate => TimeSpan.Zero;

        public override void Update(ProgressContext context)
        {
            lock (_lock)
            {
                var task = context.GetTasks().SingleOrDefault();
                if (task != null)
                {
                    var state = task.State.Get<TState>(_updatePanelStateKey);
                    if (!_lastStatus.Equals(state))
                    {
                        _lastStatus = state;
                        _renderable = _fallbackRenderFunc(_lastStatus);
                        return;
                    }
                }

                _renderable = null;
            }
        }

        public override IEnumerable<IRenderable> Process(RenderContext context, IEnumerable<IRenderable> renderables)
        {
            lock (_lock)
            {
                var result = new List<IRenderable>();
                result.AddRange(renderables);

                if (_renderable != null)
                {
                    result.Add(_renderable);
                }

                _renderable = null;

                return result;
            }
        }
    }
}