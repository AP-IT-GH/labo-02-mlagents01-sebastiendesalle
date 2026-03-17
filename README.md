# Technisch rapport implementatie van tweefasige reinforcement learning in Unity ML-Agents met raycast

### Inleiding
    
    Het doel van dit rapport is om te documenteren hoe een Machine Learning agent getraind wordt om een sequentiële taak uit te voeren (eerst een object nemen en dan naar een groene zone te gaan).
    Dit rapport is voor ontwikkelaars, lectoren of mensen die meer willen weten over ML-Agents in een Unity 3D omgeving.
    Het gaat grotendeels over de verschillen zien tussen tussen absolute coördinaten (gps stijl) en de sensor-gebaseerde observaties (raycasting).
### Methoden
##### Behaviour Parameters
	Omdat er maar één parameter wordt meegegeven via het script, moet de space size op 1 staan terwijl de rest van de sensoren (die je zelf moet schrijven zonder raycasting) door raycasting automatisch worden afgehandeld.
	Er zijn twee acties die gedefinieerd zijn, één voor de voorwaardse acties (z-as) en één voor de roterende acties (y-as).
##### Agent componenten
	De Ray Perception Sensor 3D zorgt ervoor dat we verschillende componenten kunnen aanpassen zoals: aantal rays, graden van rays en detecteerbare tags zoals "Target" en "Green Zone" kunnen ingeven. Zonder deze tags gaat de agent niet werken omdat het door deze methode geen andere parameters meekrijgt van het script.
	De Decision Requestor zorgt ervoor dat de agent keuzes kan nemen en dus acties kan ondernemen langs het neurale netwerk.
##### Override methodes van de agent
	OnEpisodeBegin(): Beschrijft dat hier de omgeving gereset wordt. De Target wordt op een willekeurige plek geplaatst, de status (targetCollected) wordt gereset en de agent wordt teruggeplaatst indien die van het platform is afgevallen.
	
	CollectObservations(): De enige reden dat deze methode bestaat is om de huidige status door te geven aan de agent als een float. Er wordt bekeken of targetCollected true of false is, als die true is dan krijgt die 1.0f mee en false 0.0f.
	
	OnActionReceived(): Hier gebeurt de translatie van vectorwaarden naar fysieke beweging (translate en rotate). Hier wordt een deel van het rewardsysteem meegegeven, als hij valt wordt SetReward op -1f gezet en wordt de episode gestopt. Hier wordt ook een systeem geïmplementeerd die ervoor zorgt dat de agent elke iteratie minder en minder stappen doet om het doel te bereiken door -1f / MaxStep te doen. Dit zorgt ervoor dat hoe minder stappen = hoe meer score. MaxStep komt van de Agent parent class.
	
	Heuristic(): Het enige doel van deze methode is om zelf te testen via toetsenbordinput om te zien of er nog aanpassingen moeten gedaan worden aan bv. rotatiesnelheid of snelheid.
	
	OnTriggerEnter(): Is een extra methode die niet van de Agent class komt, maar cruciaal is voor de logica van de verschillende fases (rode kubus en groene zone). Hier worden ook positieve beloningen uitgegeven.
### Resultaten
##### TensorBoard Data
	De Cumulative Reward grafiek werd het meeste gebruikt omdat het zeer duidelijk de stijging toont van de startwaarde `-0.8103` tot aan de stabiele eindronde `1.107` na ongeveer `100.000` stappen.
	
##### Episode Length
	toont aan hoeveel efficiënter dat de agent wordt, hoe lager de grafiek hoe sneller de agent de opdracht afmaakt.
	
	Na een bepaalde tijd kan de agent alle iteraties succesvol uitvoeren wat verwijst op een hoge mate van zekerheid in de training. Dit geeft aan dat het model stabiel is.
### Conclusie
	Uit de resultaten komt uit dat de combinatie van Ray Perception en een binaire statusvariabele voldoende informatie biedt aan het PPO-algoritme om een sequentiële, tweefasige taak op te lossen. Bij gevorderde taken gaat het zeker langer duren om dit model te trainen en slechtere resultaten opbrengen.
	
	Het toevoegen van een stappenalty is één van de belangrijkste toevoegingen voor efficiëntie en snelheid van het model. Het gebruik van visuele sensoren inplaats vann absolute coördinaten verhoogt de complexiteit van de training, maar zorgt voor een robuuster model dat onafhankelijk is van vaste coördinaten. Visuele sensoren zorgen ook voor gemak in het schrijven van het script.
### Referenties
    
    Unity Technologies (2022). _ML-Agents Documentation: Ray Perception Sensor_. Geraadpleegd via GitHub.
	(https://github.com/Unity-Technologies/ml-agents/tree/release_19_docs/docs)
	
	Introduction to Unity ML-agents (2023). Understand the Interplay of Neural Networks and Simulation Space Using the Unity ML-Agents Package. Geraadpleegd via PDF document.
