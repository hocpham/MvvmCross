// MvxSimplePropertyInfoTargetBindingFactory.cs
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
    public class MvxSimplePropertyInfoTargetBindingFactory
        : IMvxPluginTargetBindingFactory
    {
        private readonly Type _bindingType;
        private readonly MvxPropertyInfoTargetBindingFactory _innerFactory;

        public MvxSimplePropertyInfoTargetBindingFactory(Type bindingType, Type targetType, string targetName)
        {
            _bindingType = bindingType;
            _innerFactory = new MvxPropertyInfoTargetBindingFactory(targetType, targetName, CreateTargetBinding);
        }

        #region IMvxPluginTargetBindingFactory Members

        public IEnumerable<MvxTypeAndNamePair> SupportedTypes
        {
            get { return _innerFactory.SupportedTypes; }
        }

        public IMvxTargetBinding CreateBinding(object target, MvxBindingDescription description)
        {
            return _innerFactory.CreateBinding(target, description);
        }

        #endregion

        private IMvxTargetBinding CreateTargetBinding(object target, PropertyInfo targetPropertyInfo)
        {
            var targetBindingCandidate = Activator.CreateInstance(_bindingType, new[] {target, targetPropertyInfo});
            var targetBinding = targetBindingCandidate as IMvxTargetBinding;
            if (targetBinding == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning,
                                      "The TargetBinding created did not support IMvxTargetBinding");
                var disposable = targetBindingCandidate as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            return targetBinding;
        }
    }
}