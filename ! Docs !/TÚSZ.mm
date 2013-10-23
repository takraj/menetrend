<map version="1.0.1">
<!-- To view this file, download free mind mapping software FreeMind from http://freemind.sourceforge.net -->
<node CREATED="1382203234531" ID="ID_599610850" MODIFIED="1382203239961" TEXT="T&#xda;SZ">
<node CREATED="1382203251018" ID="ID_165467820" MODIFIED="1382203258382" POSITION="right" TEXT="GRAFIT">
<node CREATED="1382203741554" ID="ID_336786391" MODIFIED="1382204368978" TEXT="Gr&#xe1;f Inform&#xe1;ci&#xf3; T&#xe1;rol&#xf3;">
<font BOLD="true" NAME="SansSerif" SIZE="12"/>
<node CREATED="1382216818216" ID="ID_1795432583" MODIFIED="1382216824431" TEXT="Bin&#xe1;ris F&#xe1;jlok">
<node CREATED="1382220686481" ID="ID_1807731465" MODIFIED="1382220865100" TEXT="Legyenek sz&#xe9;tszedve a kev&#xe9;sb&#xe9; fontos adatok"/>
<node CREATED="1382216831899" ID="ID_507260759" MODIFIED="1382220857592" TEXT="ZIP VFS">
<node CREATED="1382220703348" ID="ID_750135413" MODIFIED="1382221245116" TEXT="Shapes.zip">
<node CREATED="1382220958707" ID="ID_965419733" MODIFIED="1382220989493" TEXT="shape_0.dat"/>
<node CREATED="1382220966659" ID="ID_1238348224" MODIFIED="1382220992691" TEXT="shape_1.dat"/>
<node CREATED="1382220970504" ID="ID_1841187800" MODIFIED="1382220996636" TEXT="shape_2.dat"/>
</node>
<node CREATED="1382220706390" ID="ID_820696671" MODIFIED="1382221241906" TEXT="StopDistanceMatrix.zip">
<node CREATED="1382221003877" ID="ID_937055599" MODIFIED="1382221092009" TEXT="stop_0.dat"/>
<node CREATED="1382221012376" ID="ID_762064666" MODIFIED="1382221094744" TEXT="stop_1.dat"/>
<node CREATED="1382221016457" ID="ID_864747169" MODIFIED="1382221097419" TEXT="stop_2.dat"/>
</node>
<node CREATED="1382220727952" ID="ID_74595952" MODIFIED="1382221251555" TEXT="Core.zip">
<node CREATED="1382221434323" ID="ID_770187651" MODIFIED="1382221444712" TEXT="routes.dat"/>
<node CREATED="1382221445147" ID="ID_1407561651" MODIFIED="1382221452161" TEXT="trips.dat"/>
<node CREATED="1382221461472" ID="ID_1928561997" MODIFIED="1382221465364" TEXT="stops.dat"/>
<node CREATED="1382225163584" ID="ID_612250714" MODIFIED="1382225167260" TEXT="headsigns.dat"/>
</node>
</node>
</node>
<node CREATED="1382216824940" ID="ID_1273358849" MODIFIED="1382216831567" TEXT="Gr&#xe1;f Wrapper"/>
</node>
</node>
<node CREATED="1382203259989" ID="ID_257191092" MODIFIED="1382203638025" POSITION="right" TEXT="TEKE">
<node CREATED="1382203797246" ID="ID_396957685" MODIFIED="1382216786287" TEXT="Terv K&#xe9;sz&#xed;t&#x151;">
<font BOLD="true" NAME="SansSerif" SIZE="12"/>
<node CREATED="1382204334946" ID="ID_854729262" MODIFIED="1382216794854" TEXT="&#xc1;llapott&#xe1;rol&#xe1;s">
<node CREATED="1382204247582" ID="ID_1883854451" MODIFIED="1382204334188" TEXT="A gr&#xe1;f statikus csom&#xf3;pontjai a meg&#xe1;ll&#xf3;k"/>
<node CREATED="1382204377924" ID="ID_391916896" MODIFIED="1382204457962" TEXT="Legyen egy list&#xe1;nk, mely kulcsai a statikus csp-ok">
<node CREATED="1382204518750" ID="ID_626977199" MODIFIED="1382215531042" TEXT="Benne set-ben a dinamikus csom&#xf3;pontok">
<node CREATED="1382204852081" ID="ID_1759330803" MODIFIED="1382204869226" TEXT="T&#xe1;rolja a history-t">
<node CREATED="1382204869876" ID="ID_1513339633" MODIFIED="1382206641518" TEXT="usedRoutes : set">
<node CREATED="1382206756387" ID="ID_1635606595" MODIFIED="1382206792970">
<richcontent TYPE="NODE"><html>
  <head>
    
  </head>
  <body>
    <p>
      Csak az&#233;rt kell, hogy ne sz&#225;ll&#237;tson vissza egy m&#225;r
    </p>
    <p>
      kor&#225;bban haszn&#225;lt route-ra az algoritmus
    </p>
  </body>
