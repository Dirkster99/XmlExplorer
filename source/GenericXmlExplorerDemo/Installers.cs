namespace GenericXmlExplorerDemo
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using GenericXmlExplorerDemo.Interfaces;
    using GenericXmlExplorerDemo.ViewModels;
    using System;

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
        #region fields
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        /// <summary>
        /// Performs the installation in the Castle.Windsor.IWindsorContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container,
                            IConfigurationStore store)
        {

            try
            {
                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Installers)).Location;
                string dir = System.IO.Path.GetDirectoryName(fullPath);

                // register components in this DLL and make them available here
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "XmlExplorerVMLib.dll")));

                // Register application viewmodel to help castle satisfy dependencies on it
                container
                    .Register(Component.For<IGenericAppViewModel>()
                    .ImplementedBy<GenericAppViewModel>().LifestyleSingleton());
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }
    }
}