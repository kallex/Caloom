using System.Collections.Generic;
using System.Linq;
using TheBall;
using TheBall.CORE;
using TheBall.Interface;

namespace AaltoGlobalImpact.OIP
{
    public class ProcessConnectionReceivedDataImplementation
    {
        public static Connection GetTarget_Connection(Process process)
        {
            string connectionID = process.InitialArguments.First(ia => ia.ItemFullType == "ConnectionID").ItemValue;
            return Connection.RetrieveFromOwnerContent(InformationContext.CurrentOwner, connectionID);
        }

        public static string GetTarget_SourceContentRoot(Connection connection)
        {
            return string.Format("TheBall.CORE/DeviceMembership/{0}_Input/", connection.DeviceID);
        }

        public static string GetTarget_TargetContentRoot()
        {
            return "";
        }

        public static Dictionary<string, string> GetTarget_CategoryMap(Connection connection)
        {
            var nativeLinkDictionary = connection.CategoryLinks.Select(catLink =>
                {
                    if (catLink.LinkingType == "ONE2ONE")
                    {
                        var nativeSourceID = connection.OtherSideCategories.FirstOrDefault(osCat => osCat.ID == catLink.SourceCategoryID).NativeCategoryID;
                        var nativeTargetID = connection.ThisSideCategories.FirstOrDefault(tsCat => tsCat.ID == catLink.TargetCategoryID).NativeCategoryID;
                        return new
                            {
                                SourceCategoryID = nativeSourceID,
                                TargetCategoryID = nativeTargetID,
                            };
                    }
                    return null;
                }).Where(res => res != null).ToDictionary(natLink => natLink.SourceCategoryID,
                natLink => natLink.TargetCategoryID);
            return nativeLinkDictionary;
        }

        public static void ExecuteMethod_CallMigrationSupport(Process process, string sourceContentRoot, string targetContentRoot, Dictionary<string, string> categoryMap)
        {
            int i = 0;
        }
    }
}