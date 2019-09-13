// Copyright (c) Rotorz Limited. All rights reserved.
// https://bitbucket.org/rotorz/script-templates-for-unity/src/master/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Toolbox.Editor.Tools
{
    /// <summary>
    /// Describes script template generator.
    /// </summary>
    public sealed class ScriptDescriptor
    {
        static ScriptDescriptor()
        {
            //gather script template generator types.
            var descriptors = new List<ScriptDescriptor>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsDefined(typeof(ScriptTemplateAttribute), true)) continue;

                    descriptors.Add(new ScriptDescriptor()
                    {
                        Type = type,
                        Attribute = (ScriptTemplateAttribute)type.GetCustomAttributes(typeof(ScriptTemplateAttribute), true).First()
                    });

                }
            }

            //sort descriptor by priority
            descriptors.Sort((a, b) => a.Attribute.Priority - b.Attribute.Priority);

            //we only want to expose read-only access to the collection
            ScriptDescriptor.descriptors = new ReadOnlyCollection<ScriptDescriptor>(descriptors);
        }

        private static readonly ReadOnlyCollection<ScriptDescriptor> descriptors;

        /// <summary>
        /// Create new instance of script generator.
        /// </summary>
        /// <returns>
        /// The new <see cref="ScriptTemplateGenerator"/> instance.
        /// </returns>
        public ScriptTemplateGenerator CreateInstance()
        {
            return Activator.CreateInstance(Type) as ScriptTemplateGenerator;
        }

        /// <summary>
        /// Type of template generator.
        /// </summary>
        public Type Type
        {
            get; private set;
        }

        /// <summary>
        /// Associated template attribute.
        /// </summary>
        public ScriptTemplateAttribute Attribute
        {
            get; private set;
        }

        /// <summary>
        /// Gets read-only collection of script template descriptors.
        /// </summary>
        public static IList<ScriptDescriptor> Descriptors => descriptors;
    }
}