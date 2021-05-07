DECLARE @RefDate DATETIME = CAST(@StartDate as DATE)
DECLARE @RefCount INT
-- Gelen kayıt henüz veritabanında insert/update olmadı. 
-- sadece Id bilgisi 0'dan büyükse veritabanında kayıt mevcut, 
-- değilse insert işlemi olacağından veritabannında henüz yok.
-- Bu yüzden kaydın diğer alanlarını kullanırken dikkat etmek lazım.
SELECT @RefCount = COUNT(*) FROM PA_Appointment
WHERE StartDate BETWEEN @RefDate AND @RefDate + 1
	AND (CASE WHEN @RefDate = CAST(StartDate as DATE) AND @Id = Id THEN 1 ELSE 0 END) = 0 -- kendisini dahil etme
SELECT @RefCount AS [Key]