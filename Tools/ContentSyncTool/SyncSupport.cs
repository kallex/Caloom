using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ContentSyncTool
{
    public static class SyncSupport
    {
        class CopyContent
        {
            public ContentItemLocationWithMD5 Source;
            public ContentItemLocationWithMD5 Target;
        }
        public static void SynchronizeSourceListToTargetFolder(ContentItemLocationWithMD5[] sourceContentList, ContentItemLocationWithMD5[] targetContentList,
            CopySourceToTargetMethod copySourceToTarget, DeleteObsoleteTargetMethod deleteObsoleteTarget)
        {
            var sourceContents = sourceContentList.OrderBy(item => item.ContentLocation).ToArray();
            var targetContents = targetContentList.OrderBy(item => item.ContentLocation).ToArray();
            int currSourceIX = 0;
            int currTargetIX = 0;
            List<CopyContent> copyItems = new List<CopyContent>();
            List<ContentItemLocationWithMD5> deleteItems = new List<ContentItemLocationWithMD5>();
            while (currSourceIX < sourceContents.Length || currTargetIX < targetContents.Length)
            {
                var currSource = currSourceIX < sourceContents.Length ? sourceContents[currSourceIX] : null;
                var currTarget = currTargetIX < targetContents.Length ? targetContents[currTargetIX] : null;
                ContentItemLocationWithMD5 currActionSource = null;
                ContentItemLocationWithMD5 currActionTarget = null;
                if (currSource != null && currTarget != null)
                {
                    if (currSource.ContentLocation == currTarget.ContentLocation)
                    {
                        currSourceIX++;
                        currTargetIX++;
                        if (currSource.ContentMD5 == currTarget.ContentMD5)
                            continue;
                        currActionSource = currSource;
                        currActionTarget = currTarget;
                    }
                    else if (String.Compare(currSource.ContentLocation, currTarget.ContentLocation) < 0)
                    {
                        currSourceIX++;
                        currActionSource = currSource;
                        currActionTarget = new ContentItemLocationWithMD5
                            {
                                ContentLocation = currActionSource.ContentLocation
                            };
                    }
                    else // source == null, target != null
                    {
                        currTargetIX++;
                        currActionTarget = currTarget;
                    }
                }
                else if (currSource != null)
                {
                    currSourceIX++;
                    currActionSource = currSource;
                    currActionTarget = new ContentItemLocationWithMD5
                        {
                            ContentLocation = currActionSource.ContentLocation
                        };
                }
                else if (currTarget != null)
                {
                    currTargetIX++;
                    currActionTarget = currTarget;
                }

                // at this stage we have either both set (that's copy) or just target set (that's delete)
                if (currActionSource != null && currActionTarget != null)
                {
                    copyItems.Add(new CopyContent {Source = currActionSource, Target = currActionTarget});
                    //copySourceToTarget(currActionSource, currActionTarget);
                }
                else if (currActionTarget != null)
                {
                    deleteItems.Add(currActionTarget);
                    //deleteObsoleteTarget(currActionTarget);
                }
            }
            // Deleting extras
            deleteItems.ForEach(item => deleteObsoleteTarget(item));

            // Parallel copy
            copyItems.EachParallel(copyContent => copySourceToTarget(copyContent.Source, copyContent.Target));
        }

        public delegate void DeleteObsoleteTargetMethod(ContentItemLocationWithMD5 obsoleteTarget);

        public delegate void CopySourceToTargetMethod(ContentItemLocationWithMD5 source, ContentItemLocationWithMD5 target);



        /// <summary>
        /// Enumerates through each item in a list in parallel
        /// </summary>
        public static void EachParallel<T>(this IEnumerable<T> list, Action<T> action)
        {
            // enumerate the list so it can't change during execution
            // TODO: why is this happening?
            list = list.ToArray();
            var count = list.Count();

            if (count == 0)
            {
                return;
            }
            else if (count == 1)
            {
                // if there's only one element, just execute it
                action(list.First());
            }
            else
            {
                // Launch each method in it's own thread
                const int MaxHandles = 64;
                for (var offset = 0; offset <= count / MaxHandles; offset++)
                {
                    // break up the list into 64-item chunks because of a limitiation in WaitHandle
                    var chunk = list.Skip(offset * MaxHandles).Take(MaxHandles);

                    // Initialize the reset events to keep track of completed threads
                    var resetEvents = new ManualResetEvent[chunk.Count()];

                    // spawn a thread for each item in the chunk
                    int i = 0;
                    foreach (var item in chunk)
                    {
                        resetEvents[i] = new ManualResetEvent(false);
                        ThreadPool.QueueUserWorkItem(new WaitCallback((object data) =>
                        {
                            int methodIndex = (int)((object[])data)[0];

                            // Execute the method and pass in the enumerated item
                            action((T)((object[])data)[1]);

                            // Tell the calling thread that we're done
                            resetEvents[methodIndex].Set();
                        }), new object[] { i, item });
                        i++;
                    }

                    // Wait for all threads to execute
                    WaitHandle.WaitAll(resetEvents);
                }
            }
        }
    }
}
