SELECT a.* 
FROM PA_AppointmentSms a
INNER JOIN PA_Sms s 
ON a.SmsId = s.Id
WHERE [Sent] = 0
	AND AppointmentId = @Id
	AND Flags & 1 = 1 -- StateChangeMessage



	