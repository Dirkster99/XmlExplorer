namespace Settings
{
    using MLib.Interfaces;
    using Settings.Interfaces;
    using Settings.Internal;

    /// <summary>
    /// Helper class to initialize an
    /// <see cref="ISettingsManager"/> service interface.
    /// </summary>
    public sealed class SettingsManager
    {
        /// <summary>
        /// Hidden default constructor.
        /// </summary>
        private SettingsManager()
        {
        }

        /// <summary>
        /// Gets an instance of an object that implements the
        /// <see cref="ISettingsManager"/> interface.
        /// </summary>
        /// <returns></returns>
        public static ISettingsManager GetInstance(IAppearanceManager appearanceManager)
        {
            return new SettingsManagerImpl(appearanceManager);
        }
    }
}
