using GameCore.Utility.Pools;
using System.Collections.Generic;
using System.Windows;

namespace LogicModel.Areas
{
    public interface IQuadStoreable
	{
		ref Rect Bound { get; }
		long Index { get; }
	}

	public class QuadTreeObject<T> where T : IQuadStoreable
	{
		public T Data
		{
			get;
			set;
		}

		public QuadTreeNode<T> OwnNode
		{
			get;
			set;
		}

		// Insert Move 가 일어나는 순간의 Position.x
		public float x { get; set; }

		// Insert Move 가 일어나는 순간의 Position.z
		public float y { get; set; }

		public float radius { get; set; }
	}

	public class QuadTreeNode<T> where T : IQuadStoreable
	{
		//public const int _MAX_QUERY_COUNT = 64;
		public const int _MAX_QUERY_COUNT = 1000;

		private static LightPool<QuadTreeNode<T>> _nodePool = new LightPool<QuadTreeNode<T>>
		(
			() => { return new QuadTreeNode<T>(); },
			1024,
			null,
			null
		);

		public QuadTreeNode()
		{
			_parent = null;
		}

		public void OnCapture(QuadTreeNode<T> parents, Rect bounds)
		{
			_parent = parents;
			_bounds = bounds;

			if (this._nodes.Count != 0)
			{
				Console.Error.WriteLine($"QuadTreeNode - _nodes != 0");

				int nodeCount = _nodes.Count;
				for (int cnt = 0; cnt < nodeCount; cnt++)
				{
					QuadTreeNode<T> node = _nodes[cnt];

					node.Release();
					_nodePool.Free(node);
				}
			}

			if (this._contents.Count != 0)
			{
				Console.Error.WriteLine($"QuadTreeNode - _contents != 0");
				this._contents.Clear();
			}
		}

		public void Release()
		{
			int nodeCount = _nodes.Count;
			for (int cnt = 0; cnt < nodeCount; cnt++)
			{
				QuadTreeNode<T> node = _nodes[cnt];

				node.Release();
				_nodePool.Free(node);
			}

			_bounds.width = 0;
			_bounds.height = 0;

			_nodes.Clear();
			_contents.Clear();
		}

		private Rect _bounds;
		private QuadTreeNode<T> _parent = null;
		private HashSet<QuadTreeObject<T>> _contents = new HashSet<QuadTreeObject<T>>();
		private List<QuadTreeNode<T>> _nodes = new List<QuadTreeNode<T>>(4);

		public ref Rect Bounds => ref _bounds;
		public QuadTreeNode<T> Parents => _parent;

		public bool IsEmpty
		{
			get => (_bounds.width == 0 && _bounds.height == 0) || _nodes.Count == 0;
		}

		public bool IsEmptyLeaf
		{
			get => this._contents.Count == 0 && this._nodes.Count == 0;
		}

		public int ContentsCount
		{
			get
			{
				int count = 0;

				//foreach (QuadTreeNode<T> node in _nodes)
				//	count += node.Count;

				count += this._contents.Count;

				return count;
			}
		}

		public int Count
		{
			get
			{
				int count = 0;

				foreach (QuadTreeNode<T> node in _nodes)
					count += node.Count;

				count += this._contents.Count;

				return count;
			}
		}

		internal void GetAllContents(List<T> results)
		{
			foreach (QuadTreeObject<T> @object in _contents)
			{
				if (results.Count >= _MAX_QUERY_COUNT)
				{
					return;
				}

				results.Add(@object.Data);
			}

			if (_nodes.Count != 0)
			{
				int nodeCount = _nodes.Count;
				for (int cnt = 0; cnt < nodeCount; cnt++)
				{
					QuadTreeNode<T> node = _nodes[cnt];

					node.GetAllContents(results);
				}
			}
		}

		public void Query(Rect queryArea, List<T> results)
		{
#if QUADTREE_DEBUG
			Share.Log.Write($"Query queryArea { queryArea } on node.Bound { this.Bounds }");
#endif
			foreach (QuadTreeObject<T> item in this._contents)
			{
				if (results.Count >= _MAX_QUERY_COUNT)
				{
					return;
				}

				// 오브젝트의 영역이 검색 영역에 겹치는 경우
				if (queryArea.Overlaps(item.Data.Bound))
				{
					results.Add(item.Data);
				}
			}

			int nodeCount = _nodes.Count;
			for (int cnt = 0; cnt < nodeCount; cnt++)
			{
				QuadTreeNode<T> node = _nodes[cnt];

				if (node.IsEmptyLeaf)
					continue;

				//자식 노드에 검색 영역이 완전히 포함되어 있는 경우
				if (node.Bounds.Contains(ref queryArea))
				{
					node.Query(queryArea, results);
					break;
				}

				// 검색 영역에 자식노드가 완전히 포함되는 경우
				if (queryArea.Contains(ref node.Bounds))
				{
					// 해당 노드의 모든 컨텐츠를 가져온다.
					node.GetAllContents(results);
					continue;
				}
				// 자식 노드가 검색하는 Area 에 일부분만 겹치는 부분이 있는 경우
				if (node.Bounds.Overlaps(queryArea))
				{
					node.Query(queryArea, results);
				}
			}
		}

