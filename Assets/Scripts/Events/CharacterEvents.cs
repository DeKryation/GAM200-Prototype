using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Events
{
    public class CharacterEvents
    {
        public static UnityAction<GameObject, int> characterDamaged; //SETHERE
        public static UnityAction<GameObject, int> characterHealed;  //SETHERE
    }
}
