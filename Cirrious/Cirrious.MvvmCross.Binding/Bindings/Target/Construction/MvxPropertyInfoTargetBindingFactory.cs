// MvxPropertyInfoTargetBindingFactory.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Collections.Generic;
using System.Reflection;
using Cirrious.MvvmCross.Binding.Interfaces;
using Cirrious.MvvmCross.Binding.Interfaces.Bindings.Target;
using Cirrious.MvvmCross.Binding.Interfaces.Bindings.Target.Construction;
using Cirrious.MvvmCross.Interfaces.Platform.Diagnostics;

namespace Cirrious.MvvmCross.Binding.Bindings.Target.Construction
{
    public class MvxPropertyInfoTargetBindingFactory
        : IMvxPluginTargetBindingFactory
    {
        private readonly Func<object, PropertyInfo, IMvxTargetBinding> _bindingCreator;
        private readonly string _targetName;
        private readonly Type _targetType;

        public MvxPropertyInfoTargetBindingFactory(Type targetType, string targetName,
                                                   Func<object, PropertyInfo, IMvxTargetBinding> bindingCreator)
        {
            _targetType = targetType;
            _targetName = targetName;
            _bindingCreator = bindingCreator;
        }

        protected Type TargetType
        {
            get { return _targetType; }
        }

        #region IMvxPluginTargetBindingFactory Members

        public IEnumerable<MvxTypeAndNamePair> SupportedTypes
        {
            get { return new[] {new MvxTypeAndNamePair {Name = _targetName, Type = _targetType}}; }
        }

        public IMvxTargetBinding CreateBinding(object target, MvxBindingDescription description)
        {
            var targetPropertyInfo = target.GetType().GetProperty(description.TargetName);
            if (targetPropertyInfo != null)
            {
                try
                {
                    return _bindingCreator(target, targetPropertyInfo);
                }
                catch (Exception exception)
                {
                    MvxBindingTrace.Trace(
                        MvxTraceLevel.Error,
                        "Problem creating target binding for {0} - exception {1}", _targetType.Name,
                        exception.ToString());
                }
            }

            return null;
        }

        #endregion
    }
}