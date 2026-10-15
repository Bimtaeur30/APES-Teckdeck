using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._MemeberWorkspace.PCM.Scripts.Spawn.Interface
{
    public interface ISpawnable
    {
        public void OnSpawn(Vector3 pos);
    }
}
