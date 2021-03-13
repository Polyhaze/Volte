namespace Volte.Core.Entities
{
    public enum ModActionType : uint
    {
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Purge command invocation.
        /// </summary>
        Purge = 1,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Warn command invocation.
        /// </summary>
        Warn = 2,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful ClearWarns command invocation.
        /// </summary>
        ClearWarns = 3,
        
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Verify command invocation.
        /// </summary>
        Verify = 4,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Delete command invocation.
        /// </summary>
        Delete = 5,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Kick command invocation.
        /// </summary>
        Kick = 6,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Softban command invocation.
        /// </summary>
        Softban = 7,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful IdBan command invocation.
        /// </summary>
        IdBan = 8,
        /// <summary>
        ///     Indicates that the moderator action in question is from a successful Ban command invocation.
        /// </summary>
        Ban = 9
    }
}