</html></richcontent>
<cloud/>
</node>
</node>
<node CREATED="1382208637351" ID="ID_1692105499" MODIFIED="1382208644430" TEXT="lastUsedRoute">
<node CREATED="1382208645537" ID="ID_1235925550" MODIFIED="1382208700167">
<richcontent TYPE="NODE"><html>
  <head>
    
  </head>
  <body>
    <p>
      Az&#233;rt kell, mert ha v&#233;letlen egy gar&#225;zsmenetre sz&#225;ll&#237;tana fel az &#250;tvonaltervez&#337;,
    </p>
    <p>
      akkor vissza lehessen sz&#225;llni
    </p>
  </body>
</html></richcontent>
<cloud/>
</node>
</node>
<node CREATED="1382204926690" ID="ID_849333511" MODIFIED="1382204941490" TEXT="actions : list">
<node CREATED="1382205184285" ID="ID_1759487180" MODIFIED="1382205191087" TEXT="Action">
<node CREATED="1382205194390" ID="ID_1227747947" MODIFIED="1382205196843" TEXT="startTime"/>
<node CREATED="1382205197269" ID="ID_795521355" MODIFIED="1382205291391" TEXT="endTime"/>
<node CREATED="1382205312186" ID="ID_1931230343" MODIFIED="1382205329175" TEXT="T&#xe1;rolja a stop azonos&#xed;t&#xf3;j&#xe1;t"/>
<node CREATED="1382205495079" ID="ID_1100434832" MODIFIED="1382206625589" TEXT="T&#xe1;rolja a trip azonos&#xed;t&#xf3;j&#xe1;t"/>
<node CREATED="1382205040477" ID="ID_285777966" MODIFIED="1382205059191" TEXT="T&#xe1;rolja a route azonos&#xed;t&#xf3;j&#xe1;t"/>
</node>
<node CREATED="1382204942241" ID="ID_211886085" MODIFIED="1382204952182" TEXT="GetOnAction">
<node CREATED="1382205203761" ID="ID_1535246483" MODIFIED="1382205208100" TEXT="1 perc k&#xf6;lts&#xe9;g"/>
</node>
<node CREATED="1382204953047" ID="ID_1201501196" MODIFIED="1382204961093" TEXT="TravelAction">
<node CREATED="1382205225823" ID="ID_1617841751" MODIFIED="1382205383163" TEXT="Melyik StopTime-b&#xf3;l"/>
<node CREATED="1382205268282" ID="ID_685112115" MODIFIED="1382205391890" TEXT="Melyik StopTime-ba"/>
</node>
<node CREATED="1382204961517" ID="ID_1768276587" MODIFIED="1382204964924" TEXT="GetOffAction">
<node CREATED="1382205082387" ID="ID_767496261" MODIFIED="1382205086766" TEXT="1 perc k&#xf6;lts&#xe9;g"/>
</node>
</node>
</node>
<node CREATED="1382205864574" ID="ID_306861337" MODIFIED="1382206016717" TEXT="onlyTravelActionNextTime">
<node CREATED="1382206018560" ID="ID_79412874" MODIFIED="1382206148896">
<richcontent TYPE="NODE"><html>
  <head>
    
  </head>
  <body>
    <p>
      Ha beker&#252;l&#233;skor ez a csom&#243;pont rosszabb aj&#225;nlat volt,
    </p>
    <p>
      mint a m&#225;r bent l&#233;v&#337;k, akkor nem sz&#225;llhatunk le itt.
    </p>
  </body>