		public bool Insert(QuadTreeObject<T> item)
		{
			if (false == _bounds.Contains(ref item.Data.Bound))
			{
				Console.Error.WriteLine($"[QuadTree] Insert Failed { Bounds } not contains item { item.Data.Bound }");
				return false;
			}

			if (_nodes.Count == 0)
				createSubNodes();

			int nodeCount = _nodes.Count;
			for (int cnt = 0; cnt < nodeCount; cnt++)
			{
				QuadTreeNode<T> node = _nodes[cnt];
				if (node.Bounds.Contains(ref item.Data.Bound))
				{
					return node.Insert(item);
				}
			}

			// - 더 이상 해당 item 을 Contain 없는 node 가 없는경우
			// - 더 이상 나눌수 없는 작은 node 크기인경우
			item.OwnNode = this;
			if (false == this._contents.Add(item))
			{
                Console.Error.WriteLine($"[QuadTree] Insert Already in contents { item.Data.Index }");
			}
			else
			{
				Console.WriteLine($"[QuadTree] Insert in contents node Bounds { this.Bounds }");
			}

			return true;
		}

		private void remove(QuadTreeObject<T> item)
		{
			this._contents.Remove(item);
		}

		public void Delete(QuadTreeObject<T> item, bool clean)
		{
			if (item.OwnNode != null)
			{
				if (item.OwnNode == this)
				{
					//Share.Log.Write($"[QuadTree] Delete on - { this.Bounds }");
					remove(item);

					if (clean == true)
						cleanUpwards();
				}
				else
				{
					item.OwnNode.Delete(item, clean);
				}

				item.OwnNode = null;
			}
		}

		public void ForEach(System.Action<QuadTreeNode<T>> action)
		{
			action(this);

			int nodeCount = _nodes.Count;
			for (int cnt = 0; cnt < nodeCount; cnt++)
			{
				QuadTreeNode<T> node = _nodes[cnt];
				node.ForEach(action);
			}
		}

		private void createSubNodes()
		{
			if ((_bounds.height * _bounds.width) <= 16)
				return;

			float halfWidth = (_bounds.width / 2f);
			float halfHeight = (_bounds.height / 2f);

            UnityEngine.Vector2 size = new UnityEngine.Vector2(halfWidth, halfHeight);

			var childLT = _nodePool.Alloc();
			var childLB = _nodePool.Alloc();
			var childRT = _nodePool.Alloc();
			var childRB = _nodePool.Alloc();

			childLT.OnCapture(this, new Rect(_bounds.position, size));
			childLB.OnCapture(this, new Rect(new UnityEngine.Vector2(_bounds.xMin, _bounds.yMin + halfHeight), size));
			childRT.OnCapture(this, new Rect(new UnityEngine.Vector2(_bounds.xMin + halfWidth, _bounds.yMin), size));
			childRB.OnCapture(this, new Rect(new UnityEngine.Vector2(_bounds.xMin + halfWidth, _bounds.yMin + halfHeight), size));

			_nodes.Add(childLT);
			_nodes.Add(childLB);
			_nodes.Add(childRT);
			_nodes.Add(childRB);
		}

		private QuadTreeNode<T> getDestinationNode(QuadTreeObject<T> item)
		{
			QuadTreeNode<T> destNode = this;
			int nodeCount = _nodes.Count;
			for (int cnt = 0; cnt < nodeCount; cnt++)
			{
				QuadTreeNode<T> node = _nodes[cnt];
				if (true == node.Bounds.Contains(ref item.Data.Bound))
				{
					destNode = node;
					break;
				}
			}
			return destNode;
		}

		private bool relocate(QuadTreeObject<T> item)
		{
			if (_bounds.Contains(ref item.Data.Bound))
			{
				QuadTreeNode<T> dest = getDestinationNode(item);

				if (item.OwnNode != dest)
				{
					QuadTreeNode<T> prevOwner = item.OwnNode;
					this.Delete(item, false);

					if (dest.Insert(item) == false)
						return false;

					prevOwner?.cleanUpwards();
				}

				return true;
			}

			return this.Parents?.relocate(item) ?? false;
		}

		private void cleanUpwards()
		{
			if (_nodes.Count != 0 && ContentsCount == 0)
			{
				if (_nodes[0].IsEmptyLeaf &&
					_nodes[1].IsEmptyLeaf &&
					_nodes[2].IsEmptyLeaf &&
					_nodes[3].IsEmptyLeaf)
				{
					// release To Pool
					_nodePool.Free(_nodes[0]);
					_nodePool.Free(_nodes[1]);
					_nodePool.Free(_nodes[2]);
					_nodePool.Free(_nodes[3]);

					_nodes.Clear();

					this.Parents?.cleanUpwards();
				}
			}
			else
			{
				this.Parents?.cleanUpwards();
			}
		}

		public bool Move(QuadTreeObject<T> item)
		{
			return item.OwnNode?.relocate(item) ?? this.relocate(item);
		}
	}
}