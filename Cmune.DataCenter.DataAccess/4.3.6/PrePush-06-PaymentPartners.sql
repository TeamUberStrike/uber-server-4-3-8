USE [Cmune]

Set Identity_Insert [dbo].[PaymentPartners] On

Insert [dbo].[PaymentPartners] ([PartnerID], [PartnerName], [ShareOnRevenue]) Values (12, 'Kongregate', 60)

Set Identity_Insert [dbo].[PaymentPartners] Off
