using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public class MoveComponentTool
{
   private const string moveTopKey = "CONTEXT/Component/Move To Top";
   private const string moveBottomKey = "CONTEXT/Component/Move To Bottom";


   [MenuItem(moveTopKey, priority = 501)]
   public static void MoveComponentToTop(MenuCommand menuCommand)
   {
      while (UnityEditorInternal.ComponentUtility.MoveComponentUp((Component) menuCommand.context));
   }

   [MenuItem(moveTopKey, validate = true)]
   public static bool MoveComponentToTopValidate(MenuCommand menuCommand)
   {
      Component[] components = ((Component) menuCommand.context).gameObject.GetComponents<Component>();

      for (int i = 0; i < components.Length; i++)
      {
         if (components[i] == ((Component) menuCommand.context))
         {
            if (i== 1)
            {
               return false;
            }
         }
      }

      return true;

   }
   
   [MenuItem(moveBottomKey, priority = 501)]
   public static void MoveComponentToBottom(MenuCommand menuCommand)
   {
      while (UnityEditorInternal.ComponentUtility.MoveComponentDown((Component) menuCommand.context));
   }

   [MenuItem(moveBottomKey, validate = true)]
   public static bool MoveComponentToBottomValidate(MenuCommand menuCommand)
   {
      Component[] components = ((Component) menuCommand.context).gameObject.GetComponents<Component>();

      for (int i = 0; i < components.Length; i++)
      {
         if (components[i] == ((Component) menuCommand.context))
         {
            if (i== (components.Length-1))
            {
               return false;
            }
         }
      }

      return true;

   }
   
}

#endif
