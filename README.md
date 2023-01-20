# Introdução ao desenvolvimento de visualizações imersivas utilizando Unity

Este é um projeto Unity base para a disciplina de **Projeto em Computação Gráfica: Visualização de Dados Imersiva** do Instituto de Informática da UFRGS.

Professores: Luciana Nedel, Carla Freitas, Jorge Wagner



## Objetivos

O objetivo deste material é auxiliar no desenvolvimento do projeto da disciplina de três formas:

1. Já fornecendo um projeto pré-configurado para VR
2. Já incluindo as seguintes dependências mais relevantes:
    * [Oculus Integration Package](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) v. 47.0
    * [Mixed Reality Toolkit (MRTK)](https://github.com/microsoft/MixedRealityToolkit-Unity) v. 2.8.2
    * [Immersive Analytics Toolkit (IATK)](https://github.com/MaximeCordeil/IATK) v 1.1
    * [Bing Maps SDK for Unity](https://github.com/microsoft/MapsSDK-Unity) v 0.11.2
      * **Atenção:** Quem desejar utilizar os recursos do Bing Maps deverá [obter uma *API key*](https://www.bingmapsportal.com) e incluí-la em um arquivo `Assets/Resources/MapSessionConfig.txt`

    * **Atenção:** O *copyright* das ferramentas acima pertencem aos seus autores. O propósito deste repositório é estritamente acadêmico.



3. Disponibilizando exemplos básicos de como criar visualizações interativas, a partir das seguintes cenas:
    * Interações básicas utilizando o MRTK
    * Interações básicas utilizando o Oculus Interaction SDK
    * Scatterplot interativo gerado utilizando GameObjects
       * Versão MRTK
       * Versão Oculus Interaction SDK
    * Mapa interativo utilizando o Bing Maps SDK (interação com MRTK)
    * Scatterplot interativo gerado utilizando o IATK (interação com MRTK)

    Ao longo do tempo novos exemplos serão incluídos: 

    * Cubo espaço-temporal mostrando como georeferenciar dados
    * Calibração da posição da mesa utilizando *Spatial Anchors*


  * **Atenção:** Notem que os exemplos que incluímos aqui são bastante simplificados, não incluindo seleções, filtros, visualizações coordenadas ou interações mais complexas. A intenção dos exemplos é fornecer um caminho inicial, e não servir como exemplo de um ambiente completo de visualização imersiva. 
  * **Atenção:** Os exemplos reproduzem o objeto *RoomEnvironment* como cenário de fundo. Este ambiente é de autoria do Oculus Integration Package. 

    

## Onde encontrar os exemplos

Os seguintes exemplos estão contidos neste projeto e podem ser explorados para o estudo das funcionalidades disponíveis:

1. Exemplos simplificados preparados por nós: `Assets/Scenes`
2. [Exemplos do Oculus Interaction SDK](https://developers.facebook.com/blog/post/2022/11/22/building-intuitive-interactions-vr/): `Assets/Oculus/Interaction/Samples/Scenes/Examples`
3. [Exemplos do Bing Maps SDK](https://github.com/microsoft/MapsSDK-Unity/wiki/Sample-project#example-scenes-for-mixed-reality-devices): `Assets/MapsSDK/Microsoft.Maps.Unity.Examples`
4. Exemplos do IATK: `Assets/IATK/Scenes`
5. [Exemplos do MRTK](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/example-scenes/example-hub?view=mrtkunity-2022-05): `Mixed Reality > Toolkit > Utilities > Import Examples from Package (UPM)`


   
