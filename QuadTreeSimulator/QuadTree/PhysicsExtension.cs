
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LogicModel
{

    public partial struct Rect : IEquatable<Rect>, IFormattable
    {
        private float m_XMin;
        private float m_YMin;
        private float m_Width;
        private float m_Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(float x, float y, float width, float height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(UnityEngine.Vector2 position, UnityEngine.Vector2 size)
        {
            m_XMin = position.x;
            m_YMin = position.y;
            m_Width = size.x;
            m_Height = size.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(Rect source)
        {
            m_XMin = source.m_XMin;
            m_YMin = source.m_YMin;
            m_Width = source.m_Width;
            m_Height = source.m_Height;
        }

        static public Rect zero => new Rect(0.0f, 0.0f, 0.0f, 0.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
        {
            return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(float x, float y, float width, float height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        public float x
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_XMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_XMin = value; }
        }

        public float y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_YMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_YMin = value; }
        }

        public UnityEngine.Vector2 position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new UnityEngine.Vector2(m_XMin, m_YMin); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_XMin = value.x; m_YMin = value.y; }
        }

        public UnityEngine.Vector2 center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new UnityEngine.Vector2(x + m_Width / 2f, y + m_Height / 2f); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_XMin = value.x - m_Width / 2f; m_YMin = value.y - m_Height / 2f; }
        }

        public UnityEngine.Vector2 min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new UnityEngine.Vector2(xMin, yMin); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { xMin = value.x; yMin = value.y; }
        }

        public UnityEngine.Vector2 max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new UnityEngine.Vector2(xMax, yMax); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { xMax = value.x; yMax = value.y; }
        }

        public float width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_Width; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Width = value; }
        }

        public float height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_Height; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Height = value; }
        }

        public UnityEngine.Vector2 size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new UnityEngine.Vector2(m_Width, m_Height); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Width = value.x; m_Height = value.y; }
        }

        public float xMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_XMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { float oldxmax = xMax; m_XMin = value; m_Width = oldxmax - m_XMin; }
        }
        public float yMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_YMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { float oldymax = yMax; m_YMin = value; m_Height = oldymax - m_YMin; }
        }
        public float xMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_Width + m_XMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Width = value - m_XMin; }
        }
        public float yMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_Height + m_YMin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Height = value - m_YMin; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UnityEngine.Vector2 point)
        {
            return (point.x >= xMin) && (point.x < xMax) && (point.y >= yMin) && (point.y < yMax);
        }

        // Returns true if the /x/ and /y/ components of /point/ is a point inside this rectangle.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UnityEngine.Vector3 point)
        {
            return (point.x >= xMin) && (point.x < xMax) && (point.y >= yMin) && (point.y < yMax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(UnityEngine.Vector3 point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return Contains(point);
            }
            bool xAxis = width < 0f && (point.x <= xMin) && (point.x > xMax) ||
                width >= 0f && (point.x >= xMin) && (point.x < xMax);
            bool yAxis = height < 0f && (point.y <= yMin) && (point.y > yMax) ||
                height >= 0f && (point.y >= yMin) && (point.y < yMax);
            return xAxis && yAxis;
        }

        // Swaps min and max if min was greater than max.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Rect OrderMinMax(Rect rect)
        {
            if (rect.xMin > rect.xMax)
            {
                float temp = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = temp;
            }
            if (rect.yMin > rect.yMax)
            {
                float temp = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = temp;
            }
            return rect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(Rect other)
        {
            return (other.xMax > xMin &&
                other.xMin < xMax &&
                other.yMax > yMin &&
                other.yMin < yMax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(Rect other, bool allowInverse)
        {
            Rect self = this;
            if (allowInverse)
            {
                self = OrderMinMax(self);
                other = OrderMinMax(other);
            }
            return self.Overlaps(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Vector2 NormalizedToPoint(Rect rectangle, UnityEngine.Vector2 normalizedRectCoordinates)
        {
            return new UnityEngine.Vector2(
                PhysicsExtension.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x),
                PhysicsExtension.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Vector2 PointToNormalized(Rect rectangle, UnityEngine.Vector2 point)
        {
            return new UnityEngine.Vector2(
                PhysicsExtension.InverseLerp(rectangle.x, rectangle.xMax, point.x),
                PhysicsExtension.InverseLerp(rectangle.y, rectangle.yMax, point.y)
            );
        }

        // Returns true if the rectangles are different.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rect lhs, Rect rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Returns true if the rectangles are the same.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Rect lhs, Rect rhs)
        {
            // Returns false in the presence of NaN values.
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Rect)) return false;

            return Equals((Rect)other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Rect other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return $"(x:{x.ToString(format, formatProvider)}, y:{y.ToString(format, formatProvider)}, width:{width.ToString(format, formatProvider)}, height:{height.ToString(format, formatProvider)})";
        }

        [System.Obsolete("use xMin")]
        public float left { get { return m_XMin; } }
        [System.Obsolete("use xMax")]
        public float right { get { return m_XMin + m_Width; } }
        [System.Obsolete("use yMin")]
        public float top { get { return m_YMin; } }
        [System.Obsolete("use yMax")]
        public float bottom { get { return m_YMin + m_Height; } }
    }

    public static class PhysicsExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Vector3 ToVector3(this UnityEngine.Vector4 value)
        {
            return new UnityEngine.Vector3(value.x, value.y, value.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Vector4 ToVector4(this UnityEngine.Vector3 value)
        {
            return new UnityEngine.Vector4(value.x, value.y, value.z, 0.0f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Vector3 normalized(this UnityEngine.Vector3 value)
        {
            return UnityEngine.Vector3.Normalize(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float magnitude(this UnityEngine.Vector3 value)
        {
            return value.magnitude;
        }

        public static readonly float RotationEpsilon = 0.001f;
        public static readonly float DistanceEpsilon = 0.01f;
        public static readonly float TargetReachEpsilon = 0.2f;
        public static readonly float HeightCheckEpsilon = 5.0f;
        public static readonly float ClientSyncEpsilon = 3.0f;
        public static readonly float ServerStandEpsilon = 0.1f;
        public static readonly short MaxDegree = 360;
        public const float kEpsilonNormalSqrt = 1e-15F;
        public const float Deg2Rad = (float)Math.PI * 2F / 360F;
        public const float Rad2Deg = 1F / Deg2Rad;

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }
        public static float Clamp01(float value)
        {
            if (value < 0F)
                return 0F;
            else if (value > 1F)
                return 1F;
            else
                return value;
        }
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
                return Clamp01((value - a) / (b - a));
            else
                return 0.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(UnityEngine.Vector3 from, UnityEngine.Vector3 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(SqrMagnitude(from) * SqrMagnitude(to));
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Clamp(UnityEngine.Vector3.Dot(from, to) / denominator, -1F, 1F);
            return ((float)Math.Acos(dot)) * Rad2Deg;
        }

        /// <summary>
        /// 0,0,0 기준의 2 vector 의 degree
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle_DegBetween(this UnityEngine.Vector3 from, ref UnityEngine.Vector3 to, UnityEngine.Vector3 right)
        {
            float angle = Angle(from, to);
            return (Angle(right, to) > 90f) ? 360f - angle : angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(UnityEngine.Vector3 vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

        public static bool Contains(this Rect self, ref Rect rect)
        {
            //return self.Contains(rect.position) && self.Contains(rect.position + rect.size);
            return self.Contains(rect.min) && self.Contains(rect.max);
        }
        /// <summary>
        /// direction vector 를 degree 로 변환 x:0 z:1 이 0(360)degree 시계방향  증가 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float VectorToDeg(this UnityEngine.Vector3 from, ref UnityEngine.Vector3 to)
        {
            UnityEngine.Vector3 direction = to - from;
            float angle = MathF.Atan2(direction.x, direction.z) * Rad2Deg;
            if (angle < 0f) angle += 360f;

            return angle;
        }

        public static float DirectionToDeg(this UnityEngine.Vector3 direction)
        {
            float angle = MathF.Atan2(direction.x, direction.z) * Rad2Deg;
            if (angle < 0f) angle += 360f;

            return angle;
        }

        /// <summary>
        /// degree 를 direction normal vector 로 변환
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static UnityEngine.Vector3 DegToVector(this short direction)
        {
            float fColumn = MathF.Sin(direction * Deg2Rad);
            float fRow = MathF.Cos(direction * Deg2Rad);

            return new UnityEngine.Vector3(fColumn, 0, fRow);
        }

        /// <summary>
        /// degree 만큼 origin vector rotate (시계방향, 반시계방향으로는 -degree 만큼 회전이다.)
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static UnityEngine.Vector3 RotateVector2DClockwise(this UnityEngine.Vector3 origin, short direction)
        {
            float sinDirection = MathF.Sin(direction * Deg2Rad);
            float cosDirection = MathF.Cos(direction * Deg2Rad);

            return new UnityEngine.Vector3(cosDirection * origin.x + sinDirection * origin.z, 0, cosDirection * origin.z - sinDirection * origin.x);
        }


        /// <summary>
        /// from 부터 target 까지 direction 의 distance 만큼의 위치를 구함.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="moveDistance"></param>
        /// <param name="resultDirection"></param>
        /// <param name="forceMove"> target 의 위치를 넘어가면 강제로 moveDistance 까지 이동할지 여부</param>
        /// <returns></returns>
        public static UnityEngine.Vector3 GetFowardPositionFromStart(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos, float moveDistance, out float resultDirection, out float resultDistance, bool forceMove = false)
        {
            UnityEngine.Vector3 direction = targetPos - fromPos;
            float angleTo = direction.DirectionToDeg();
            float distance = direction.magnitude;
            // 두 벡터의 거리가 moveDistance보다 크거나 해당 거리까지 강제 이동일 경우
            if (distance > moveDistance || forceMove)
                resultDistance = moveDistance;
            else
                resultDistance = distance;

            var result = fromPos + (UnityEngine.Vector3.Normalize(direction) * resultDistance);
            //if( resultDistance < 5.0f)
            result.y = fromPos.y;
            //else
            //	result.y = targetPos.y;

                Console.WriteLine($"[GetFowardPositionFromStart] from {fromPos} target {targetPos} result {result}" +
                        $" degree {angleTo}  realMove {resultDistance}, distance {distance}");

            resultDirection = angleTo;

            return result;
        }

        public static UnityEngine.Vector3 GetDirectionalPositionFromStart(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 direction, float moveDistance, out float resultDirection)
        {
            resultDirection = direction.DirectionToDeg();

            var result = fromPos + (UnityEngine.Vector3.Normalize(direction) * moveDistance);
            result.y = fromPos.y;

            return result;
        }

        /// <summary>
        /// target 부터 from 까지 direction 의 distance 만큼의 위치를 구함. from 의 위치를 넘어가면 from 위치까지만
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="moveDistance"></param>
        /// <param name="resultDirection"></param>
        /// <returns></returns>
        public static UnityEngine.Vector3 GetFowardPositionFromTarget(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos, float moveDistance, out float resultDirection)
        {
            UnityEngine.Vector3 direction = fromPos - targetPos;
            float angleTo = targetPos.VectorToDeg(ref fromPos);

            float distance = direction.magnitude;

            // Forward Distance 가 from-target distance 보다 큰경우
            if (moveDistance >= distance)
            {
                moveDistance = distance;
            }

            // fromPos 와 같은 위치면 그냥 from Pos 까지..
            var result = targetPos + (UnityEngine.Vector3.Normalize(direction) * moveDistance);
            result.y = targetPos.y;

                Console.WriteLine($"[GetFowardPosition] target {targetPos} result {result} degree {angleTo}");
            resultDirection = angleTo;

            return result;
        }


        /// <summary>
        /// from 부터 target 까지 direction 으로 target 의 backward 위치를 구함
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="moveDistance"></param>
        /// <param name="resultDirection"></param>
        /// <returns></returns>
        /// 
        public static System.Random directionRand = new System.Random(Environment.TickCount);
        public static UnityEngine.Vector3 GetBackwardPosition(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos, float moveDistance, out float resultDirection, short substituteDirectionDeg = 0)
        {
            UnityEngine.Vector3 direction = targetPos - fromPos;
            direction.y = 0.0f;

            float angleTo = fromPos.VectorToDeg(ref targetPos);

            // 타겟과 같은 위치
            if (direction.magnitude < DistanceEpsilon)
            {
                substituteDirectionDeg = substituteDirectionDeg == 0 ? (short)directionRand.Next(0, 360) : substituteDirectionDeg;
                angleTo = substituteDirectionDeg;
                direction = substituteDirectionDeg.DegToVector();

                // 원래 타겟의 dir 보단 그냥 실패 처리가 나을듯...
                //resultDirection = 0.0f;				
                //return targetPos;
            }

            var result = targetPos + (UnityEngine.Vector3.Normalize(direction) * moveDistance);
            result.y = targetPos.y;

            Console.WriteLine($"[GetBackwardPosition] target {targetPos} result {result} degree {angleTo}");
            resultDirection = angleTo;

            return result;
        }

        public static float GetDistance(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos)
        {
            UnityEngine.Vector3 direction = targetPos - fromPos;

            if (Math.Abs(direction.y) < HeightCheckEpsilon)
            {
                direction.y = 0;
            }

            return direction.magnitude;
        }

        public static UnityEngine.Vector3 GetDirectionAndDistance(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos, out float distance)
        {
            UnityEngine.Vector3 direction = targetPos - fromPos;

            if (Math.Abs(direction.y) < HeightCheckEpsilon)
            {
                direction.y = 0;
            }

            distance = direction.magnitude;
            if (distance > DistanceEpsilon)
            {
                direction = UnityEngine.Vector3.Normalize(direction);
            }
            else
            {
                direction = UnityEngine.Vector3.forward;
                distance = 0.0f;
            }

            return direction;
        }

        // fromPos 위치에서 시전자가 targetPos 를 바라보고 있다고 가정함.
        // fromPos->targetPos Vector를 dgree 만큼 회전시킨 뒤, offset Vector를 바라보는 방향으로 돌려서 더해서 리턴. 
        public static UnityEngine.Vector3 GetRotateWithOffset(this UnityEngine.Vector3 fromPos, ref UnityEngine.Vector3 targetPos, float offsetX, float offsetY, float offsetZ, float degree)
        {
            UnityEngine.Vector3 forwardDir = targetPos - fromPos;
            if (forwardDir.magnitude < PhysicsExtension.DistanceEpsilon)
            {
                return fromPos;
            }

            UnityEngine.Vector3 rotatedDirection = forwardDir.RotateVector2DClockwise((short)degree);

            // target이 (0, 0, forwardDir.magnitude)일때의 Offset이 offsetX, offsetY, offsetZ;
            // target이 forwardDir 일때의 Offset을 구해야 한다.
            UnityEngine.Vector3 offset = UnityEngine.Vector3.zero;
            offset.x = offsetX;
            offset.y = offsetY;
            offset.z = offsetZ;
            UnityEngine.Vector3 rotatedOffset = offset.GetRotatedOffset(forwardDir);

            // 실제 해당 Remote 의 Target 지점
            return fromPos + rotatedOffset + rotatedDirection;
        }

        public static UnityEngine.Vector3 GetRotatedOffset(this UnityEngine.Vector3 tableOffset, UnityEngine.Vector3 facialVector)
        {
            float rotateDegreeinClockwise = facialVector.DirectionToDeg();

            return tableOffset.RotateVector2DClockwise((short)rotateDegreeinClockwise);
        }

        /// <summary>
        /// Cone 과 Point Intersection X,Z 를 이용한 2D 계산
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="ConeCenter">cone 의 중심 시작</param>
        /// <param name="ConeAxis">cone 의 direction (NORMAL)</param>
        /// <param name="Angle">cone 의 degree angle 1 ~ 180  (HALF)</param>
        /// <param name="Radius"> cone 의 radius</param>
        /// <returns></returns>
        public static bool IntersectConePoint2D(this UnityEngine.Vector3 Point,
                                       UnityEngine.Vector3 ConeCenter,
                                       UnityEngine.Vector3 ConeAxis,
                                       float Angle,
                                       float Radius)
        {
            Point.y = 0;
            ConeCenter.y = 0;
            ConeAxis.y = 0;

            UnityEngine.Vector3 LocalDirection = Point - ConeCenter;

            float sqrRadius = Radius * Radius;

            // 일단 길이 체크 - Cone Radius 
            if (LocalDirection.sqrMagnitude > sqrRadius)
            {
                return false;
            }

            UnityEngine.Vector3 Direction = UnityEngine.Vector3.Normalize(LocalDirection);
            float V = UnityEngine.Vector3.Dot(Direction, UnityEngine.Vector3.Normalize(ConeAxis));

            // V - Direction 과 ConeAxis 가 Normal 이기때문에  magnitude / 는 필요없음
            //float V2 = Vector3.Dot(LocalDirection, ConeAxis);
            //V2 = V2 / (LocalDirection.magnitude * ConeAxis.magnitude);

            float ConeAngleCos = (float)System.Math.Cos(Angle * Deg2Rad);

            if (V >= ConeAngleCos)
            {
                return true;
            }

            return false;
        }

        public static float GetSqrtDistance2D(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos)
        {
            targetPos.y = 0;
            fromPos.y = 0;

            UnityEngine.Vector3 direction = targetPos - fromPos;
            return direction.sqrMagnitude;
        }


        public static float GetDistance2D(this UnityEngine.Vector3 fromPos, UnityEngine.Vector3 targetPos)
        {
            targetPos.y = 0;
            fromPos.y = 0;

            UnityEngine.Vector3 direction = targetPos - fromPos;
            return direction.magnitude;
        }

        // 여기서 부턴 Client 에서 사용하는 함수...
        public static bool IntersectCylinderSphere2D(this UnityEngine.Vector3 Origin,
                                                    UnityEngine.Vector3 Start,
                                                  UnityEngine.Vector3 End,
                                                  float CylinderRadius,
                                                  float Radius)
        {
            Origin.y = 0;
            Start.y = 0;
            End.y = 0;

            float FixedRadius = Radius + CylinderRadius;
            float sqrFixedRadius = FixedRadius * FixedRadius;

            UnityEngine.Vector2 Start2D = new UnityEngine.Vector2(Start.x, Start.z);
            UnityEngine.Vector2 Origin2D = new UnityEngine.Vector2(Origin.x, Origin.z);

            UnityEngine.Vector2 EO = Start2D - Origin2D;
            if (EO.sqrMagnitude <= sqrFixedRadius)                                 // Start 에서 반지름이 CylinderRadius 인 원에 Target 의 몸체 일부분이라도 포함되면 true
            {
                return true;
            }

            UnityEngine.Vector3 DirVector = End - Start;
            UnityEngine.Vector2 DirVector2D = new UnityEngine.Vector2(DirVector.x, DirVector.z);

            float Length2D = DirVector2D.magnitude;
            if (Length2D < RotationEpsilon)
            {
                return false;                                                       // 여기엔 타겟의 위치에 대한 고려는 없다. Start->End 의 길이가 너무 짧으면 False. 
            }
            else
            {
                float V = UnityEngine.Vector2.Dot(DirVector2D / Length2D, -EO);                    // (DirVector2D / Length2D) => DirVector2D 방향으로의 유닛벡터 
                                                                                                // V = Vector3.Dot(DirVector2D의 유닛벡터, Start2D -> Origin 벡터)
                                                                                                //   = Start2D -> Origin 벡터의 (Start->End) 방향 성분의 길이. (Start->End 직선 위로 정사영 내린 것의 길이)
                float Disc = sqrFixedRadius - (UnityEngine.Vector2.Dot(EO, EO) - V * V);        // Disc = (CilinderRadius + Radius) ^2 - (Start2D -> Origin 벡터의 길이 ^2 - 정사영 길이 ^2)
                                                                                             //      = (CilinderRadius + Radius) ^2 - Origin 점으로부터 Start->End 직선까지의 높이의 제곱
                                                                                             //      => 
                if (Disc >= 0.0f)                                                   // 즉 CilinderRadius + Radius >= Origin 점에서 Start->End 직선까지의 직교거리(최단거리) 일 경우
                {
                    float Distance2D = (V - MathF.Sqrt(Disc));                      // Disc 의 의미는 Start 를 중심으로 하는 반지름 CylinderRadius 의 원에 Target을 Start->End 로부터의 높이를 유지하며 끌어당겨서 붙였을 때 
                                                                                    // Start -> End 방향의 성분의 길이.
                    float Time = Distance2D / Length2D;
                    if (Time >= 0.0f && Time <= 1.0f)                               // 캡슐 모양이 맞다.  (양쪽 다)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [System.Obsolete("3D 연산은 사용할 예정은 없다.")]
        public static bool IntersectConeSphere(this UnityEngine.Vector3 SphereCenter,
                                           float SphereRadius,
                                           UnityEngine.Vector3 ConeCenter,
                                           UnityEngine.Vector3 ConeAxis,
                                           float Angle,
                                           float Radius)
        {
            UnityEngine.Vector3 LocalCenter = SphereCenter - ConeCenter;

            if (LocalCenter.magnitude > SphereRadius + Radius)
            {
                return false;
            }

            float ConeAngleCos = (float)System.Math.Cos(Angle * Deg2Rad);
            float ConeAngleSin = (float)System.Math.Sin(Angle * Deg2Rad);
            UnityEngine.Vector3 U = ConeAxis * (-SphereRadius / ConeAngleSin);
            UnityEngine.Vector3 D = LocalCenter - U;
            float Dsqr = UnityEngine.Vector3.Dot(D, D);
            float E = UnityEngine.Vector3.Dot(ConeAxis, D);

            if (E > 0 && E * E >= Dsqr * (ConeAngleCos * ConeAngleCos))
            {
                Dsqr = UnityEngine.Vector3.Dot(LocalCenter, LocalCenter);
                E = UnityEngine.Vector3.Dot(-ConeAxis, LocalCenter);
                if (E > 0 && E * E >= Dsqr * (ConeAngleSin * ConeAngleSin))
                {
                    return Dsqr <= SphereRadius * SphereRadius;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }



    }
}
