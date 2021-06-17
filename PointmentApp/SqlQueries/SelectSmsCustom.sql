SELECT a.* 
FROM PA_AppointmentSms a
INNER JOIN PA_Sms s 
ON a.SmsId = s.Id
WHERE 1 = 1 
	AND [Sent] = 0
	AND AppointmentId = @Id
	AND Flags & 4 = 4 -- Custom
