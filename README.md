#Xamarin CRM

Xamarin CRM est une application de démonstration dont le but est d'aider le personnel d'une société fictive qui vend du matériel et des fournitures pour imprimante 3D. L'application permet aux vendeurs de suivre leurs performances de vente, gérer les prospects, les contacts, les commandes et de parcourir le catalogue produit.

![](Preview.png)

###Plateformes supportés : iOS et Android

###L'architecture de l'application est découpée en deux parties :
  1. Une application mobile Xamarin.Forms pour iOS et Android.
  2. Une Web Api.NET avec Entity Framework basée sur Azure Mobile Service pour la partie data.

**Il n'est pas nécessaire de déployé le service mobile Azure.** Une  instance de ce service est déjà déployé dans Azure. L'application mobile est configurée pour consommer ce service.

##Xamarin.Forms app (Xamarin CRM)

####Technologies utilisés
* [Xamarin.Forms](http://xamarin.com/forms)
* [Xamarin.Forms.Maps](https://developer.xamarin.com/guides/cross-platform/xamarin-forms/user-interface/map)
* [Active Directory Authentication Library (ADAL)](https://blog.xamarin.com/put-adal-xamarin.forms)
* [Azure Mobile Service libraries](https://azure.microsoft.com/en-us/documentation/services/mobile-services)
* [Syncfusion Essential Studio charts](http://www.syncfusion.com/products/xamarin)

####Points forts
######Partage de code supérieur à 95% entre les plateformes:
<img src="../../wiki/images/XamarinCRM_shared_code.png" alt="Over 95% shared code" width="75%">

######Controles natifs:
<img src="../../wiki/images/XamarinCRM_native_controls.png" alt="Natively rendered controls" width="50%">

######OAuth authentication avec Microsoft's ADAL (Active Directory Authentication Library):
<img src="../../wiki/images/XamarinCRM_authentication.png" alt="ADAL OAuth authentication" width="50%">

######Les graphiques ont été réalisé avec Syncfusion Essential Studio:
<img src="../../wiki/images/XamarinCRM_graphs.png" alt="Syncfusion charts" width="100%">

######Cartes natives pour chaque plateformes:
<img src="../../wiki/images/XamarinCRM_native_maps.png" alt="Native mapping" width="50%">

######Azure Mobile Service pour les données (.NET backend)
