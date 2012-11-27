using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AaltoGlobalImpact.OIP;

namespace TheBall
{
    [DebuggerDisplay("{Key}: TargetCount: {Targets.Count} SubscribersCount: {SubscribersList.Count}")]
    public class SubcriptionGraphItem
    {
        public string Key;
        public List<SubcriptionGraphItem> Targets = new List<SubcriptionGraphItem>();
        public List<Subscription> SubscribersList = new List<Subscription>();
        public bool IsVisited = false;

        public Subscription[] GetMySubscriptionsFromTargets()
        {
            var result = Targets.SelectMany(target => target.SubscribersList.Where(sub => sub.SubscriberRelativeLocation == Key)).
                ToArray();
            return result;
        }

        public void Visit(List<SubcriptionGraphItem> executionList)
        {
            if (IsVisited)
                return;
            foreach(var target in Targets)
            {
                target.Visit(executionList);
            }
            IsVisited = true;
            executionList.Add(this);
        }

        public void AddTargetIfMissing(string targetKey, Dictionary<string, SubcriptionGraphItem> dictionary)
        {
            var targetObject = dictionary[targetKey];
            if(Targets.Contains(targetObject) == false)
                Targets.Add(targetObject);
        }
    }
}