namespace XmlExplorerVMLib.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using XmlExplorerVMLib.Interfaces;
    using XmlExplorerVMLib.ViewModels;

    /// <summary>
    /// This class gets picked up by from Castle.Windsor because
    /// it implements the <see cref="IWindsorInstaller"/> interface.
    /// 
    /// The <see cref="IWindsorInstaller"/> interface is used by the
    /// container to resolve installers when calling
    /// <see cref="IWindsorContainer"/>.Install(FromAssembly.This()); 
    /// </summary>
    public class Installers : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the Castle.Windsor.IWindsorContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        void IWindsorInstaller.Install(IWindsorContainer container,
                                       IConfigurationStore store)
        {
            // resolve this viewmodel class via its interface
            container
                .Register(Component.For<IAppViewModel>()
                .ImplementedBy<AppViewModel>().LifestyleSingleton());
        }
    }
}
