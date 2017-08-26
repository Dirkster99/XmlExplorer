namespace XmlExplorerDemo.ViewModels.XML
{
    internal class NamespaceDefinition : Base.BaseViewModel
    {
        #region fields
        private string _namespace;
        private string _oldPrefix;
        private string _newPrefix;
        #endregion fields

        #region properties
        public string Namespace
        {
            get { return _namespace; }
            set
            {
                _namespace = value;
                base.NotifyPropertyChanged(() => Namespace);
            }
        }

        public string OldPrefix
        {
            get { return _oldPrefix; }
            set
            {
                _oldPrefix = value;
                base.NotifyPropertyChanged(() => OldPrefix);
            }
        }

        public string NewPrefix
        {
            get { return _newPrefix; }
            set
            {
                _newPrefix = value;
                base.NotifyPropertyChanged(() => NewPrefix);
            }
        }
        #endregion properties
    }
}
