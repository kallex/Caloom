using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentSyncTool
{
    public static class SyncSupport
    {

        public static void SynchronizeSourceListToTargetFolder(ContentItemLocationWithMD5[] sourceContentList, ContentItemLocationWithMD5[] targetContentList,
            CopySourceToTargetMethod copySourceToTarget, DeleteObsoleteTargetMethod deleteObsoleteTarget)
        {
            var sourceContents = sourceContentList.OrderBy(item => item.ContentLocation).ToArray();
            var targetContents = targetContentList.OrderBy(item => item.ContentLocation).ToArray();
            int currSourceIX = 0;
            int currTargetIX = 0;
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
                    else if (String.CompareOrdinal(currSource.ContentLocation, currTarget.ContentLocation) < 0)
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
                    copySourceToTarget(currActionSource, currActionTarget);
                else if (currActionTarget != null)
                    deleteObsoleteTarget(currActionTarget);

            }
        }

        public delegate void DeleteObsoleteTargetMethod(ContentItemLocationWithMD5 obsoleteTarget);

        public delegate void CopySourceToTargetMethod(ContentItemLocationWithMD5 source, ContentItemLocationWithMD5 target);

    }
}
