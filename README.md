# Smart Priorities

![Workshop preview](package/preview.png)

[Install from Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3770104072)

Mod locale per Oxygen Not Included che aggiorna automaticamente le priorità
personali dei duplicanti in base al loro livello relativo per ogni categoria di
lavoro.

Per ciascuna categoria:

- chi ha il livello più alto riceve priorità molto alta;
- gli altri vengono distribuiti proporzionalmente tra alta, normale e bassa;
- i pari livello ricevono la stessa priorità;
- nessun duplicante idoneo scende sotto priorità bassa, quindi ogni categoria
  rimane sempre coperta anche all'inizio della colonia;
- i divieti imposti dai tratti del duplicante vengono rispettati.

Le categorie senza una competenza associata usano priorità fisse: `Supporto
vitale` è alta (4) per tutti e `Attivazione` è molto alta (5), così i compiti
essenziali e gli ordini manuali non restano in attesa.

La mod gestisce duplicanti organici e bionici. Le priorità vengono ricalcolate
ogni 60 secondi di gioco, quindi seguono la crescita degli attributi e l'arrivo
di nuovi duplicanti. I duplicanti morti o in fase di distruzione vengono
esclusi sia dal calcolo sia dalla colonna Smart. Le modifiche manuali nella
schermata Priorità vengono riallineate al calcolo automatico al
ribilanciamento successivo.

Nella schermata Priorità compare una colonna `Smart` con un interruttore per
ogni duplicante; l'interruttore nell'intestazione agisce su tutta la colonia.
Lo stesso controllo compare anche nel pannello laterale quando si seleziona un
duplicante direttamente. Il controllo automatico è attivo per tutti per
impostazione predefinita, ma può essere disattivato e riattivato
individualmente. La scelta viene salvata nella partita; quando è disattivato,
la mod lascia intatte le priorità personali di quel duplicante.

## Build

```sh
dotnet build -c Release
dotnet test -c Release
./install-local.sh
```

La build usa gli assembly della copia Steam locale di ONI. Per un percorso
diverso si può passare a MSBuild la proprietà `OniManagedDir`.
