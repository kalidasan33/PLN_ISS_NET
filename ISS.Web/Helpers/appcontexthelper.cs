using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.SessionState;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;


namespace ISS.MVC.Helpers
{
    public class AppContextHelper
    {
        private static volatile AppContextHelper _instance;
        private static IUnityContainer _container;


        #region Ctor

        private AppContextHelper()
        {
           
        }

        #endregion Ctor

        #region Properties

        public static AppContextHelper Current
        {
            get
            {
                if (_instance == null)
                {                   
                        if (_instance == null)
                            _instance = new AppContextHelper();
                   
                }
                return _instance;
            }
        }

        public static IUnityContainer UnityContainer
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer().LoadConfiguration();
               
                return _container;
            }
        }

        public static T Resolve<T>(params ResolverOverride[] overrides)
        {
            return UnityContainer.Resolve<T>(overrides);
        }

        

        #endregion Properties
    }
}