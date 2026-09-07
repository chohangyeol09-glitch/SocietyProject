using System.Collections.Generic;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 충돌 콜백(OnCollisionEnter)은 Collider / Rigidbody 가 붙은 오브젝트에만 전달된다.
    /// 모듈을 하위 오브젝트에 따로 떼어놓는 구조에서도 충돌을 받을 수 있도록,
    /// BikeCollisionModule 이 런타임에 바이크 몸체(Rigidbody 오브젝트)에 이 중계 컴포넌트를 붙인다.
    /// 직접 씬에 추가할 필요는 없다.
    /// </summary>
    [DisallowMultipleComponent]
    public class BikeCollisionRelay : MonoBehaviour
    {
        private readonly List<BikeCollisionModule> _targets = new List<BikeCollisionModule>();

        /// <summary>충돌을 전달받을 모듈을 등록한다. 같은 모듈을 여러 번 등록해도 한 번만 들어간다.</summary>
        public void Register(BikeCollisionModule module)
        {
            if (module != null && !_targets.Contains(module))
                _targets.Add(module);
        }

        private void OnCollisionEnter(Collision collision)
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                if (_targets[i] == null)
                {
                    _targets.RemoveAt(i);
                    continue;
                }

                _targets[i].ReportCollision(collision);
            }
        }
    }
}