</html></richcontent>
<cloud/>
</node>
</node>
<node CREATED="1382205820816" ID="ID_809101439" MODIFIED="1382205849903" TEXT="currentTime"/>
<node CREATED="1382205430629" ID="ID_429860255" MODIFIED="1382208171323" TEXT="nextPossibleTravelActions : list">
<node CREATED="1382205604485" ID="ID_1670654720" MODIFIED="1382205619224" TEXT="null, ha az utols&#xf3; action GetOffAction"/>
<node CREATED="1382205637908" ID="ID_872311825" MODIFIED="1382205691809" TEXT="Egy&#xe9;bk&#xe9;nt TravelAction lista az aktu&#xe1;lis trip k&#xf6;vetkez&#x151; meg&#xe1;ll&#xf3;ival"/>
<node CREATED="1382205715004" ID="ID_647213315" MODIFIED="1382205731847" TEXT="&#xdc;res lista, ha nincs k&#xf6;vetkez&#x151; meg&#xe1;ll&#xf3;"/>
</node>
<node CREATED="1382204988406" ID="ID_697317303" MODIFIED="1382206865269" TEXT="T&#xe1;rolja a stop azonos&#xed;t&#xf3;j&#xe1;t (melyik statikus csp-hoz tartozik)"/>
<node CREATED="1382206882605" ID="ID_435841401" MODIFIED="1382206893304" TEXT="M&#x171;veletek">
<node CREATED="1382206893839" ID="ID_647210148" MODIFIED="1382207687884" TEXT="CreateNextDynamicNodes()">
<node CREATED="1382206961049" ID="ID_477686749" MODIFIED="1382207003040" TEXT="A history figyelembev&#xe9;tel&#xe9;vel"/>
<node CREATED="1382207010714" ID="ID_1103642166" MODIFIED="1382524258261" TEXT="Milyen lehet&#x151;s&#xe9;gek vannak &#xfa;j stat node-ra l&#xe9;pni?">
<node CREATED="1382208248357" ID="ID_1702780616" MODIFIED="1382208253752" TEXT="Tov&#xe1;bbutaz&#xe1;s"/>
<node CREATED="1382208254203" ID="ID_374832647" MODIFIED="1382208998376" TEXT="Gyalogl&#xe1;s (lesz&#xe1;ll&#xe1;s + s&#xe9;ta a k&#xf6;zeli meg&#xe1;ll&#xf3;k egyik&#xe9;be)">
<node CREATED="1382207746342" ID="ID_608159998" MODIFIED="1382208175490" TEXT="possibleTransfers : list">
<node CREATED="1382207881121" ID="ID_719554357" MODIFIED="1382207885505" TEXT="Biztos nem null"/>
<node CREATED="1382207888136" ID="ID_1846310376" MODIFIED="1382207925115" TEXT="actions = actions + GetOffAction"/>
<node CREATED="1382207928561" ID="ID_919958786" MODIFIED="1382207957973" TEXT="stop azonos&#xed;t&#xf3;ja az 50 legk&#xf6;zelebbi egyike"/>
<node CREATED="1382208016213" ID="ID_1987050881" MODIFIED="1382208064499" TEXT="currentTime = GetOffAction.endTime + gyalogl&#xe1;si sebess&#xe9;gnek megfelel&#x151;"/>
</node>
<node CREATED="1382216354681" ID="ID_440555985" MODIFIED="1382216365660" TEXT="De csak akkor kell sz&#xe1;molni, ha nincs tiltva"/>
</node>
<node CREATED="1382515330312" ID="ID_1946994333" MODIFIED="1382515518789" TEXT="Felsz&#xe1;ll&#xe1;s egy j&#xe1;ratra"/>
<node CREATED="1382208281581" ID="ID_544054299" MODIFIED="1382524220844" TEXT="M&#xe1;sik route-ra &#xe1;tsz&#xe1;ll&#xe1;s">
<icon BUILTIN="messagebox_warning"/>
<node CREATED="1382208324742" ID="ID_420582466" MODIFIED="1382208333865" TEXT="Mi van, ha a k&#xf6;vetkez&#x151; az gar&#xe1;zsmenet?">
<node CREATED="1382208453158" ID="ID_1028229139" MODIFIED="1382208461345" TEXT="Szerencs&#xe9;tlen nem tud tov&#xe1;bb utazni..."/>
<node CREATED="1382208462065" ID="ID_430842398" MODIFIED="1382208469368" TEXT="Ha v&#xe1;rna, akkor viszont igen..."/>
<node CREATED="1382208601487" ID="ID_459567476" MODIFIED="1382208617484" TEXT="Meg kellene engedni, hogy az utolj&#xe1;ra haszn&#xe1;ltra visszasz&#xe1;llhasson..."/>
</node>
<node CREATED="1382216215135" ID="ID_938550596" MODIFIED="1382216225819" TEXT="Csak m&#xe9;g nem haszn&#xe1;ltakkal sz&#xe1;molva"/>
</node>
<node CREATED="1382208708613" ID="ID_473230973" MODIFIED="1382208718607" TEXT="Az utolj&#xe1;ra haszn&#xe1;lt route-ra visszasz&#xe1;ll&#xe1;s">
<node CREATED="1382208760607" ID="ID_605494413" MODIFIED="1382208840966" TEXT="Csak akkor, ha a meg&#xe1;ll&#xf3;k sz&#xe1;ma k&#xfc;l&#xf6;nb&#xf6;zik"/>
<node CREATED="1382210334034" ID="ID_1620027485" MODIFIED="1382210348352" TEXT="Csak akkor sz&#xe1;m&#xed;tjuk, ha nem lehet tov&#xe1;bb utazni"/>
<node CREATED="1382209071428" ID="ID_913518120" MODIFIED="1382209077131" TEXT="DirectionID azonos"/>
<node CREATED="1382208860596" ID="ID_1934799737" MODIFIED="1382208870811" TEXT="Lehet esetleg headsign vizsg&#xe1;lat helyette"/>
</node>
</node>
<node CREATED="1382209411653" ID="ID_625429762" MODIFIED="1382476817765" TEXT="&#xfc;res lista, ha csak tov&#xe1;bbutaz&#xe1;s lehets&#xe9;ges, de nem lehet tov&#xe1;bbutazni"/>
<node CREATED="1382209993570" ID="ID_565761227" MODIFIED="1382216379344" TEXT="Ha egyszer m&#xe1;r kisz&#xe1;molta, mentse el, legk&#xf6;zelebb ne sz&#xe1;molja ki">
<icon BUILTIN="messagebox_warning"/>
</node>
</node>
<node CREATED="1382214873800" ID="ID_13120064" MODIFIED="1382214943453" TEXT="GetMetaData() : NodeMetaData">
<node CREATED="1382214954937" ID="ID_1362901296" MODIFIED="1382214964049" TEXT="A*-n&#xe1;l">
<node CREATED="1382214969212" ID="ID_1536164062" MODIFIED="1382214974690" TEXT="g value"/>
<node CREATED="1382214981209" ID="ID_1236688156" MODIFIED="1382214983788" TEXT="h value"/>
<node CREATED="1382214975308" ID="ID_1159720872" MODIFIED="1382214978393" TEXT="f value"/>
</node>
<node CREATED="1382214964657" ID="ID_1508104454" MODIFIED="1382214967885" TEXT="Dijkstra-n&#xe1;l">
<node CREATED="1382214996578" ID="ID_608947968" MODIFIED="1382215105933" TEXT="c value"/>
<node CREATED="1382215109034" ID="ID_1324551009" MODIFIED="1382215117156" TEXT="v value"/>
</node>
</node>
<node CREATED="1382215148322" ID="ID_1294748804" MODIFIED="1382215182796" TEXT="SetComparator() : NodeComparator">
<node CREATED="1382215202980" ID="ID_21046107" MODIFIED="1382215213811" TEXT="priority queue-hoz kell"/>
<node CREATED="1382215224009" ID="ID_499470024" MODIFIED="1382215229282" TEXT="&#xf6;sszehasonl&#xed;t&#xe1;shoz kell"/>
</node>
</node>
</node>
</node>
<node CREATED="1382204544116" ID="ID_462841863" MODIFIED="1382204571830" TEXT="Ha &#xfa;jra el&#x151;ker&#xfc;l egy m&#xe1;r l&#xe1;tott stat csp">
<node CREATED="1382204575832" ID="ID_396843775" MODIFIED="1382204628912" TEXT="Ha a statikus csp elemein&#xe9;l jobb">
<node CREATED="1382204609938" ID="ID_382178469" MODIFIED="1382204640412" TEXT="Hozz&#xe1;adjuk a list&#xe1;hoz &#xe9;s ez &#xfa;j din csp lesz"/>
<node CREATED="1382204821008" ID="ID_653494269" MODIFIED="1382204828071" TEXT="M&#xe1;s lesz a history"/>
</node>
<node CREATED="1382204643952" ID="ID_1623850010" MODIFIED="1382204656119" TEXT="Ha rosszabb v ugyanolyan j&#xf3;">
<node CREATED="1382204657561" ID="ID_756709176" MODIFIED="1382204671000" TEXT="Felt&#xe9;telesen hozz&#xe1;adjuk din csp-k&#xe9;nt"/>
<node CREATED="1382204671315" ID="ID_1893992004" MODIFIED="1382204684416" TEXT="Tov&#xe1;bbl&#xe9;p&#xe9;s csak trip folytat&#xe1;sak&#xe9;nt"/>
</node>
<node CREATED="1382204762519" ID="ID_1183479582" MODIFIED="1382204797976" TEXT="Nem fog exp n&#x151;ni a din gr&#xe1;f"/>
</node>
</node>
</node>
</node>
<node CREATED="1382203295859" ID="ID_287017643" MODIFIED="1382203708110" POSITION="left" TEXT="FESZIT">
<node CREATED="1382203829566" ID="ID_1215234520" MODIFIED="1382204370479" TEXT="Felhaszn&#xe1;l&#xf3;i &#xe9;s Szolg&#xe1;ltat&#xe1;s Interf&#xe9;sz">
<font BOLD="true" NAME="SansSerif" SIZE="12"/>
</node>
</node>
</node>
</map>
