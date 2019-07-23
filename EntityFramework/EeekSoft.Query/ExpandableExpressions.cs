using System;
using System.Collections.Generic;
using System.Text;
using System.Query;
using System.Xml.XLinq;
using System.Data.DLinq;
using System.Expressions;
using System.Reflection;

namespace EeekSoft.Expressions
{
	/// <summary>
	/// Contains extension methods for Expression
	/// </summary>
	public static class ExpressionExtensions
	{
		/// <summary>
		/// Invoke expression (compile & invoke)
		/// </summary>
		public static T Invoke<A0, T>(this Expression<Func<A0, T>> expr, A0 a0)
		{
			return expr.Compile().Invoke(a0);
		}

		/// <summary>
		/// Takes expr and replaces all calls to Invoke (extension) method by it's implementation 
		/// (modifies expression tree)
		/// </summary>
		public static Expression<Func<A0,T>> Expand<A0,T>(this Expression<Func<A0,T>> expr)
		{
			return (Expression<Func<A0,T>>)new ExpressionExpander().Visit(expr);
		}

		/// <summary>
		/// Takes expr and replaces all calls to Invoke (extension) method by it's implementation 
		/// (modifies expression tree)
		/// </summary>
		public static Expression Expand(this Expression expr)
		{
			return new ExpressionExpander().Visit(expr);
		}
	}


	/// <summary>
	/// Implementation of ExpressionVisiter that does the replacement
	/// </summary>
	internal class ExpressionExpander : ExpressionVisitor
	{
		Dictionary<ParameterExpression,Expression> _replaceVars;

		internal ExpressionExpander()
		{
			_replaceVars = null;
		}

		private ExpressionExpander(Dictionary<ParameterExpression,Expression> replaceVars)
		{
			_replaceVars = replaceVars;
		}

		internal override Expression VisitParameter(ParameterExpression p)
		{
			if ((_replaceVars != null) && (_replaceVars.ContainsKey(p)))
				return _replaceVars[p];
			else
				return base.VisitParameter(p);
		}

		internal override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof(ExpressionExtensions))
			{
				ConstantExpression c = (ConstantExpression)m.Parameters[0];
				LambdaExpression lambda = (LambdaExpression)c.Value;
				
				Dictionary<ParameterExpression,Expression> replaceVars
					= new Dictionary<ParameterExpression,Expression>();
				for(int i=0; i<lambda.Parameters.Count; i++)
					replaceVars.Add(lambda.Parameters[i], m.Parameters[i+1]);

				return new ExpressionExpander(replaceVars).Visit(lambda.Body);
			}
			return base.VisitMethodCall(m);
		}
	}
}