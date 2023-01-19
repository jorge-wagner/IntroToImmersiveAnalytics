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
    * Scatterplot interativo gerado utilizando o IATK
    * Mapa interativo utilizando o Bing Maps SDK
    * Cubo espaço-temporal mostrando como georeferenciar dados
    * Calibração da posição da mesa utilizando Spatial Anchors


## Onde encontrar os exemplos

Os seguintes exemplos estão contidos neste projeto e podem ser explorados para o estudo das funcionalidades disponíveis:

1. Exemplos simplificados preparados por nós: `Assets/Scenes`
2. Exemplos do Oculus Interaction SDK: `Assets/Oculus/Interaction/Samples/Scenes/Examples`
3. Exemplos do MRTK: `Assets/MRTK/Samples`
4. Exemplos do Bing Maps SDK: `Assets/MapsSDK/Microsoft.Maps.Unity.Examples`
5. Exemplos do IATK: `Assets/IATK/Scenes`


   
