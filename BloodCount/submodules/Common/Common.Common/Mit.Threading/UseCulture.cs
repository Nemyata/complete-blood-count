using System;
using System.Globalization;
using System.Threading;

namespace Common.Threading
{
    /// <summary>
    /// Set the <see cref="Thread.CurrentThread.CurrentCulture"/> to 
    /// CultureInfo with given name until instance is disposed.
    /// Set null or string.Empty to use <see cref="CultureInfo.InvariantCulture"/>.</summary>
    public class UseCulture : IDisposable
    {
        private readonly CultureInfo old;

        /// <summary>
        /// Initialize a new instance of the UseCulture class
        /// using <see cref="CultureInfo.InvariantCulture"/>.</summary>
        public UseCulture() {
            old = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        /// <summary>
        /// Initialize a new instance of the UseCulture class
        /// using <see cref="CultureInfo"/> with given <paramref name="cultureName"/>.</summary>
        /// <param name="cultureName">Culture name to use during instance is not disposed.</param>
        public UseCulture(string cultureName)
        {
            old = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
        }

        ~UseCulture() {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Revert the CurrentThread.CurrentCulture back to original setting
        /// and removing instance from finalization queue.</summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Revert the CurrentThread.CurrentCulture back to original setting.</summary>
        public void Dispose(bool disposing) {
            Thread.CurrentThread.CurrentCulture = old;
        }
    }
}
