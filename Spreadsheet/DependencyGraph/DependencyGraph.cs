// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // Represents all values with dependents as pairs
        private Dictionary<string, HashSet<string>> dependents;
        // Represents all vlaues with dependees as pairs
        private Dictionary<string, HashSet<string>> dependees;
        private int p_size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return p_size; }
            private set { p_size = value; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))
                    return dependees[s].Count;
                return 0;
            }

        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
                return true;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s))
                return true;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependents.Count != 0)
                if (dependents.ContainsKey(s))
                    return dependents[s];
            return new List<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (dependees.Count != 0)
                if (dependees.ContainsKey(s))
                    return dependees[s];
            return new List<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // Tracks if a dependency was created
            bool pairAdded = false;

            // Dependents
            // Key 's' already exists
            if (dependents.ContainsKey(s))
            {
                // 't' is not yet a dependent
                if (!dependents[s].Contains(t))
                {
                    dependents[s].Add(t);
                    pairAdded = true;
                }

                // if 't' is already a dependent of 's', do nothing...
            }
            // Key 's' doesn't yet exist
            else
            {
                dependents.Add(s, new HashSet<string>());
                dependents[s].Add(t);
                pairAdded = true;
            }

            // Dependees
            // Key 't' already exists
            if (dependees.ContainsKey(t))
            {
                // 's' is not yet a dependee
                if (!dependees[t].Contains(s))
                {
                    dependees[t].Add(s);
                    pairAdded = true;
                }

                // if 's' is already a dependee of 't', do nothing...
            }
            // Key 't' doesn't yet exist
            else
            {
                dependees.Add(t, new HashSet<string>());
                dependees[t].Add(s);
                pairAdded = true;
            }

            // Increment size if successful dependency creation
            if (pairAdded)
                Size++;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            bool pairRemoved = false;

            // Dependents
            // Key 's' exists
            if (dependents.ContainsKey(s))
            {
                // 't' is a dependent
                if (dependents[s].Contains(t))
                {
                    // 's' has only one dependent left
                    if (dependents[s].Count == 1)
                    {
                        // 's' has no more dependents and is removed
                        dependents.Remove(s);
                        pairRemoved = true;
                    }

                    // More than one dependent left
                    else
                    {
                        dependents[s].Remove(t);
                        pairRemoved = true;
                    }
                }
            }
            // Key 's' does not exist, do nothing...

            // Dependees
            // Key 't' exists
            if (dependees.ContainsKey(t))
            {
                // 's' is a dependee
                if (dependees[t].Contains(s))
                {
                    // 't' has only one dependee left
                    if (dependees[t].Count == 1)
                    {
                        // 't' has no more dependees and is removed
                        dependees.Remove(t);
                        pairRemoved = true;
                    }

                    // More than one dependee left
                    else
                    {
                        dependees[t].Remove(s);
                        pairRemoved = true;
                    }
                }
            }
            // Key 't' does not exist, do nothing...

            // Decrement size if removed dependency
            if (pairRemoved)
            {
                Size--;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {

            if (dependents.ContainsKey(s))
                foreach (string r in new HashSet<string>(dependents[s]))
                {
                    RemoveDependency(s, r);
                }
            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }


        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if(dependees.ContainsKey(s))
            foreach (string r in new HashSet<string>(dependees[s]))
            {
                RemoveDependency(r, s);
            }
            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
        }

    }

}
