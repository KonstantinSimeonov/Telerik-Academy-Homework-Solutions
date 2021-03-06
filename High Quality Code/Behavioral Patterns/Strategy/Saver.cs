﻿namespace GenericMemento
{
    using System;
    using System.Collections.Generic;

    using GenericMemento.CloningStrategies;

    public class Saver<T> : IStateSaver<T>
    {
        /// <summary>
        /// Serves as a container for different cloning routines.
        /// </summary>
        private static IList<ICloningStrategy<T>> cloningStratgies = new List<ICloningStrategy<T>>() 
        {
            new CustomCloning<T>(),
            new DotNetCloning<T>(),
            new SerializationCloning<T>(),
            new ReflectionCloning<T>()
        };

        private IMemory<T> memory;

        public Saver()
        {
            this.memory = new Memory<T>();
        }

        public bool HasStates
        {
            get
            {
                return !this.memory.IsEmpty;
            }
        }

        public void SaveState(T obj)
        {
            this.memory.PushItem(this.DeepClone(obj));
        }

        public T GetState()
        {
            return this.memory.GetItem();
        }

        // uses the .IsMatch method of the cloning routines to determine which one to use
        private T DeepClone(T obj)
        {
            T result = default(T);
            var hasMatched = false;

            foreach (var cloningStrategy in cloningStratgies)
            {
                if(cloningStrategy.IsMatch(obj))
                {
                    result = cloningStrategy.Clone(obj);
                    hasMatched = true;
                    break;
                }
            }

            if(!hasMatched)
            {
                throw new ArgumentException("Me no can clone dis T_T");
            }

            return result;
        }
    }
}