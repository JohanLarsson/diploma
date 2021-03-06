using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using Diploma.Core.Framework;
using Diploma.ViewModels;

namespace Diploma
{
    public sealed class AppBootstrapper : SimpleInjectorBootstrapper
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();

            var cultureInfo = CultureInfo.GetCultureInfo("en");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        protected override IEnumerable<Assembly> SelectPackageAssemblies()
        {
            yield return typeof(DAL.Package).Assembly;
            yield return typeof(BLL.Package).Assembly;
            yield return typeof(Package).Assembly;
        }
    }
}
