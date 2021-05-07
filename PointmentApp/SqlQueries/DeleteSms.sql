DELETE a
FROM PA_AppointmentSms a
WHERE SmsId = @SmsId;

DELETE a
FROM PA_Sms a
WHERE Id = @SmsId;