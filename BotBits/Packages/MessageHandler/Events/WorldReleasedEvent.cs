using PlayerIOClient;

namespace BotBits.Events
{
    /// <summary>
    ///     Occurs when crew world is getting released.
    /// </summary>
    /// <seealso cref="ReceiveEvent{T}" />
    [ReceiveEvent("worldRelease")]
    public sealed class WorldReleasedEvent : ReceiveEvent<WorldReleasedEvent>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WorldReleasedEvent" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="client"></param>
        internal WorldReleasedEvent(BotBitsClient client, Message message)
            : base(client, message)
        {
        }
    }
}