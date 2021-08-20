using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PersistentPeeperShoal
{
	public static class ShoalGeometry
	{
		// a is semi-major axis
		// b is semi-minor axis
		public static Vector3 Ellipse(Transform shoal, float a, float b, float angle)
		{
			// let x and z be in the ellipse slice
			float scale = PersistentPeeperShoalPatcher.Config.geoScale;
			float x = shoal.position.x + scale * a * Mathf.Cos(angle);
			float z = shoal.position.z + scale * b * Mathf.Sin(angle);

			return new Vector3(x, shoal.position.y, z);
		}

		public static Vector3 BoundVertical(Vector3 shoal, Vector3 dest)
        {
			float y = dest.y;
			if(y < shoal.y - PersistentPeeperShoalPatcher.Config.cycleHeight/2)
            {
				y = shoal.y - PersistentPeeperShoalPatcher.Config.cycleHeight / 2;
			}
			else if (y > shoal.y + PersistentPeeperShoalPatcher.Config.cycleHeight / 2)
			{
				y = shoal.y + PersistentPeeperShoalPatcher.Config.cycleHeight / 2;
			}
			dest.y = y;
			return dest;
		}
	}
}
