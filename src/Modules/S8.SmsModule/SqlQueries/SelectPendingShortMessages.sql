SELECT * 
FROM PA_Sms
WHERE [Sent] = 0
	AND [ScheduledSendingTime] < GETDATE()
	AND [ScheduledSendingTime] > GETDATE() - 1