using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;

namespace Cmune.DataCenter.Business
{
    public static class CmuneBoxTransactionService
    {
        public static BoxTransactionView ToBoxTransactionView(this BoxTransaction boxTransaction)
        {
            if (boxTransaction == null)
                return null;

            return new BoxTransactionView()
            {
                BoxId = boxTransaction.BoxId,
                BoxType = (BoxType)boxTransaction.BoxType,
                Cmid = boxTransaction.Cmid,
                CreditPrice = boxTransaction.CreditPrice,
                Id = boxTransaction.Id,
                Category = (BundleCategoryType)boxTransaction.Category,
                IsAdmin = boxTransaction.IsAdmin,
                PointPrice = boxTransaction.PointPrice,
                TotalCreditsAttributed = boxTransaction.TotalCreditsAttributed,
                TotalPointsAttributed = boxTransaction.TotalPointsAttributed,
                TransactionDate = boxTransaction.TransactionDate,
            };
        }

        public static BoxTransaction ToBoxTransaction(this BoxTransactionView boxTransactionView)
        {
            if (boxTransactionView == null)
                return null;
            else
                return new BoxTransaction()
                {
                    Id = boxTransactionView.Id,
                    BoxId = boxTransactionView.BoxId,
                    BoxType = (int)boxTransactionView.BoxType,
                    Category = (int)boxTransactionView.Category,
                    Cmid = boxTransactionView.Cmid,
                    CreditPrice = boxTransactionView.CreditPrice,
                    IsAdmin = boxTransactionView.IsAdmin,
                    PointPrice = boxTransactionView.PointPrice,
                    TotalCreditsAttributed = boxTransactionView.TotalCreditsAttributed,
                    TotalPointsAttributed = boxTransactionView.TotalPointsAttributed,
                    TransactionDate = boxTransactionView.TransactionDate,
                };
        }

        public static bool AddBoxTransaction(BoxTransactionView boxTransactionView)
        {
            bool success = false;

            using (var cmune = new CmuneDataContext())
            {
                if (boxTransactionView.BoxId > 0 &&
                    boxTransactionView.Cmid > 0 &&
                    boxTransactionView.Id == 0)
                //   (boxTransactionView.PointPrice > 0 || boxTransactionView.CreditPrice > 0))
                //(boxTransactionView.TotalPointsAttributed > 0 || boxTransactionView.TotalCreditsAttributed > 0))
                {
                    cmune.BoxTransactions.InsertOnSubmit(boxTransactionView.ToBoxTransaction());
                    cmune.SubmitChanges();

                    success = true;
                }
            }

            return success;
        }
    }
}