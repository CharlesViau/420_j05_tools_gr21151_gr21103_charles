using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdaptable 
{
  void Init();
  void PostInit();
  void Refresh();
  void FixedRefresh();
}
