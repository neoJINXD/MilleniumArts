using System.Collections.Generic;

// Author: Anik Patel
// Description: Observable class for values that other classes will listen for
// Version: 1.0
// Changes: [N/A]
namespace Zone.Core.Utils
{
    public class Observable<T>
    {
        // Delegate and Events for subscribing (+=) and unsubscribing (-=) to the given property changing
        public delegate void ChangeValue(T v);
        public event ChangeValue propertyUpdated;

        T v;
        public T val
        {
            get
            {
                return v;
            }
            set
            {
                T previousVal = v;

                if (!EqualityComparer<T>.Default.Equals(previousVal, value))
                {
                    v = value;
                    if (propertyUpdated != null)
                    {
                        propertyUpdated(v);
                    }
                } 
            }
        }

        // Constructor
        // Input - value: the initial value for the parameter
        public Observable(T value)
        {
            v = value;
        }
    }
}
