using Nsc.Wu.BLL.Mapper;
using Nsc.Wu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.BLL.Service
{
    public class PaymentService
    {
        public ServiceResponseWithResult Detail(int id)
        {

            double deductionPercent = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["DeductionPercentage"].ToString());
            double vatPercentage = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VATPercentage"].ToString());
            Dto.PaymentDto data = new Dto.PaymentDto();

            using (Model.WarmUpEntities context = new Model.WarmUpEntities())
            {
                Model.Event paymentmodel = context.Events.FirstOrDefault(o => o.Id == id);
                if (String.IsNullOrEmpty(paymentmodel.PaymentFee))
                    return new ServiceResponseWithResult(true, "success", data);
                data.Id = paymentmodel.Id;
                data.TaxAmount = (Convert.ToDouble(paymentmodel.PaymentFee) * (vatPercentage / 100));
                data.Amount = Convert.ToDouble(paymentmodel.PaymentFee) + data.TaxAmount;


                data.DeductionAmont = Convert.ToDouble(paymentmodel.PaymentFee) - (Convert.ToDouble(paymentmodel.PaymentFee) * (deductionPercent / 100));
                data.TransferAmount = Convert.ToDouble(paymentmodel.PaymentFee) - data.DeductionAmont;

                return new ServiceResponseWithResult(true, "success", data);
            }
        }
        public ServiceResponseWithResult Save(Dto.PaymentDto data)
        {
            double deductionPercent = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["DeductionPercentage"].ToString());
            Model.Payment paymentmodel = AutoMapperHelper<Dto.PaymentDto, Model.Payment>.MapModel(data);
            paymentmodel.CreatedUtcDate = DateTime.UtcNow;
            paymentmodel.DeductionAmont = paymentmodel.Amount - (paymentmodel.Amount * deductionPercent / 100);
            paymentmodel.TransferAmount = paymentmodel.Amount - paymentmodel.DeductionAmont;
            paymentmodel.EventEntryCode = Token.Get(6);

            using (Model.WarmUpEntities context = new Model.WarmUpEntities())
            {
                context.Payments.Add(paymentmodel);
                context.SaveChanges();
            }

            data.Id = paymentmodel.Id;
            data.TransferAmount = paymentmodel.TransferAmount;
            data.EventEntryCode = paymentmodel.EventEntryCode;
            data.DeductionAmont = paymentmodel.DeductionAmont;
            return new ServiceResponseWithResult(true, "success", data);
        }
    }
}
