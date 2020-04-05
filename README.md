# Demo ONVIF .NET Core con client WCF
Questo progetto console .NET Core usa delle classi proxy generate a partire dai file WSDL della specifica ONVIF che sono elencati su https://www.onvif.org/profiles/specifications/

La specifica ONVIF è formata di vari documenti WSDL, uno per ciascun modulo. Non bisogna referenziarli tutti ma solo quelli necessari all'applicazione (ad esempio [Device](https://www.onvif.org/ver10/device/wsdl/devicemgmt.wsdl) e [PTZ](https://www.onvif.org/ver20/ptz/wsdl/ptz.wsdl)).

## Generazione delle classi proxy
In questo progetto demo, le classi proxy sono già state generate. Per chi è curioso di sapere come sono state generate, ecco le istruzioni.

Per prima cosa, installare il tool globale `svcutil` eseguendo questo comando:
```
dotnet tool install --global dotnet-svcutil
```

A questo punto eseguire il seguente comando dalla directory principale di questo progetto per generare le classi proxy. In questo caso viene usato il WSDL del modulo `Device` che consente di scoprire quali servizi sono offerti dalla telecamera. Il comando genererà il file [Proxies/Device.cs](Proxies/Device.cs) che contiene la classe [DeviceClient](Proxies/Device.cs#L16919).
```
dotnet svcutil https://www.onvif.org/ver10/device/wsdl/devicemgmt.wsdl --outputDir Proxies --outputFile Device.cs --namespace *,OnvifDemo.Proxies.Device
```

Aggiungere anche il modulo `Media` per ottenere i nomi dei profili esposti dalla telecamera. Il comando genererà il file [Proxies/Media.cs](Proxies/Media.cs) che contiene la classe [MediaClient](Proxies/Media.cs#L15538).

```
dotnet svcutil https://www.onvif.org/ver10/media/wsdl/media.wsdl --verbosity Silent --outputDir Proxies --outputFile Media.cs --namespace *,OnvifDemo.Proxies.Media
```

Infine, ecco il comando per aggiungere anche il proxy per il modulo `PTZ` che consente di spostare l'inquadratura della telecamera. Il comando genererà il file [Proxies/Ptz.cs](Proxies/Ptz.cs) che contiene la classe [PTZClient](Proxies/Ptz.cs#L12530).
```
dotnet svcutil https://www.onvif.org/ver20/ptz/wsdl/ptz.wsdl --outputDir Proxies --outputFile Ptz.cs --namespace *,OnvifDemo.Proxies.Ptz
```


## Utilizzo delle classi proxy
In questo progetto è installato il pacchetto NuGet `System.ServiceModel.Http` con il seguente comando:
```
dotnet add package System.ServiceModel.Http
```

Poi, vedere il codice la classe [Program](Program.cs) su come usare le varie classi `DeviceClient`, `MediaClient` e `PTZClient`. 

## Configurazione
Bisogna giusto modificare il device endpoint della propria telecamera dalla linea 14 del file [Program.cs](Program.cs#L13).

## Software che può tornare utile
 * Client ONVIF: [ONVIF Device Manager](https://sourceforge.net/projects/onvifdm/)
 * Server ONVIF [Happytime Multi ONVIF Server](http://www.happytimesoft.com/products/multi-onvif-server/index.html)