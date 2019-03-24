using System;
using System.Reflection;

using VideoFritter.Common;

namespace VideoFritter.About
{
    internal class AboutViewModel : AbstractViewModelBase
    {
        public string AssemblyTitle
        {
            get
            {
                return GetAssemblyAttribute<AssemblyTitleAttribute>().Title;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                return GetAssemblyAttribute<AssemblyCompanyAttribute>().Company;
            }
        }


        public Version AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private T GetAssemblyAttribute<T>() where T : class
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly.GetCustomAttributes(typeof(T), false) is T[] assemblyAttributes)
            {
                if (assemblyAttributes.Length < 1)
                {
                    throw new ArgumentException($"The {typeof(T)} has no value in the current Assembly!");
                }

                return assemblyAttributes[0];
            }

            throw new ArgumentException($"Failed to retrieve the {typeof(T)} from the current Assembly!");
        }

    }
}
