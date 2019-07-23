using System;
using System.Collections.Generic;
using System.Text;
using System.Query;
using System.Data.DLinq;

using EeekSoft.Expressions;

namespace EeekSoft.Query
{
	/// <summary>
	/// Extension methods for IQueryable
	/// </summary>
	public static class TableExtensions
	{
		/// <summary>
		/// Returns wrapper that automatically expands expressions in LINQ queries
		/// </summary>
		public static IQueryable<T> ToExpandable<T>(this IQueryable<T> q)
		{
			return new ExpandableWrapper<T>(q);
		}
	}


	/// <summary>
	/// Wrapper for IQueryable that calls Expand
	/// </summary>
	internal class ExpandableWrapper<T> : IQueryable<T>
	{
		IQueryable<T> _item;

		public ExpandableWrapper(IQueryable<T> item)
		{
			_item = item;
		}

		#region IQueryable<T> Members

		public IQueryable<S> CreateQuery<S>(System.Expressions.Expression expression)
		{
			return _item.CreateQuery<S>(expression.Expand());
		}

		public S Execute<S>(System.Expressions.Expression expression)
		{
			return _item.Execute<S>(expression.Expand());
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _item.GetEnumerator();
		}

		#endregion

		#region IQueryable Members

		public IQueryable CreateQuery(System.Expressions.Expression expression)
		{
			return _item.CreateQuery(expression);
		}

		public Type ElementType
		{
			get { return _item.ElementType; }
		}

		public object Execute(System.Expressions.Expression expression)
		{
			return _item.Execute(expression.Expand());
		}

		public System.Expressions.Expression Expression
		{
			get { return _item.Expression; }
		}

		#endregion

		#region IEnumerable Members
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _item.GetEnumerator();
		}

		#endregion
	}
}
