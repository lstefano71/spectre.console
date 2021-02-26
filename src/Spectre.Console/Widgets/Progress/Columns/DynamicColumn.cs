using System;
using Spectre.Console.Rendering;

namespace Spectre.Console
{
    internal sealed class DynamicColumn<T> : ProgressColumn
        where T : struct
    {
        private readonly Func<T, IRenderable> _func;
        private readonly string _stateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicColumn{T}"/> class.
        /// </summary>
        /// <param name="func">The function to generate the rendered text given a value T.</param>
        /// <param name="stateKey">Key mapping to the state value.</param>
        public DynamicColumn(Func<T, IRenderable> func, string stateKey)
        {
            this._func = func;
            _stateKey = stateKey;
        }

        /// <inheritdoc/>
        protected internal override bool NoWrap => false;

        /// <inheritdoc/>
        public override IRenderable Render(RenderContext context, ProgressTask task, TimeSpan deltaTime)
        {
            var state = task.State.Get<T>(_stateKey);
            return _func(state);
        }
    }
}