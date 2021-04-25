# Kurulum Notları

 - Veritabanı oluşturup, `ConnectionString` güncellenmeli

> Entegrasyon noktalarını tamamen bağımsız yapabilmek için Reflection ile Assembly okunacak. EBilet servisleri şimdilik referanslardan projeye takılı.


# Windows Service
#### Publish
Projeyi sağ tıkla ve publish et. 
#### Create
`sc.exe create "DNA.API Mitochondrion Service" binPath=E:\DNA\Mitochondrion\DNA.API.exe start= auto`
#### query, start, stop, delete
`sc.exe query "DNA.API Service"`
`sc.exe start "DNA.API Service"`
`sc.exe stop "DNA.API Service"`
`sc.exe delete "DNA.API Service"`
#### stop, publish, start
    cd D:\Develop\DNA\DNA\DNA.API
    sc.exe stop "DNA.API Service"; dotnet publish -c Release -o D:\Develop\DNA\publish; sc.exe start "DNA.API Service"


# Hangfire
#### Delete Queue
    DELETE j FROM HangFire.Job AS j
        LEFT JOIN HangFire.JobQueue AS jq ON jq.JobId=j.Id
        WHERE jq.[Queue]='default' AND (StateName='Enqueued');
    DELETE FROM HangFire.JobQueue WHERE [Queue]='default'


# TODO List ✓

| CreateDate | ✓ | Todo
|------------|----|------------------------
| 2020-10-03 |    | https://github.com/DevExpress-Examples/reporting-eud-designer-in-javascript-with-react
| 2020-10-08 |    | Hangfire Token Based Auth https://medium.com/we-code/hangfire-dashboard-and-jwt-authentication-751aae3bcd4a
| 2020-10-08 |    | Hangfire Readonly dashboard (yetkisiz kullanıcılar job silip tetiklemesin)
| 2020-10-08 | ✓ | IRestService eklenmesi (RestClient ile çağırılacak noktalar için ortak servis)
| 2020-10-08 |    | DxGrid multiselect aktif et.
| 2020-10-09 | ✓ | appsettings.*.json dosyalarına gerekli nesne ve ayarların başlangıçta otomatik eklenmesi (dosya yedeklemesi yap)
| 2020-10-12 | ✓ | mention fields https://blog.campvanilla.com/reactjs-input-trigger-github-twitter-mentions-8ad1d878110d (Popup ile hallettim)
| 2020-12-02 | ✓ | Https konfigurasyonu
| 2020-12-02 | ✓ | Ayarlardan Test E-posta gönderimi
| 2020-12-02 | ✓ | Test/Canlı parametresi
| 2020-12-02 |    | 
