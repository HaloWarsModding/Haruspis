//-----------------------------------------------------------------------------
// File: LogLevel.cs
// Description: Contains the LogLevel enum representing different levels of logging.
//-----------------------------------------------------------------------------

namespace Ethereal.Core
{
    /// <summary>
    /// Represents different levels of logging:
    /// - Verbose: Detailed information for debugging purposes.
    /// - Debug: Information useful for debugging the application.
    /// - Information: General information about the application's operation.
    /// - Warning: Indicates potential issues that are not critical.
    /// - Error: Indicates critical errors that require attention.
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error
    }
}