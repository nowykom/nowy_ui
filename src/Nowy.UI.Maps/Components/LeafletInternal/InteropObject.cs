using Microsoft.JSInterop;

namespace Nowy.UI.Maps.Components.LeafletInternal;

/// <summary>
/// Abstract base class for types that represent JavaScript objects.
/// </summary>
public abstract class InteropObject : IAsyncDisposable
{
    /// <summary>
    /// The JavaScript binder used to talk to the interop layer.
    /// </summary>
    internal LeafletMapJSBinder? JSBinder;

    /// <summary>
    /// The JavaScript runtime object reference.
    /// </summary>
    internal IJSObjectReference? JSObjectReference;

    public bool IsBound => this.JSBinder is not null;

    /// <summary>
    /// Creates the JavaScript object, stores a reference to it and the
    /// JavaScript runtime object used to create it.
    /// </summary>
    /// <param name="jsBinder">The JavaScript binder used to talk to the interop layer.</param>
    /// <returns>A task that represents the async create operation.</returns>
    internal async Task BindJsObjectReference(LeafletMapJSBinder jsBinder)
    {
        this.JSBinder = jsBinder;

        if (this.JSBinder is null) throw new ArgumentNullException(nameof(this.JSBinder));
        if (this.JSBinder?.JSRuntime is null) throw new ArgumentNullException(nameof(this.JSBinder.JSRuntime));

        this.JSObjectReference = await this._createJsObjectRef();
    }

    /// <summary>
    /// Creates the JavaScript object
    /// </summary>
    /// <returns>The reference to the new JavaScript object.</returns>
    protected abstract Task<IJSObjectReference> _createJsObjectRef();

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        if (this.JSObjectReference is { })
            await this.JSObjectReference.DisposeAsync();
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the JavaScript binding has not been
    /// set up for this object.
    /// </summary>
    /// <param name="nullBindingMessage">The error message to be used when an exception is thrown.</param>
    internal void GuardAgainstNullBinding(string nullBindingMessage)
    {
        if (this.JSBinder is null)
        {
            throw new InvalidOperationException(nullBindingMessage);
        }
    }
}
