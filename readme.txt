Projektname:
    MLAPI Networking (Spielname: UsAmong)
Namen der Teammitglieder: 
    Christopher Meyer
    Fabian Weber

Besonderheiten: 
    Funktioniert unmodifiziert nur local (localhost), da wir, um ein Saubereres UI zu haben, das Setzen der Connection Adresse auskommentiert haben.
    Dies ist darin begründet, dass wir im Studentenwohnheim keine Möglichkeit haben, den Port des anderen zu erreichen
    (durch z.B. Portforwardung, static IP,...). Dies sollte jedoch kein Einfluss auf die Funktionalität haben.
    Eine Instanz gibt ein vorgegebenen Port als Host frei, in dem sich dieser dazu entscheidet das Spiel als Host zu starten.
    Alle anderen nehmen als Client teil.
    Steuerung (w,a,s,d):
        w: oben
        a: links
        s: unten
        d: rechts
    Zum Starten muss irgendeiner den Start Button drücken.
    Bedienung entweder über die Buttons oder Key Q zum töten (Imposter only) und Key R zum reporten.
 
Herausforderungen & Erfahrungen:

  Spawnen des Players:
    Unser Erste Herausforderung bestand schon damit ein Playerobject zu spawnen, nachdem wir uns in einer anderen Scene verbunden haben.
    Dies musste zuerst in der NetworkManager Component von MLAPI als 'Dafault Player Prefab' hinterlegt werden.
    Dann kann es der Server und NUR der Server Instanziieren und als Playerobject spawnen.
    Dabei war es auch wichtig die NetworkObject Component auf dem PlayerObject zu haben, ansonsten konnte dieser nicht richtig gespawnt werden.
    Hierfür braucht man noch zwingend die ClientID von dem jeweiligen client, um automatisch sicherzustellen, um wessen PlayerObject es sich handelt.
    Bzw. dass in den Controlls des Players die Variable 'IsLocalPlayer' passt.
    Des Weiteren mussten wir ausstellen, dass der Player automatisch von MLAPI gespawnt werden beim verbinden, sonst war der Player in der falschen Scene.
    Dies hatte erst nicht funktioniert, also mussten wir noch den 'ConnectionApprovalCallback' implementieren, bei dem wir sagen konnten das createPlayerObject false ist.
  
  ServerRPC & ClientRPC:
    Eins der wichtigsten Konzepte, was wir finden. Dabei hatten wir auch sehr viele Schwierigkeiten, welche viele viele Stunden an Zeit gekostet haben.
    Hierbei konnten wir lernen dass IMMER bei RPC die Methode mit 'ServerRPC' oder 'ClientRPC' enden MUSS. und auch ServerRPC nur vom Client aufgeruft werden sollte (kann) und ClientRPC nur vom Server.
    Zusätzlich muss man immer alles 10x überprüfen bei irgendwelchen Fehlern, ob nicht doch auf dem Client variablen benutzet werden, welche nur auf dem server verfügbar sind, oder umgekehrt.
    Bestimmte Methoden sollten auch nur vom Server aufgerufen werden, wenn da Methoden oder Vaiablen benutzt werden, welche Clientseitig nicht das machen was sie sollten.
    (Als Bsp: 'NetworkManager.Singleton.ConnectedClientsList'. Diese ist nur auf dem Server verfügbar)
  
  NetworkVariabeln und normale Variabeln:
    Anfangs hatten wir viele Variabeln als normale Variabeln, welche eigentlich NetworkVariabeln sein sollten. z.B. ob der Spieler lebt oder nicht.
    Auch hierbei waren viele Stunden Aufwand gefragt um Fehler solcher Art zu beheben. Denn jeder Spieler ist eine Instanz pro Client. NetworkVariabeln sind pro Spieler auf allen Clients,
    normale Variablen sind jedoch pro Spieler auf einem Client.
    Gegen Ende des Projektes fanden wir endlich auch etwas versteckt in der Dokumenation wie wir eigene Typen als NetworkVariabln erstellen können.
    Dies war an und für sich relativ einfach, aber hätte uns früher viele Fehlermeldungen erspart bzgl. dass der Type (z.B. GameObject) nicht als NetworkVariable erlaubt ist.
  
  Darstellung ob man nach Rechts oder nach Links schaut:
    Um dieses Thema zu bearbeiten wäre es einfach gewesen einfach die Skalierung zu flippen. Aber die localScale wird nicht mit der NetworkTransform Component syncronisiert, wie wir festgestellt haben.
    So haben wir dies mit Animations und dem Animator sowie dem NetworkAnimator realisiert.  
  
  Dokumentation und Tutorials:
    Eine der wesentlichen Herrausforderungen für uns war es zu wissen, wie benutzt ich jetzt x,y,z. Wie kann ich a nach b schicken?...
    Leider wurden uns diese Fragen sozusagen nur sehr langsam beantwortert. Die Offizielle Dokumenation half uns meist auch nur so weiter mit dem Namen welchen wir brauchen, bzw. benutzen können.
    Aber leider meißt ohne Erklärung wie das zu Benutzen ist. Auf Platformen wie YouTube sah es nicht besser aus, zu Projekt beginn gab es nur grundlegende Tutorials, die so auch von Unity bereitgestellt wurden und somit nicht weiterhalfen.
    Leider wurden dabei keine tieferen Konzepte verständlich rübergebracht.
    Durch viele breaking changes der MLAPI, halfen Forenbeiträge nur selten weiter, da in diesen angesprochene Probleme schon veraltet waren.
     Das zerrte teilweise an der Motivation und erschwerte die Umsetzung.
    

 Debugging:
    Zum Debuggen eines Spiels mussten wir meist mehrere Instanzen (2 - 4) dieses Spieles starten, um zu gewährleisten, dass das eingebaute Feature funktioniert.
    Da die Fehlerbehebung im Zusammenhang mit Netzwerkomponenten sich manchmal als durchaus schwierig gestaltet, konnte sich dies auch gern mal über Stunden ziehen. Erschwert hat dies manchmal noch die Aufteilung in Client und Host, da der Debug Log im Editor nur die Lognachrichten des dort gestarten Spiels anzeigt.
    Später fanden wir heraus, dass hierfür spiezelle Debug-Logs von MLAPI bereitgestellt werden. Dies hätte uns einiges an Arbeit ersparrt.
  
  Verhindern dass sich jemand verbindet, während einem Spiel:
    Um zu vermeiden, dass sich ein Client verbindet während eines laufendes Spieles, den 'approve' von dem 'ApprovalCheck' währenddessen auf false.
    Leider hatte dies nicht gereicht, da der Spieler bereits irgendwie Verbunden war, aber dann wieder getrennt wurde, und es gab keine Möglichkeit, welche wir finden wurde um den Verbindunsaufbau richtig abzuwarten.
    Als Workaround rufen wir beim Disconnect vom Spieler wieder das 'MainMenu' auf.  

 2D-Game:
    Das war das erste 2D-Game, welches wir zusammen entwickelt haben, deshalb mussten wir uns natürlich auch mit den, in diesem Zusammenhang auftretenden, Besonderheiten bei der Implementierung (2DCollider, 2D... ) und dem Aufbau (Tilemap, Bewegung, UI,...) auseinandersetzen. Dies nahm natürlich auch etwas Zeit in Anspruch. 

Inspiration:
    Among Us
    
Assets:
    MLAPI [Docs](https://docs-multiplayer.unity3d.com/docs/getting-started/about-mlapi) | [Github](https://github.com/Unity-Technologies/com.unity.multiplayer.mlapi)

Images & Icons:
    [Dungeon sheet tiles](https://opengameart.org/content/a-blocky-dungeon)
    [Teleporter](http://www.pngall.com/portal-png/download/33537) | modified.
    [Chat](https://pngtree.com/freepng/social-icon_4421694.html) | modified.
    [Megaphone](https://www.pngrepo.com/svg/128338/megaphone) | modified.
    [Skull](https://www.pngegg.com/de/png-nzauk) | modified.
    [Settings](https://de.pngtree.com/freepng/setting-up-the-app_4491056.html) | modified.

Characters created by [AnimaComedenti](https://github.com/AnimaComedenti)

Video:
https://youtu.be/zTpDqE61jhI

Projekt:
https://github.com/fabiastisch/UsAmong
