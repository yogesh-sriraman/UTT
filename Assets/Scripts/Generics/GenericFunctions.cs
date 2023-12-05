using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.medcare360.utt
{
    public static class GenericFunctions
    {
        public static Dictionary<string, T> GetClassesFromAssembly<T, U>(Type type,
            U constructorParameter)
            where T: class
        {
            
            Dictionary<string, T> classes = new Dictionary<string, T>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(allTypes => allTypes.Namespace != null &&
                allTypes.Namespace == "com.medcare360.utt" &&
                allTypes.IsClass && type.IsAssignableFrom(allTypes));


            foreach (var t in types)
            {
                Type[] pTypes = new Type[1];
                pTypes[0] = constructorParameter.GetType();
                var constructor = t.GetConstructor(pTypes);

                object[] objs = new object[1];
                objs[0] = constructorParameter;
                var obj = constructor.Invoke(objs) as T;

                classes.Add(t.Name, obj);
            }

            return classes;
        }

        public static T CreateClassOfType<T>(Type type,
            Type[] constructorParams = null)
            where T : class
        {
            var res = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(allTypes => allTypes.IsClass &&
                type.IsAssignableFrom(allTypes)).FirstOrDefault();

            if (constructorParams == null)
                constructorParams = Type.EmptyTypes;
            var constructor = res.GetConstructor(constructorParams);

            T obj = constructor.Invoke(null) as T;

            return obj;

        }


        public static T GetInterfaceOfTypeFromScene<T>(Type type)
            where T : class
        {
            MonoBehaviour[] monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
            foreach (var monobehaviour in monoBehaviours)
            {
                var components = monobehaviour.GetComponents(type);

                foreach (var component in components)
                {
                    if (type.IsAssignableFrom(component.GetType()))
                    {
                        return monobehaviour as T;
                    }
                }
            }

            return null;
        }
    }

}