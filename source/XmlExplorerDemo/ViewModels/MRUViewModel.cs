namespace XmlExplorerDemo.ViewModels
{
    using MRULib.MRU.Interfaces;
    using Settings.Interfaces;
    using SettingsModel.Models;
    using System;

    internal class MRUViewModel : Base.BaseViewModel
    {
        #region private fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMRUListViewModel _MRUFilelist = null;
        #endregion private fields

        #region constructors
        public MRUViewModel()
        {
            _MRUFilelist = MRULib.MRU_Service.Create_List();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the MRU File list hosted in this viewmodel.
        /// </summary>
        public IMRUListViewModel List
        {
            get { return _MRUFilelist; }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called on application shut down
        /// to write MRU Data into persistence and make it
        /// available on next start-up.
        /// </summary>
        /// <param name="sessionData"></param>
        public void WriteMruToSession(IProfile sessionData)
        {
            try  // Write back MRU data information
            {
                sessionData.LastActiveSourceFiles.Clear();
                foreach (var item in List.Entries)
                {
                    var fileRef = new FileReference();
                    fileRef.path = item.Key;
                    fileRef.LastTimeOfEdit = item.Value.LastUpdate;
                    fileRef.IsPinned = item.Value.IsPinned;
                    sessionData.LastActiveSourceFiles.Add(fileRef);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method should be called on application start-up to read MRU
        /// data from persisted session object into its associated viewmodel.
        /// </summary>
        /// <param name="sessionData"></param>
        public void ReadMruFromSession(IProfile sessionData)
        {
            try  // Read back MRU data information
            {
                List.Entries.Clear();
                foreach (var item in sessionData.LastActiveSourceFiles)
                {
                    IMRUEntryViewModel mruItem =
                        MRULib.MRU_Service.Create_Entry(item.path,
                                                        item.LastTimeOfEdit);
                    mruItem.SetIsPinned(item.IsPinned);

                    List.UpdateEntry(mruItem);
                }
            }
            catch
            {
            }
        }
        #endregion methods
    }
}
