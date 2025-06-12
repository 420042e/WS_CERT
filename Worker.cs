using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Serilog;
using System.Collections;


namespace WS_CERT; // Aseg�rate de que coincida con tu namespace

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    // Usamos inyecci�n de dependencias para obtener el logger configurado con Serilog
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("El Worker se ha iniciado en: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Aqu� va la l�gica principal de tu servicio
            _logger.LogInformation("El Worker est� ejecutando una tarea en: {time}", DateTimeOffset.Now);
            _logger.LogWarning("Este es un mensaje de advertencia de ejemplo.");

            string[] hosts = { "DEVCRM13", "DEVAII00", "DEVAIO00", "DEVAIC00", "DEVYAS00", "DEVANT01", "DEVPWB00", "DEVCFW00", "DEVODS00", "DEVSQR00", "DEVBEN01", "DEVSWD00", "DEVSWW00", "DEVSEG00", "DEVPSD01", "DEVPSD02", "DEVAPD00", "DEVAPD01", "DEVYAB00", "DEVSLK00", "DEVSLA00", "DEVARR00", "DEVARR01", "DEVMON00", "DEVSAG00", "DEVSAS00", "DEVPSA00", "DEVSAA00", "DEVSAD00", "DEVSRC00", "DEVSRD00", "DEVSRA00", "DEVSRT00", "DEVING00", "DEVRSS00", "DEVRDS00", "DEVRDS01", "DEVECS00", "DEVSPS00", "DEVARR02", "DEVCRS00", "DEVZCB01", "DEVZCO00", "DEVZCM00", "DEVZCS00", "DEVECD00", "DEVPMD00", "DEVPMA00", "DEVZCN00", "DEVSBD00", "DEVWST01", "DEVIBS01", "DEVFAW00", "DEVBCR00", "DEVMCR00", "DEVWCR00", "DEVBAA00", "DEVCFD10", "DEVCRM02", "DEVRHY00", "DEVOTP00", "DEVRYN00", "DEVBID00", "DEVWUT00", "DEVSGN00", "DEVDNW00", "DEVDNR00", "DEVDNA00", "DEVDNS00", "DEVDND00", "DEVMDP00", "DEVSAW01", "DEVTMP00", "DEVWSA16", "DEVCWD01", "DEVCWN00", "DEVDWR00", "DEVOCM00", "DEVNMW01", "DEVNMW02", "DEVTLM00", "DEVTLM01", "DEVWAC03", "DEVBMQ03", "DEVCCL00", "DEVDOC02", "DEVFTP00", "DEVNEB00", "DEVNEF00", "DEVNED00", "DEVACH13", "DEVWSA11", "DEVMLA00", "DEVFRW01", "DEVFRW02", "DEVMLD00", "DEVANT00", "DEVTNW00", "DEVFEN00", "DEVMGD00", "DEVZCB00", "DEVBCD00", "DEVDOA00", "DEVBFD01", "DEVFIW00", "DEVAND00", "DEVCRM01", "DEVCRE00", "DEVSSE00", "DEVTMB00", "DEVSRS00", "DEVINA01", "DEVHOW01", "DEVT7X10", "DEVNCD01", "DEVBTC00", "DEVSMS00", "DEVT7X11", "DEVSNW00", "DEVIIS10", "DEVWAC01", "DEVPSW00", "DEVHID01", "DEVSSI03", "DEVEXT03", "DEVBIO01", "DEVIND01", "DEVSSD01", "DEVDFS00", "DEVHOG00", "DEVTNH00", "DEVHOW00", "DEVWSD00", "DEVSCN02", "DEVAPI00", "DEVWSA02", "DEVCRM00", "DEVDBR00", "DEVFRW00", "DEVWAC00", "DEVSSI02", "DEVITF00", "DEVFEN02", "DEVNMW00", "DEVIMD10", "DEVSOD10", "DEVDOC01", "DEVSSI01", "DEVWAE00", "DEVNAC00", "DEVWWW10", "DEVRMD00", "DEVOFW00", "DEVBMQ01", "DEVULW00", "DEVCEW00", "DEVWSE00", "DEVHOD00", "DEVSSD16", "DEVSLD00", "DEVWPW00", "DEVOFD00", "DEVHOB00", "DEVAGD00", "DEVSSI00", "DEVTMW01", "DEVCRE01", "DEVMRP00", "DEVODD00", "DEVXTR12", "DEVACH10", "DEVBCW00", "DEVCWA00", "DEVFDB10", "DEVZCW04", "DEVBOC01", "DEVDBS08", "DEVBMQ00", "DEVWSS00", "DEVWAC02", "DEVCWN10", "DEVAID10", "DEVBFW00", "DEVSAH00", "DEVACH02", "DEVODW00", "DEVRMQ00", "DEVRHA00", "DEVRHA01", "DEVRHD00", "DEVRHD01", "DEVRHR00", "DEVSNA00", "DEVSLK01", "DEVBCA00", "DEVNOD00", "DEVRMC00", "DEVTMW00", "DEVSCN12", "DEVIBW00", "DEVNMF00", "DEVSCD12", "DEVMMQ00", "DEVZCW01", "DEVSLH00", "DEVZCW00", "DEVWCC00", "DEVWCM00", "DEVMGT00", "DEVSQI00", "DEVSIS02", "DEVSQE00", "DEVPSC00", "DEVBEN00", "DEVSLB00", "DEVCEN01", "DEVSWW01", "DEVCWN01", "DEVTKN00", "DEVSLH01", "DEVZCW02", "DEVTKN01", "DEVSDB00", "DEVINS00", "DEVPSD00", "DEVIBS10", "DEVDOD01", "DEVHIS01", "DEVRVW00", "DEVBAW00", "DEVBMB01", "DEVBOC00", "DEVZCD00", "DEVDBS12", "DEVCMQ01", "DEVSCN11", "DEVEXT02", "DEVSSE01", "DEVFAD00", "DEVSOW00", "DEVPAW00", "DEVBOC02", "DEVSND00", "DEVACR02", "DEVBMQ02", "DEVSGN01", "DEVHOC00", "DEVHID00", "DEVCED01", "DEVSLS00", "DEVACH01", "DEVACH11", "DEVACR00", "DEVACR01", "DEVAGW00", "DEVAIA00", "DEVBBW00", "DEVBBW01", "DEVBBW02", "DEVBBW03", "DEVBFA00", "DEVBMD01", "DEVDWI00", "DEVFEN01", "DEVIBD00", "DEVNSB00", "DEVNSW00", "DEVRYN01", "DEVRYN02", "DEVRYN03", "DEVRYN04", "DEVSQD00", "DEVSQS00", "DEVSQW00", "DEVSQW01", "DEVSSD02", "DEVWEB16", "DEVWEB17", "DEVWIS00", "DEVCRS02" }; // Agrega aquí todos los servidores que necesites
            //string[] hosts = { "CERAPI00", "CERULW00", "CERAII00", "CERAII01", "CERAIO00", "CERAIO01", "CERAIC00", "CERAIB00", "CERYAD01", "CERYAD00", "CERMRP00", "cerdnw00", "cerdnd00", "CERANT01", "CERDOD01", "CERSQI00", "CERPWB00", "CERBTC00", "CERSWD00", "CERPAW00", "CERPSD01", "CERPSD02", "CERAPD01", "CERYAB00", "CERSLH01", "CERSLH03", "CERSLK00", "CERAIC01", "CERSOD10", "CERSLH00", "CERSLH02", "CERCSW00", "CERBWC00", "CERBMS00", "CERSAG00", "CERSAS00", "CERSAA00", "CERSAD00", "CERPSA00", "CERRSS00", "CERYAS00", "CERZCB01", "CERZCO00", "CERZCM00", "CERZCS00", "CERZCN00", "CERECS00", "CERECD00", "CERAPD00", "CERSPS00", "CERSRA00", "CERSBD00", "CERSRT00", "CERSRD00", "CERBMQ02", "CERMCR00", "CERWCR00", "CERBCR00", "CERYCM01", "CERCCL00", "CERCCL01", "CERNEB00", "CERNEF00", "CERNED00", "CERBFD01", "CERFAW00", "CERWSA16", "CERSSE01", "CERMMQ00", "CERCWM01", "CERCWM11", "CERIAD00", "CERIMD10", "CERZCD00", "CERCWF13", "CERBMQ01", "CERWAC02", "CERBEN01", "CERSOW00", "CERTMD00", "CERWSD00", "CERCWN01", "CERPSC00", "CERCRT00", "CERBMD01", "CERIND01", "CERSQE00", "CERNAC00", "CEROTP00", "CERHOB00", "CERRVW00", "CERWSS00", "CERCFW00", "CERBAW00", "CERCFD10", "CERSSE00", "CERCBD01", "CERIBS10", "CERZCW01", "CERDYW16", "CERACH02", "CERWAC00", "CERIGD00", "CERWSE00", "CERSNW00", "CERNMF00", "CERMGT00", "CERFEN02", "CERSLS00", "CERSSD16", "CERBCD00", "CERFIW00", "CERACH12", "CERSSD01", "CERCWA00", "CERSSR00", "CERMLD00", "CERBMB00", "CERFTP00", "CERBID00", "CERACH13", "CERDBS08", "CERNAC10", "CERDFS00", "CERSLK02", "CERMLA00", "CERBIO01", "CERINA02", "CERBMQ00", "CERHOG00", "CERDWH10", "CERTMW00", "CERSLK01", "CERACH11", "CERIIS10", "CERCWM12", "CERRHD00", "CERHOD00", "CERTKN00", "CERMDP00", "CERZCW02", "CERZCW00", "CERODW00", "CERSCN11", "CERWCM00", "CERDOC01", "CERZCW03", "CERXTD00", "CERNOD00", "CERAID10", "CERCEN01", "CERBOC00", "CERWSA02", "CERZCB00", "CERFDB10", "CERIGM00", "CEROFW00", "CERNMW00", "CERWIS01", "CERFRW00", "CERINS00", "CERWCC00", "CERRHA00", "CERRMQ00", "CERRHS00", "CERHOW00", "CERSND00", "CERPSW00", "CERSLB00", "CERWUT00", "CERSCD11", "CEREXT02", "CERPSD00", "CERITF00", "CERSPA00", "CERCWM14", "CERSLD00", "CERIVD00", "CERSLA00", "CERSGN00", "CERWSA00", "CERCSW01", "CERBAA00", "CERODS00", "CERWPW00", "CERFAD00", "CERINA00", "CERODD00", "CERAND00", "CERHOW01", "CERTMW02", "CERWWW10", "CERACH10", "CERZCW04", "cersww01", "CERSIS02", "CERCED01", "CERSSI02", "CERSCD10", "CERDYD16", "CERWAE00", "CERDBS12", "CERCWM10", "CERWAC01", "CERCSD00", "CERTMH01", "CERFEN00", "CERDOA01", "CERBCA00", "CERSERVER11", "CERCMQ01", "CERSSI01", "CERSCN02", "CERHID00", "CERCWM16", "CEREXT03", "CERBFW00", "CERSPB00", "CERAGD00", "CERPSW01", "CERSAH01", "CERBOC02", "CERXTR12", "CEROFD00", "CERCWQ00", "CERCBD00", "CERBMW00", "CERCEW00", "CERACH01", "CERAGW00", "CERBBW00", "CERBBW01", "CERBBW02", "CERBEN00", "CERBUY00", "CERDOA00", "CERDOC00", "CERDWI00", "CERFEN01", "CERHVY00", "CERIBD00", "CERNSB00", "CERNSW00", "CERPGY00", "CERSQD00", "CERSQS00", "CERSQW00", "CERSQW01", "CERSSD02", "CERSWW00", "CERUEY00", "CERUEY01", "CERUEY02", "CERUSY00", "CERWEB16", "CERWEB17", "CERWIS00", "CERXCM01" }; // Agrega aquí todos los servidores que necesites
            //string[] hosts = { "SERVER9", "SERVER17", "SERVER99", "SERVER21", "SERVER19", "SERVER24", "SERVER15", "SERVER18", "SERVER32", "SERVER12", "SERVER14", "SERVER99_B", "SERVER11", "SERVER11A", "SERVER23", "SERVER28", "SERVER31", "SERVER34", "SERVER12A", "SERVER14A", "SERVER20", "SERVER27", "SERVER11B", "SERVER20A", "SERVER20B", "SERVER33", "SERVER98", "SERVER29", "SERVER14B", "SERVER99_C", "SERVER99_D", "SERVER99_E", "SERVER14C", "SERVER15A", "SERVER98_E", "SERVER98_F", "SERVER98_G", "SERVER98_H", "SERVER98_C", "SERVER98_D", "SERVER8", "SERVER30AZ", "SERVER31AZ", "SERVER99AZ" }; // Agrega aquí todos los servidores que necesites
            //string[] hosts = { "BTBULW00", "BTBSWW00", "BTBNMW00", "BTBWAE00", "BTBOPE01", "BTBFEN02", "BTBIMP01", "BTBIMP00", "BTBSIS02", "BTBAND00", "BTBSRS00", "BTBDFS00", "BTBCCS00", "BTBBZA02", "BTBCAL00", "BTBBFD01", "BTBBFW00", "BTBACR00", "BTBACR01", "BTBMDS01", "BTBACR02", "BTBACR03", "BTBNCW01", "BTBPSC02", "BTBXTD00", "BTBOFD00", "BTBPSW00", "BTBBEN00", "BTBFEN00", "BTBBMW00", "BTBFTP00", "BTBLPD00", "BTBNAC01", "BTBNAC10", "BTBPSC01", "BTBOFW01", "BTBOTP00", "BTBSWW01", "BTBSQW00", "BTBSQW01", "BTBITF00", "BTBNSW00", "BTBSPM00", "BTBSQL00", "BTBSQS00", "BTBWIS00", "BTBWUT00", "BTBPRB00", "BTBSOC01", "BTBPSC00", "BTBHID00", "BTBVER00", "BTBATC10", "BTBRHA00", "BTBODD00", "BTBODS00", "BTBODW00", "BTBOPE00", "BTBIBS00", "BTBRMQ00", "BTBAGW00", "BTBSIW00", "BTBBMQ04", "BTBDOC00", "BTBWAC03", "BTBWAC04", "BTBBOC00", "BTBDOW00", "BTBDOA00", "BTBFAD00", "BTBBZD01", "BTBBZD00", "BTBCWD01", "BTBDBR01", "BTBSLB00", "BTBCWD02", "BTBTKN01", "BTBSWD00", "BTBDBS12", "BTBWAC05", "BTBBMQ05", "BTBACH12", "BTBACH13", "BTBACH10", "BTBSSD00", "BTBSLA00", "BTBDOD00", "BTBIND01", "BTBVEP01", "BTBPSW01", "BTBINS00", "BTBSGN10", "BTBBAW00", "BTBFRW00", "BTBBAA00", "BTBNOD00", "BTBAIW01", "BTBAIS00", "BTBAIS01", "BTBAID00", "BTBAID01", "BTBBTC00", "BTBSUS00", "BTBWIS02", "BTBSQE00", "BTBWSE00", "BTBCEW00", "BTBSQI00", "BTBSQR00", "BTBSOW00", "BTBDOA01", "BTBWSA01", "BTBTMB40", "BTBSAP40", "BTBWEB46", "BTBCCD00", "BTBMEP00", "BTBLOP00", "BTBBBW01", "BTBWAC02", "BTBBMQ02", "BTBBCD00", "BTBSWI40", "BTBSND00", "BTBCWM11", "BTBSNW00", "BTBAPD00", "BTBAPD01", "BTBDMD00", "BTBLPD01", "BTBWAC01", "BTBBMQ01", "BTBSOD00", "BTBSOD01", "BTBCWA00", "BTBWSS00", "BTBWSD00", "BTBNSH05", "BTBSAH01", "BTBSLK01", "BTBSLK00", "BTBSCM00", "BTBAGD00", "BTBTMH02", "BTBKIS03", "BTBCAM00", "BTBYAD00", "BTBYAD01", "BTBEXT04", "BTBGEN20", "BTBGEN22", "BTBBFA02", "BTBBFA03", "BTBGEN09", "BTBBOC01", "BTBWAC06", "BTBBMQ06", "BTBWAC07", "BTBBMQ07", "BTBGEN18", "BTBGEN19", "BTBGEN21", "BTBGEN08", "BTBGEN23", "BTBBFD02", "BTBIMD01", "BTBAGW01", "BTBGEN01", "BTBGEN02", "BTBGEN03", "BTBGEN04", "BTBGEN05", "BTBGEN06", "BTBGEN07", "BTBGEN11", "BTBIMD00", "BTBGEN15", "BTBGEN13", "BTBGEN17", "BTBGEN10", "BTBGEN12", "BTBGEN14", "BTBGEN16", "BTBDNT00", "BTBTMW06", "BTBBMD99", "BTBCED01", "BTBCEN01", "BTBFIW00", "BTBDYD01", "BTBSOM01", "BTBSOM02", "BTBSOM00", "BTBSOM03", "BTBSOM04", "BTBCBD00", "BTBRVW00", "BTBWPW00", "BTBDCO01", "BTBBMW02", "BTBBBW00", "BTBWAC00", "BTBIBS12", "BTBIBS13", "BTBDYN00", "BTBDYN01", "BTBWEB11", "BTBWIS01", "BTBSDW00", "BTBLPD02", "BTBOOS00", "BTBISS10", "BTBSLK02", "BTBNSH04", "BTBNSH42", "BTBNSH03", "BTBKIS02", "BTBDYD00", "BTBDYR00", "BTBTMW07", "BTBBOC02", "BTBFAW00", "BTBHOW01", "BTBHOW00", "BTBHOG00", "BTBVEA01", "BTBEPO00", "BTBEPO01", "BTBODA00", "BTBRHD00", "BTBACH01", "BTBNCD01", "BTBTMW08", "BTBHSL00", "BTBHSL01", "BTBBZA00", "BTBDWI00", "BTBACD00", "BTBAIW02", "BTBAIW03", "BTBZCW02", "BTBZCB00", "BTBWCC00", "BTBSCD00", "BTBSCD01", "BTBWCM00", "BTBMMQ00", "BTBZCW00", "BTBZCW01", "BTBZCW03", "BTBZCW04", "BTBVEA02", "BTBANS00", "BTBRDS00", "BTBKFK00", "BTBCWM16", "BTBBOC03", "BTBAPI01", "BTBPSD01", "BTBPSD02", "BTBAPI02", "BTBRZX00", "BTBVEA03", "BTBWEB16", "BTBSPE02", "BTBBZA01", "BTBHOC00", "BTBHOB00", "BTBHOD00", "BTBBBW02", "BTBVEA04", "BTBAPI03", "BTBAIS02", "BTBWEB17", "BTBAIS03", "BTBAPB00", "BTBSNA01", "BTBSLD01", "BTBDOC02", "BTBSSI02", "BTBRLY00", "BTBMGT00", "BTBCWF13", "BTBBMQ09", "BTBWAC09", "BTBOFW02", "BTBSLD00", "BTBRLY01", "BTBABD00", "BTBABD01", "BTBBIM00", "BTBIMW00", "BTBIMW01", "BTBIMW02", "BTBIMW03", "BTBTMH01", "BTBTMW03", "BTBTMW04", "BTBTMW05", "BTBSRH00", "BTBTMH42", "BTBTMW42", "BTBTMW41", "BTBTMW40", "BTBTMW43", "BTBTMW44", "BTBIMM00", "BTBIMM01", "BTBIMM02", "BTBIMM03", "BTBSSR00", "BTBNSH43", "BTBMRP00", "BTBSSD01", "BTBNMD00", "BTBSSE00", "BTBSSE01", "BTBSSD02", "BTBCON01", "BTBXCM01", "BTBDBS08", "BTBINA01", "BTBBMQ00", "BTBBMQ03", "BTBZCB01", "BTBZCO00", "BTBZCM00", "BTBZCS00", "BTBRLY03", "BTBPSC03", "BTBPSW02", "BTBPOC00", "BTBAII00", "BTBAII01", "BTBAIO02", "BTBAIO05", "BTBAIC01", "BTBAIB01", "BTBAII02", "BTBAIO00", "BTBAIO01", "BTBAIO03", "BTBAIO04", "BTBAIC00", "BTBAIC02", "BTBAIB00", "BTBAIB02", "BTBPSW03", "BTBKFK01", "BTBKFK02", "BTBKFK03", "BTBSSI01", "BTBRLY02", "BTBMDW00", "BTBSCN03", "BTBAGD01", "BTBNTP02", "BTBPRS00", "BTBIBD00", "BTBRVW03", "BTBYPD01", "BTBRVW02", "BTBDFS02", "BTBYPD00", "BTBDOC01", "BTBIGD00", "BTBIGF00", "BTBDNW00", "BTBDUO00", "BTBDNR00", "BTBDNA00", "BTBDNS00", "BTBDND00", "BTBDND01", "BTBEXT06", "BTBDUO01", "BTBANT01", "BTBWAC11", "BTBWAC12", "BTBBMQ11", "BTBBMQ12", "BTBSMS02", "BTBSMD00", "BTBIGM00", "BTBOTP01", "BTBOTP02", "BTBSMS03", "BTBSMD01", "BTBSMB00", "BTBAPI00", "BTBIBS10", "BTBIBS11", "BTBCWQ00", "BTBCWQ01", "BTBPWB00", "BTBIBD01", "BTBCFD10", "BTBCFW00", "BTBEPO02", "BTBEPO03", "BTBBMB00", "BTBBMB01", "BTBVEP07", "BTBPAW00", "BTBVID00", "BTBTMW45", "BTBBRK00", "BTBOCS00", "BTBWWW02", "BTBWWW03", "BTBAPD03", "BTBRDS01", "BTBYAB00", "BTBVEP08", "BTBDFS01", "BTBSQD00", "BTBBRK01", "BTBNAC02", "BTBNAC03", "BTBARN00", "BTBENC00", "BTBFEN01", "BTBACH11", "BTBBIM01", "BTBBMH00", "BTBSPS00", "BTBYAS01", "BTBDNS01", "BTBZCM01", "BTBZCO01", "BTBOTD00", "BTBOTD01", "BTBBWC00", "BTBBWC01", "BTBUID00", "BTBBMS00", "BTBBMS01", "BTBUID01", "BTBEXT03", "BTBIMB00", "BTBIMB01", "BTBIMB02", "BTBSAG00", "BTBSAS00", "BTBSAA00", "BTBSAD00", "BTBPSA00", "BTBSLH00", "BTBTKN00", "BTBZCN00", "BTBRMS00", "BTBIBI00", "BTBPSC04", "BTBPSC05", "BTBPSC06", "BTBPSW04", "BTBPSW05", "BTBPSW06", "BTBYPE00", "BTBYPE01", "BTBYPE02", "BTBYPH00", "BTBRSS00", "BTBBAP00", "BTBSLS01", "BTBDOC03", "BTBSLY00", "BTBSLS02", "BTBRDS02", "BTBRDS03", "BTBOYD00", "BTBOYD01", "BTBIYD00", "BTBIYD01", "BTBRDS04", "BTBVEP00", "BTBOTP03", "BTBOTP04", "BTBOTB00", "BTBOTB01", "BTBFRW01", "BTBFRW02", "BTBMDB00", "BTBMDB01", "BTBMDB02", "BTBTMB00", "BTBECS00", "BTBECD00", "BTBRDY00", "BTBADS00", "BTBDTX00", "BTBSBD00", "BTBSRC00", "BTBSRD00", "BTBSRT00", "BTBSRA00", "BTBCWM14", "BTBEXT02", "BTBBBW04", "BTBBBW03", "BTBBMQ08", "BTBWAC08", "BTBCWM12", "BTBSIC02", "BTBZCD00", "BTBZCD01", "BTBIAD00", "BTBIAW00", "BTBBCR00", "BTBMCR00", "BTBWCR00", "BTBSTW00", "BTBCWM01", "BTBCWM02", "BTBCWM03", "BTBDWA00", "btbqus01", "BTBPMA00", "BTBTMH40", "BTBYCM01", "BTBDWR00", "BTBMDP00", "BTBNMR00", "BTBIIS10", "BTBXTR11", "BTBWST01", "BTBWSA16", "BTBPMD00", "BTBQRD00", "BTBQRA00", "BTBMLD00", "BTBMLA00", "BTBKMS11", "BTBSWF00", "BTBSSD16", "BTBOSS10", "BTBWSA02", "BTBSLH04", "BTBSSR02", "BTBWIT00", "BTBXTR12", "BTBWWW10", "BTBMPA00", "BTBMPA01", "BTBMPD00", "BTBRVW01", "BTBSAP00", "BTBBBW05", "BTBNCW00", "BTBNAC00", "BTBBBW06", "BTBTMB01", "BTBBOY00", "BTBOCM00", "BTBNMW01", "BTBNMW02", "BTBBMQ34", "BTBBMQ33", "BTBBMQ30", "BTBBMQ63", "BTBBMQ64", "BTBBMQ60", "BTBWAC33", "BTBWAC34", "BTBWAC30", "BTBWAC63", "BTBWAC64", "BTBWAC60", "BTBSLK03", "BTBSLK04", "BTBSLK06", "BTBSLB01", "BTBSLB02", "BTBAYD00", "BTBAYD01", "BTBAYD02", "BTBCCL00", "BTBCCL01", "BTBCCL02", "BTBNMD01", "BTBNMD02", "BTBNMD03", "BTBDNT01", "BTBNYW00", "BTBNYW01", "BTBBNY00", "btbexh01", "BTBYES00", "BTBYES01", "BTBYES02", "BTBYES03", "BTBSLY01", "BTBWAC31", "BTBBMQ31", "BTBWAC32", "BTBBMQ32", "BTBWAC35", "BTBBMQ35", "BTBWAC61", "BTBBMQ61", "BTBWAC62", "BTBBMQ62", "BTBWAC65", "BTBBMQ65", "BTBWAC66", "BTBBMQ66", "BTBWAC71", "BTBBMQ71", "BTBWAC72", "BTBBMQ72", "BTBOSL00", "BTBATI00", "BTBTMD00", "BTBTMD01", "BTBSLH01", "BTBNEB00", "BTBNEF00", "BTBNEO00", "BTBNED00", "BTBLMQ00", "BTBRRD00", "BTBBIO01", "BTBSMS00", "BTBRHD02", "BTBCBD01", "BTBBEN01", "BTBRHS00", "BTBNMF00", "BTBULD00", "BTBBID10", "BTBSNA00", "BTBAPD02", "BTBSP2016", "BTBBZD02", "BTBVEA00", "BTBSPB00", "BTBSCD02", "BTBSPA01", "BTBSPB02", "BTBAGD02", "BTBUID02", "BTBPSD03", "BTBDFS03", "BTBCWD03", "BTBSLD02", "BTBBAK00", "BTBOTD02", "BTBYAD02", "BTBIBD02", "BTBMON00", "BTBSMD02", "BTBABD02", "BTBSPT02", "BTBSPW00", "BTBBMB03", "BTBSPW01", "BTBIMD02", "BTBAID02", "BTBSPA03", "BTBSPA00", "BTBSPA02", "BTBSPB01", "BTBYPD02", "BTBOYD02", "BTBIYD02", "BTBECS01", "BTBECD01", "BTBSPT05", "BTBZCD02", "BTBTMB03", "BTBTMD02", "BTBSLB03", "BTBNOR01AZ", "BTBNOR02AZ", "BTBNOR03AZ", "BTBRHE01", "BTBRHE05", "BTBRHE06", "BTBRHE04", "BTBRHE02", "BTBRHE03", "BTBNTP01" }; // Agrega aquí todos los servidores que necesites
            //string[] hosts = { "BCRGON00", "BCRGON02", "BCRGON01", "BCRRMD01", "BCRROC01", "BCRNBX00", "BCRRPA01", "BCRRPA02", "BCRMON00", "BCRAUD00", "BCRSIC00", "BCRSIC02", "BCRCCX00", "BCRCSW04", "BCRCRS01", "BCRMCW00", "BCRQRD10", "BCRCSD00", "BCRCCP00", "BCRMDW01", "BCRCSC00", "BCRVPA00", "BCRMDW00", "BCRRME00", "BCRCOP00", "BCRROC02", "BCRCSF00", "BCRCEN06", "BCRCEN07", "BCRAWX00", "BCRGLB00", "BCRCEN08", "BCRCEN09", "BCRCEN10", "BCRPFD00", "BCRROC00", "BTBBCR00", "BCRCSW00", "BCRCSD01", "BCRQRD11", "BCRITR01", "BCRTEL00", "BCRPFE00", "BCRRBM00", "BCRITR00", "BCRMKT00", "BCRDNC00", "BCRDNF00", "BCRCSW05", "BCRCSW02", "BCRCSD02", "BCRCSW01", "BCRSWI00", "BCRSWI01", "BCRSWI02", "BCRRAP00", "BCRPCS00", "BCRPWD00", "BCRDVR01", "BCRCOM00", "BCRRPA00", "BCRSFS01", "BCRDUO00" }; // Agrega aquí todos los servidores que necesites
            
            int port = 443;

            
            ArrayList certificados = new ArrayList();

            foreach (string host in hosts)
            {
                try
                {
                    using (TcpClient client = new TcpClient(host, port))
                    using (SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                    {
                        sslStream.AuthenticateAsClient(host);
                        X509Certificate cert = sslStream.RemoteCertificate;

                        if (cert != null)
                        {
                            
                            var info = new CertificadoInfo
                            {
                                Host = host,
                                Sujeto = cert.Subject,
                                Emisor = cert.Issuer,
                                ValidoDesde = cert.GetEffectiveDateString(),
                                ValidoHasta = cert.GetExpirationDateString(),
                                Observacion = ""
                            };
                            certificados.Add(info);


                            _logger.LogInformation("Certificado del servidor {host}:", host);
                            _logger.LogInformation(" - Sujeto: {subject}", cert.Subject);
                            _logger.LogInformation(" - Emisor: {issuer}", cert.Issuer);
                            _logger.LogInformation(" - Válido desde: {start}", cert.GetEffectiveDateString());
                            _logger.LogInformation(" - Válido hasta: {end}", cert.GetExpirationDateString());
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo obtener el certificado del servidor {host}.", host);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //BorrarE
                    var info = new CertificadoInfo
                    {
                        Host = host,
                        Sujeto = "null",
                        Emisor = "null",
                        ValidoDesde = "null",
                        ValidoHasta = "null",
                        Observacion = "Error al obtener el certificado del servidor " + host
                    };
                    certificados.Add(info);

                    _logger.LogError(ex, "Error al obtener el certificado del servidor {host}.", host);
                }
            }



            string archive = "D:\\BCB_LOGS\\logs\\DetalleServidores-" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            using var wbook = new XLWorkbook();

            var ws1 = wbook.Worksheets.Add("Resumen");
            ws1.Cell(1, 1).Value = "SERVIDOR";
            ws1.Cell(1, 2).Value = "SUJETO ";
            ws1.Cell(1, 3).Value = "EMISOR";
            ws1.Cell(1, 4).Value = "START";
            ws1.Cell(1, 5).Value = "END";
            ws1.Cell(1, 6).Value = "OBSERVACIONES";


            // Volcar datos desde el ArrayList
            int row = 2;
            DateTime hoy = DateTime.Today;
            foreach (CertificadoInfo cert in certificados)
            {
                ws1.Cell(row, 1).Value = cert.Host;
                ws1.Cell(row, 2).Value = cert.Sujeto;
                ws1.Cell(row, 3).Value = cert.Emisor;
                ws1.Cell(row, 4).Value = cert.ValidoDesde;
                ws1.Cell(row, 5).Value = cert.ValidoHasta;
                
                
                if (DateTime.TryParse(cert.ValidoHasta, out DateTime fechaVencimiento))
                {
                    double diasRestantes = (fechaVencimiento - hoy).TotalDays;

                    if (diasRestantes <= 7)
                    {
                    ws1.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.Red;
                    }
                    else if (diasRestantes <= 14)
                    {
                    ws1.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.Orange;
                    }
                }


                ws1.Cell(row, 6).Value = cert.Observacion;
                row++;
            }
            wbook.SaveAs(archive);



            // Espera 1 dia antes de la siguiente ejecuci�n
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        _logger.LogInformation("El Worker se est� deteniendo.");
    }

    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // Aquí puedes personalizar la validación si lo deseas
        return true; // Aceptar todos los certificados (solo para pruebas)
    }
}