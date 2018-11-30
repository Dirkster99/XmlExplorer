namespace XmlExplorerVMLib
{
    using XmlExplorerVMLib.Interfaces;
    using XmlExplorerVMLib.ViewModels;

    public static class Factory
    {
        public static IAppViewModel CreateAppViewModel()
        {
            return new AppViewModel();
        }
    }
}
