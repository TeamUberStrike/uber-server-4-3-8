using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Core.Types;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Business
{
    public static class PromotionContentService
    {
        #region PromotionContentModel

        public static PromotionContentViewModel ToPromotionContentModel(this PromotionContent promotionContent)
        {
            var promotionContentModel = new PromotionContentViewModel();

            promotionContentModel.PromotionContentId = promotionContent.PromotionContentId;
            promotionContentModel.Name = promotionContent.Name;
            promotionContentModel.StartDate = promotionContent.StartDate;
            promotionContentModel.EndDate = promotionContent.EndDate;
            promotionContentModel.IsPermanent = promotionContent.IsPermanent;

            return promotionContentModel;
        }

        public static PromotionContent ToPromotionContent(this PromotionContentViewModel promotionContentModel)
        {
            var promotionContent = new PromotionContent();

            promotionContent.PromotionContentId = promotionContentModel.PromotionContentId;
            promotionContent.Name = promotionContentModel.Name;
            promotionContent.StartDate = promotionContentModel.StartDate;
            promotionContent.EndDate = promotionContentModel.EndDate;
            promotionContent.IsPermanent = promotionContentModel.IsPermanent;

            return promotionContent;
        }

        public static void CopyFromPromotionContentModel(this PromotionContent promotionContent, PromotionContentViewModel promotionContentModel)
        {
            promotionContent.Name = promotionContentModel.Name;
            promotionContent.StartDate = promotionContentModel.StartDate;
            promotionContent.EndDate = promotionContentModel.EndDate;
            promotionContent.IsPermanent = promotionContentModel.IsPermanent;
        }

        public static IQueryable<PromotionContentViewModel> ToPromotionContentModelQueryable(this IQueryable<PromotionContent> promotionContentList)
        {
            var promotionContentModelList = from promotionContent in promotionContentList select promotionContent.ToPromotionContentModel();
            return promotionContentModelList;
        }

        #endregion

        #region PromotionContentElementModel


        public static PromotionContentElementViewModel ToPromotionContentElementModel(this PromotionContentElement promotionContentElement)
        {
            var promotionContentElementModel = new PromotionContentElementViewModel();

            promotionContentElementModel.PromotionContentElementId = promotionContentElement.PromotionContentElementId;
            promotionContentElementModel.PromotionContentId = promotionContentElement.PromotionContentId;
            promotionContentElementModel.ChannelType = (ChannelType) promotionContentElement.ChannelType;
            promotionContentElementModel.ChannelElement = (ChannelElement) promotionContentElement.ChannelLocation;
            promotionContentElementModel.Filename = promotionContentElement.Filename;
            promotionContentElementModel.FilenameTitle = promotionContentElement.FilenameTitle;
            promotionContentElementModel.AnchorLink = promotionContentElement.AnchorLink;

            return promotionContentElementModel;
        }

        public static PromotionContentElement ToPromotionContentElement(this PromotionContentElementViewModel promotionContentElementModel)
        {
            var promotionContentElement = new PromotionContentElement();

            promotionContentElement.PromotionContentElementId = promotionContentElementModel.PromotionContentElementId;
            promotionContentElement.PromotionContentId = promotionContentElementModel.PromotionContentId;
            promotionContentElement.ChannelType = (int)promotionContentElementModel.ChannelType;
            promotionContentElement.ChannelLocation = (int) promotionContentElementModel.ChannelElement;
            promotionContentElement.Filename = promotionContentElementModel.Filename;
            promotionContentElement.FilenameTitle = promotionContentElementModel.FilenameTitle;
            promotionContentElement.AnchorLink = promotionContentElementModel.AnchorLink;
            return promotionContentElement;
        }

        public static void CopyFromPromotionContentElementModel(this PromotionContentElement promotionContentElement, PromotionContentElementViewModel promotionContentElementModel)
        {
            promotionContentElement.PromotionContentId = promotionContentElementModel.PromotionContentId;
            promotionContentElement.ChannelType = (int)promotionContentElementModel.ChannelType;
            promotionContentElement.ChannelLocation = (int) promotionContentElementModel.ChannelElement;
            promotionContentElement.Filename = promotionContentElementModel.Filename;
            promotionContentElement.FilenameTitle = promotionContentElementModel.FilenameTitle;
            promotionContentElement.AnchorLink = promotionContentElementModel.AnchorLink;
        }

        public static IQueryable<PromotionContentElementViewModel> ToPromotionContentElementModelQueryable(this IQueryable<PromotionContentElement> promotionContentElementList)
        {
            var promotionContentElementModelList = from promotionContentElement in promotionContentElementList select promotionContentElement.ToPromotionContentElementModel();
            return promotionContentElementModelList;
        }

        public static List<PromotionContentElement> ToPromotionContentElementModelQueryable(this List<PromotionContentElementViewModel> promotionContentElementModelList)
        {
            var promotionContentElementList = from promotionContentElementModel in promotionContentElementModelList select promotionContentElementModel.ToPromotionContentElement();
            return promotionContentElementList.ToList();
        }

        #endregion

        public static PromotionContentViewModel GetLastPromotionContent()
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContent = uberstrikeDB.PromotionContents.Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now).FirstOrDefault();
                
                if (promotionContent == null)
                {
                    promotionContent = uberstrikeDB.PromotionContents.Where(d => d.IsPermanent == true).FirstOrDefault();
                }

                if (promotionContent != null)
                {
                    var promotionContentModel = promotionContent.ToPromotionContentModel();
                    promotionContentModel.PromotionContentElements = GetPromotionContentElements(promotionContent.PromotionContentId);
                    return promotionContentModel;
                }
                else
                    return null;
            }
        }

        public static PromotionContentViewModel GetPromotionContent(int promotionContentId)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                PromotionContent promotionContent = uberstrikeDB.PromotionContents.SingleOrDefault(mS => mS.PromotionContentId == promotionContentId);
                if (promotionContent != null)
                {
                    var promotionContentModel = promotionContent.ToPromotionContentModel();
                    promotionContentModel.PromotionContentElements = GetPromotionContentElements(promotionContentModel.PromotionContentId);
                    return promotionContentModel;
                }
                return null;
            }
        }

        public static PromotionContentElementViewModel GetPortalPromotionElement(ChannelElement channelElement, List<PromotionContentElementViewModel> promotionContentElements)
        {
            var promotionElement = from pce in promotionContentElements
                       where pce.ChannelElement == channelElement && pce.ChannelType == ChannelType.WebPortal
                       select pce;
            return promotionElement.FirstOrDefault();
        }

        public static PromotionContentElementViewModel GetFacebookPromotionElement(ChannelElement channelElement, List<PromotionContentElementViewModel> promotionContentElements)
        {
            var promotionElement = from pce in promotionContentElements
                                   where pce.ChannelElement == channelElement && pce.ChannelType == ChannelType.WebFacebook
                                   select pce;
            return promotionElement.FirstOrDefault();
        }
        

        public static List<PromotionContentElementViewModel> GetPromotionContentElements(int promotionContentId)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                List<PromotionContentElementViewModel> promotionContentElementModelList = uberstrikeDB.PromotionContentElements.Where(d => d.PromotionContentId == promotionContentId).ToPromotionContentElementModelQueryable().ToList();
                return promotionContentElementModelList;
            }
        }

        public static List<PromotionContentViewModel> GetPromotionContents()
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                List<PromotionContentViewModel> promotionContentModelList = uberstrikeDB.PromotionContents.ToPromotionContentModelQueryable().ToList();
                foreach (var promotionContent in promotionContentModelList)
                {
                    promotionContent.PromotionContentElements = GetPromotionContentElements(promotionContent.PromotionContentId);
                }
                return promotionContentModelList;
            }
        }

        public static int AddPromotionContent(PromotionContentViewModel promotionContentModel, List<PromotionContentElementViewModel> promotionContentElementsModel = null)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContent = promotionContentModel.ToPromotionContent();
                uberstrikeDB.PromotionContents.InsertOnSubmit(promotionContent);
                uberstrikeDB.SubmitChanges();

                if (promotionContentElementsModel != null)
                {
                    uberstrikeDB.PromotionContentElements.InsertAllOnSubmit(promotionContentElementsModel.ToPromotionContentElementModelQueryable());
                    uberstrikeDB.SubmitChanges();
                }
                return promotionContent.PromotionContentId;
            }
        }

        public static int AddPromotionContentElement(PromotionContentElementViewModel promotionContentElementModel)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContentElement = promotionContentElementModel.ToPromotionContentElement();
                uberstrikeDB.PromotionContentElements.InsertOnSubmit(promotionContentElement);
                uberstrikeDB.SubmitChanges();
                return promotionContentElement.PromotionContentElementId;
            }
        }


        public static bool AddPromotionContentElements(List<PromotionContentElementViewModel> promotionContentElementModels)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                uberstrikeDB.PromotionContentElements.InsertAllOnSubmit(promotionContentElementModels.ToPromotionContentElementModelQueryable());
                uberstrikeDB.SubmitChanges();

                return true;
            }
        }


        public static bool DeletePromotionContent(int promotionContentId)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContent = uberstrikeDB.PromotionContents.SingleOrDefault(d => d.PromotionContentId == promotionContentId);
                if (promotionContent != null)
                {
                    var promotionContentElements = uberstrikeDB.PromotionContentElements.Where(d => d.PromotionContentId == promotionContent.PromotionContentId).ToList();

                    uberstrikeDB.PromotionContentElements.DeleteAllOnSubmit(promotionContentElements);
                    uberstrikeDB.PromotionContents.DeleteOnSubmit(promotionContent);

                    uberstrikeDB.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool EditPromotionContent(PromotionContentViewModel promotionContentModel)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContent = uberstrikeDB.PromotionContents.SingleOrDefault(mS => mS.PromotionContentId == promotionContentModel.PromotionContentId);

                if (promotionContent != null)
                {
                    promotionContent.CopyFromPromotionContentModel(promotionContentModel);
                    uberstrikeDB.SubmitChanges();

                    return true;
                }
                return false;
            }
        }

        public static bool DeletePromotionContentElements(int promotionContentId)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContentElements = uberstrikeDB.PromotionContentElements.Where(mS => mS.PromotionContentId == promotionContentId);
                uberstrikeDB.PromotionContentElements.DeleteAllOnSubmit(promotionContentElements);
                uberstrikeDB.SubmitChanges();

                return true;
            }
        }


        public static bool EditPromotionContentElement(PromotionContentElementViewModel promotionContentElementModel)
        {
            using (UberstrikeDataContext uberstrikeDB = new UberstrikeDataContext())
            {
                var promotionContentElement = uberstrikeDB.PromotionContentElements.SingleOrDefault(mS => mS.PromotionContentElementId == promotionContentElementModel.PromotionContentElementId);

                if (promotionContentElement != null)
                {
                    promotionContentElement.CopyFromPromotionContentElementModel(promotionContentElementModel);
                    uberstrikeDB.SubmitChanges();

                    return true;
                }
                return false;
            }
        }

        public static List<ChannelElement> GetChannelElementList(ChannelElement selectedChannelLocation = ChannelElement.Banner)
        {
            var listChannelElement = new List<ChannelElement>();

            listChannelElement.Add(ChannelElement.Banner);
            listChannelElement.Add(ChannelElement.RightPromotion);

            return listChannelElement;
        }

        public static List<ChannelType> GetChannelTypeList(ChannelType selectedChannelType = ChannelType.WebFacebook)
        {
            var listOfChannelType = new List<ChannelType>();

            listOfChannelType.Add(ChannelType.WebFacebook);
            listOfChannelType.Add(ChannelType.WebPortal);

            return listOfChannelType;
        }
    }
}
