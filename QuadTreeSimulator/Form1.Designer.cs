using GameCore.Utility.Pools;
using LogicModel;
using LogicModel.Areas;
using System.Numerics;

namespace QuadTreeSimulator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        /// 


        private class PlayInfo : IQuadStoreable
        {
            private static int indexer = 0;

            private long PlayIndex;
            private Rect QuadTreeBound = new Rect();

            public UnityEngine.Vector2 QuadTreeObjectSize;
            public UnityEngine.Vector2 Position;
            public UnityEngine.Vector2 Direction;
            public UnityEngine.Vector2 DestPosition;
            public bool IsMove = false;

            public PlayInfo()
            {
                PlayIndex = ++indexer;
            }

            public void SetPlayerRadius(float radius)
            {
                this.QuadTreeObjectSize = new UnityEngine.Vector2(radius * 2.0f, radius * 2.0f);
            }


            #region IQuadStoreable Implement
            public ref Rect Bound
            {
                get
                {
                    // 오브젝트 쿼드트리 시작 위치 (좌, 상단)
                    this.QuadTreeBound.x = Position.x - (QuadTreeObjectSize.x * 0.5f);
                    this.QuadTreeBound.y = Position.y - (QuadTreeObjectSize.y * 0.5f);

                    // 오브젝트 쿼드트리 크기
                    this.QuadTreeBound.size = QuadTreeObjectSize;

                    return ref this.QuadTreeBound;
                }
            }

            public long Index => PlayIndex;
            #endregion
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Form1";


            float offsetX = 10.0f; // 그리드 시작 X 위치
            float offsetY = 10.0f; // 그리드 시작 Y 위치

            gridCountX = 10; // 그리드 가로 갯수
            gridCountY = 10; // 그리드 새로 갯수

            gridWidth = 100.0f; // 그리드 가로 길이
            gridHeight = 100.0f; // 그리드 새로 길이

            this.ClientSize = new Size((int)(gridCountX * gridWidth + offsetX + 10), (int)(gridCountY * gridHeight + offsetY + 10));
            rectangleBounds = new Rectangle(new Point(0, 0), ClientSize);


            // 사각형 시작 위치
            var rectOffset = new UnityEngine.Vector2(offsetX, offsetY);

            // 사각형 크기
            var rectSize = new UnityEngine.Vector2(gridCountX * gridWidth, gridCountY * gridHeight);

            // 쿼드 트리 크기
            LogicModel.Rect rect = new LogicModel.Rect(
                                        position: rectOffset,
                                        size: rectSize);

            // 쿼드트리 루트 노드 생성
            quadTreeNode = new QuadTreeNode<PlayInfo>();
            quadTreeNode.OnCapture(null, rect);

            // 플레이 정보 삽입
            CreatePlayInfo(
                    count: 1000, 
                    positionRange: 1000, 
                    radiusRange: 15.0f
                );

            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(rectangleBounds.Width + 1, rectangleBounds.Height + 1);
            bufferedGraphics = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, rectangleBounds.Width, rectangleBounds.Height));

            // 마우스 휠 이벤트 (Zoom In, Zoom Out)
            MouseWheel += new MouseEventHandler(EventMouseWheel);

            // 오른쪽 마우스 스크롤 
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EventMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EventMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EventMouseUp);

            // 키 다운 (W, A, S, D) 쿼드트리 탐색 위치 이동
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EventKeyDown);

            // 업데이트 타이머
            updateTimer.Tick += new EventHandler(UpdateTick);
            updateTimer.Interval = 30;
            updateTimer.Start();
        }

        private void EventMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) 
            {
                if (IsRightMouseDown == false)
                {
                    MouseDownPos = MousePosition;
                    IsRightMouseDown = true;
                }
            }
        }

        private void EventMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (IsRightMouseDown == true)
                {
                    MouseDownPos = MousePosition;
                    IsRightMouseDown = false;
                }
            }
        }
        private void EventMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (IsRightMouseDown)
                {
                    var positionGap = new Point(MousePosition.X - MouseDownPos.X, MousePosition.Y - MouseDownPos.Y);

                    MouseWidthOffset += positionGap.X;
                    MouseHeightOffset += positionGap.Y;
                    MouseDownPos = MousePosition;
                }
            }
        }
        private DateTime BeforeTick = DateTime.Now;
        void EventKeyPress(object sender, KeyPressEventArgs e)
        {
            var moveAmount = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
            if (e.KeyChar == 'W'
                        || e.KeyChar == 'w')
            {
                moveAmount.y -= SearchRectMoveSpeed;
                e.Handled = true;
            }
            if (e.KeyChar == 'S'
                || e.KeyChar == 's')
            {
                moveAmount.y += SearchRectMoveSpeed;
                e.Handled = true;

            }
            if (e.KeyChar == 'A'
                || e.KeyChar == 'a')
            {
                moveAmount.x -= SearchRectMoveSpeed;
                e.Handled = true;

            }
            if (e.KeyChar == 'D'
                || e.KeyChar == 'd')
            {
                moveAmount.x += SearchRectMoveSpeed;
                e.Handled = true;
            }

            if (e.Handled == true)
            {
                var nowTick = DateTime.UtcNow;
                var diff = nowTick - this.BeforeTick;
                this.BeforeTick = nowTick;

                //moveAmount.x *= (diff.Microseconds / 1000_000.0f);
                //moveAmount.y *= (diff.Microseconds / 1000_000.0f);

                SearchRect.x += moveAmount.x;
                SearchRect.y += moveAmount.y;
            }

            if (e.KeyChar == 'Q' || e.KeyChar == 'q')
            {

                if (SearchRect.size.x - SearchRectSizeChangeRate <= 0 ||
                    SearchRect.size.y - SearchRectSizeChangeRate <= 0)
                {
                    return;
                }

                SearchRect.x += SearchRectSizeChangeRate * 0.5f;
                SearchRect.y += SearchRectSizeChangeRate * 0.5f;
                SearchRect.size = new UnityEngine.Vector2(
                    SearchRect.size.x - SearchRectSizeChangeRate, 
                    SearchRect.size.y - SearchRectSizeChangeRate);
            }


            if (e.KeyChar == 'E' || e.KeyChar == 'e')
            {

                if (SearchRect.size.x + SearchRectSizeChangeRate > gridCountX * gridWidth ||
                    SearchRect.size.y + SearchRectSizeChangeRate > gridCountY * gridHeight)
                {
                    return;
                }

                SearchRect.x -= SearchRectSizeChangeRate * 0.5f;
                SearchRect.y -= SearchRectSizeChangeRate * 0.5f;
                SearchRect.size = new UnityEngine.Vector2(
                    SearchRect.size.x + SearchRectSizeChangeRate,
                    SearchRect.size.y + SearchRectSizeChangeRate);
            }

        }

        void EventKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 'W'
                || e.KeyValue == 'w'
                || e.KeyValue == 'S'
                || e.KeyValue == 's'
                || e.KeyValue == 'A'
                || e.KeyValue == 'a'
                || e.KeyValue == 'D'
                || e.KeyValue == 'd')
            {
                this.BeforeTick = DateTime.UtcNow;
            }
        }

        void EventMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                // 마우스 휠을 위로 스크롤할 때
                // zoom in
                ZoomInOutScale *= 1.1f;
            }
            else
            {
                // 마우스 휠을 아래로 스크롤할 때
                // zoom out
                ZoomInOutScale *= 0.9f;
            }
        }

        private void CreatePlayInfo(int count, int positionRange, float radiusRange)
        {
            for (int i = 0; i < count; ++i)
            {
                var item = this.ObjectPool.Alloc();
                item.Data = new PlayInfo();
                this.PlayInfoDic.Add(item.Data.Index, item);

                while (true)
                {
                    item.Data.Position = GetRandomPosition(positionRange);
                    item.Data.SetPlayerRadius((float)((random.NextDouble() * radiusRange) + 2.0));
                    if (quadTreeNode.Insert(item) == true)
                    {
                        break;
                    }
                }
            }
        }

        private UnityEngine.Vector2 GetRandomPosition(int positionRange)
            => new UnityEngine.Vector2(random.Next(0, positionRange), random.Next(0, positionRange));

        // 랜덤 좌표로 이동
        private void MovePlayInfo()
        {
            foreach (var node in PlayInfoDic.Values)
            {
                var playInfo = node.Data; 
                if (playInfo == null)
                {
                    Console.Error.WriteLine($"playInfo is null.");
                    continue;
                }

                if (playInfo.IsMove == false)
                {
                    var nextPosition = GetRandomPosition(1000);
                    var direction = (nextPosition - playInfo.Position).normalized;
                    playInfo.IsMove = true;
                    playInfo.DestPosition = nextPosition;
                    playInfo.Direction = direction;
                }

                playInfo.Position += playInfo.Direction * 2.5f;

                if ((playInfo.DestPosition - playInfo.Position).magnitude < 5.0f)
                {
                    playInfo.IsMove = false;
                }

                this.quadTreeNode.Move(node);
            }
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            MovePlayInfo();

            Graphics g = bufferedGraphics.Graphics;
            g.Clear(Color.Black);
            drawQuadTree(g);
            drawQueryArea(g);

            bufferedGraphics.Render(Graphics.FromHwnd(this.Handle));
        }

        List<PlayInfo> SearchResult = new();
        List<PlayInfo> AllNodes = new();

        private void drawQueryArea(Graphics g)
        {
            AllNodes.Clear();
            SearchResult.Clear();

            // 검색 영역내 노드들 탐색
            quadTreeNode.Query(SearchRect, SearchResult);

            // 모든 노드들을 가져온다.
            quadTreeNode.GetAllContents(AllNodes);

            // 검색 영역 밖 노드들 
            var exNodes = AllNodes.Except(SearchResult);

            // 검색 영역 내 노드 렌더링
            foreach (var playInfo in SearchResult)
            {
                g.FillEllipse(SolidBlueBrush, new Rectangle(
                    (int)Math.Round(playInfo.Bound.xMin * ZoomInOutScale + MouseWidthOffset), 
                    (int)Math.Round(playInfo.Bound.yMin * ZoomInOutScale + MouseHeightOffset), 
                    (int)Math.Round(playInfo.Bound.size.x * ZoomInOutScale), 
                    (int)Math.Round(playInfo.Bound.size.y * ZoomInOutScale)));
            }

            // 검색 영역 밖 노드 렌더링
            foreach (var playInfo in exNodes)
            {
                g.FillEllipse(SolidRedBrush, new Rectangle(
                    (int)Math.Round(playInfo.Bound.xMin * ZoomInOutScale + MouseWidthOffset),
                    (int)Math.Round(playInfo.Bound.yMin * ZoomInOutScale + MouseHeightOffset),
                    (int)Math.Round(playInfo.Bound.size.x * ZoomInOutScale),
                    (int)Math.Round(playInfo.Bound.size.y * ZoomInOutScale)));
            }

            g.DrawRectangle(RedPen,
                    new Rectangle(
                        (int)Math.Round(SearchRect.xMin * ZoomInOutScale + MouseWidthOffset), 
                        (int)Math.Round(SearchRect.yMin * ZoomInOutScale + MouseHeightOffset), 
                        (int)Math.Round(SearchRect.size.x * ZoomInOutScale), 
                        (int)Math.Round(SearchRect.size.y * ZoomInOutScale)));
        }

        private void drawQuadTree(Graphics g)
        {
            this.quadTreeNode.ForEach(
                (quadTreeNode) =>
                {
                    var rect = quadTreeNode.Bounds;
                    g.DrawRectangle(VioletPen,
                        new Rectangle(
                            new Point(
                                (int)Math.Round(rect.xMin * ZoomInOutScale + MouseWidthOffset),
                                (int)Math.Round(rect.yMin * ZoomInOutScale + MouseHeightOffset)
                            ), 
                            new Size(
                                (int)Math.Round(rect.size.x * ZoomInOutScale), 
                                (int)Math.Round(rect.size.y * ZoomInOutScale)
                            )
                        )
                    );
                }
            );
        }

        private System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();

        QuadTreeNode<PlayInfo> quadTreeNode;

        int gridCountX; // 그리드 가로 갯수
        int gridCountY; // 그리드 새로 갯수

        float gridWidth; // 그리드 가로 길이
        float gridHeight; // 그리드 새로 길이

        private Rectangle rectangleBounds;
        private BufferedGraphics bufferedGraphics;
        private Pen VioletPen = new Pen(Color.BlueViolet, 1);
        private Brush SolidBlueBrush = Brushes.Blue;
        private Brush SolidRedBrush = Brushes.Red;
        private Pen RedPen = new Pen(Color.Red, 1);

        private Point MouseDownPos = new Point(0, 0);
        private bool IsRightMouseDown = false;
        private int MouseWidthOffset = 0;
        private int MouseHeightOffset = 0;


        private float ZoomInOutScale = 1.0f; // 줌인 줌아웃 스케일
        private Rect SearchRect = new Rect(100, 100, 300, 300); // 쿼드트리 탐색 범위
        //private float SearchRectMoveSpeed = 50000.0f;
        private float SearchRectSizeChangeRate = 10.0f;
        private float SearchRectMoveSpeed = 20.0f;

        private Dictionary<long, QuadTreeObject<PlayInfo>> PlayInfoDic = new();

        private LightPool<QuadTreeObject<PlayInfo>> ObjectPool = new LightPool<QuadTreeObject<PlayInfo>>
                (
                    () => { return new QuadTreeObject<PlayInfo>(); },
                    1024,
                    null,
                    null
                );


        private static Random random = new Random(Environment.TickCount);

        #endregion
    }
}