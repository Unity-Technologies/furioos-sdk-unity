using UnityEngine ;

/** Annotation personnalisée a ajouter sur les attributs et les propriétée que l'on veut voir afficher dans l'inspecteur.
 */
[System.AttributeUsage(System.AttributeTargets.Property)]
public class SerializeProperty : System.Attribute {}
