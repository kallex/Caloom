namespace AaltoGlobalImpact.OIP
{
    partial class Image : IBeforeStoreHandler
    {
        public void PerformBeforeStoreUpdate()
        {
            if (ReferenceToInformation == null)
                ReferenceToInformation = OIP.ReferenceToInformation.CreateDefault();
            this.ReferenceToInformation.Title = this.Title;
            ReferenceToInformation.URL = DefaultViewSupport.GetDefaultViewURL(this);
        }
    }
}