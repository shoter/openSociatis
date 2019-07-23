// ExpressionVisitor is part of System.Query namespace, but is marked as internal :-(
// so this implementation was extracted using Reflector. I did it just for demonstration
// how useful the class is (and why it should be public in final version).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Query;
using System.Expressions;

namespace EeekSoft.Expressions
{
	public abstract class ExpressionVisitor
	{
		internal ExpressionVisitor()
		{
		}

		internal static BinaryExpression MakeBinaryExpression(ExpressionType eType, Expression left, Expression right)
		{
			switch (eType)
			{
				case ExpressionType.Add:
					{
						return Expression.Add(left, right);
					}
				case ExpressionType.AddChecked:
					{
						return Expression.AddChecked(left, right);
					}
				case ExpressionType.And:
					{
						return Expression.And(left, right);
					}
				case ExpressionType.AndAlso:
					{
						return Expression.AndAlso(left, right);
					}
				case ExpressionType.BitwiseAnd:
					{
						return Expression.BitAnd(left, right);
					}
				case ExpressionType.BitwiseOr:
					{
						return Expression.BitOr(left, right);
					}
				case ExpressionType.BitwiseXor:
					{
						return Expression.BitXor(left, right);
					}
				case ExpressionType.Coalesce:
					{
						return Expression.Coalesce(left, right);
					}
				case ExpressionType.Divide:
					{
						return Expression.Divide(left, right);
					}
				case ExpressionType.EQ:
					{
						return Expression.EQ(left, right);
					}
				case ExpressionType.GT:
					{
						return Expression.GT(left, right);
					}
				case ExpressionType.GE:
					{
						return Expression.GT(left, right);
					}
				case ExpressionType.Index:
					{
						return Expression.Index(left, right);
					}
				case ExpressionType.LE:
					{
						return Expression.LE(left, right);
					}
				case ExpressionType.LShift:
					{
						return Expression.LShift(left, right);
					}
				case ExpressionType.LT:
					{
						return Expression.LT(left, right);
					}
				case ExpressionType.Modulo:
					{
						return Expression.Modulo(left, right);
					}
				case ExpressionType.Multiply:
					{
						return Expression.Multiply(left, right);
					}
				case ExpressionType.MultiplyChecked:
					{
						return Expression.MultiplyChecked(left, right);
					}
				case ExpressionType.NE:
					{
						return Expression.NE(left, right);
					}
				case ExpressionType.Or:
					{
						return Expression.Or(left, right);
					}
				case ExpressionType.OrElse:
					{
						return Expression.OrElse(left, right);
					}
				case ExpressionType.RShift:
					{
						return Expression.RShift(left, right);
					}
				case ExpressionType.Subtract:
					{
						return Expression.Subtract(left, right);
					}
				case ExpressionType.SubtractChecked:
					{
						return Expression.SubtractChecked(left, right);
					}
			}
			throw new ArgumentException("eType: " + eType);
		}

		internal static MemberExpression MakeMemberExpression(Expression expr, MemberInfo mi)
		{
			FieldInfo info3 = mi as FieldInfo;
			if (info3 != null)
			{
				return Expression.Field(expr, info3);
			}
			PropertyInfo info4 = mi as PropertyInfo;
			if (info4 == null)
			{
				throw new Exception("Member is not a Field or Property: " + mi);
			}
			return Expression.Property(expr, info4);
		}

		internal static MethodCallExpression MakeMethodCallExpression(ExpressionType eType, Expression obj, MethodInfo method, IEnumerable<Expression> args)
		{
			switch (eType)
			{
				case ExpressionType.MethodCall:
					{
						return Expression.Call(method, obj, args);
					}
				case ExpressionType.MethodCallVirtual:
					{
						return Expression.CallVirtual(method, obj, args);
					}
			}
			throw new ArgumentException("eType: " + eType);
		}

		internal static UnaryExpression MakeUnaryExpression(ExpressionType eType, Expression operand, Type type)
		{
			ExpressionType type1 = eType;
			if (type1 <= ExpressionType.ConvertChecked)
			{
				switch (type1)
				{
					case ExpressionType.As:
						{
							return Expression.As(operand, type);
						}
					case ExpressionType.BitwiseAnd:
						{
							goto Label_0093;
						}
					case ExpressionType.BitwiseNot:
						{
							return Expression.BitNot(operand);
						}
					case ExpressionType.Cast:
						{
							return Expression.Cast(operand, type);
						}
					case ExpressionType.Convert:
						{
							return Expression.Convert(operand, type);
						}
					case ExpressionType.ConvertChecked:
						{
							return Expression.ConvertChecked(operand, type);
						}
				}
			}
			else if (type1 <= ExpressionType.Negate)
			{
				if (type1 == ExpressionType.Len)
				{
					return Expression.Len(operand);
				}
				if (type1 == ExpressionType.Negate)
				{
					return Expression.Negate(operand);
				}
			}
			else if (type1 != ExpressionType.Not)
			{
				if (type1 == ExpressionType.Quote)
				{
					return Expression.Quote(operand);
				}
			}
			else
			{
				return Expression.Not(operand);
			}
		Label_0093:
			throw new ArgumentException("eType: " + eType);
		}

		internal virtual Expression Visit(Expression exp)
		{
			if (exp == null)
			{
				return exp;
			}
			switch (exp.NodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.BitwiseAnd:
				case ExpressionType.BitwiseOr:
				case ExpressionType.BitwiseXor:
				case ExpressionType.Coalesce:
				case ExpressionType.Divide:
				case ExpressionType.EQ:
				case ExpressionType.GT:
				case ExpressionType.GE:
				case ExpressionType.Index:
				case ExpressionType.LE:
				case ExpressionType.LShift:
				case ExpressionType.LT:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.NE:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.RShift:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					{
						return this.VisitBinary((BinaryExpression)exp);
					}
				case ExpressionType.As:
				case ExpressionType.BitwiseNot:
				case ExpressionType.Cast:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Len:
				case ExpressionType.Negate:
				case ExpressionType.Not:
				case ExpressionType.Quote:
					{
						return this.VisitUnary((UnaryExpression)exp);
					}
				case ExpressionType.Conditional:
					{
						return this.VisitConditional((ConditionalExpression)exp);
					}
				case ExpressionType.Constant:
					{
						return this.VisitConstant((ConstantExpression)exp);
					}
				case ExpressionType.Funclet:
					{
						return this.VisitFunclet((FuncletExpression)exp);
					}
				case ExpressionType.Invoke:
					{
						return this.VisitInvocation((InvocationExpression)exp);
					}
				case ExpressionType.Is:
					{
						return this.VisitTypeIs((TypeBinaryExpression)exp);
					}
				case ExpressionType.Lambda:
					{
						return this.VisitLambda((LambdaExpression)exp);
					}
				case ExpressionType.ListInit:
					{
						return this.VisitListInit((ListInitExpression)exp);
					}
				case ExpressionType.MemberAccess:
					{
						return this.VisitMemberAccess((MemberExpression)exp);
					}
				case ExpressionType.MemberInit:
					{
						return this.VisitMemberInit((MemberInitExpression)exp);
					}
				case ExpressionType.MethodCall:
				case ExpressionType.MethodCallVirtual:
					{
						return this.VisitMethodCall((MethodCallExpression)exp);
					}
				case ExpressionType.New:
					{
						return this.VisitNew((NewExpression)exp);
					}
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					{
						return this.VisitNewArray((NewArrayExpression)exp);
					}
				case ExpressionType.Parameter:
					{
						return this.VisitParameter((ParameterExpression)exp);
					}
			}
			throw new InvalidOperationException(string.Format("Unhandled Expression Type: {0}", exp.NodeType));
		}

		internal virtual Expression VisitBinary(BinaryExpression b)
		{
			Expression expression3 = this.Visit(b.Left);
			Expression expression4 = this.Visit(b.Right);
			if ((expression3 == b.Left) && (expression4 == b.Right))
			{
				return b;
			}
			return ExpressionVisitor.MakeBinaryExpression(b.NodeType, expression3, expression4);
		}

		internal virtual Binding VisitBinding(Binding binding)
		{
			switch (binding.BindingType)
			{
				case BindingType.MemberAssignment:
					{
						return this.VisitMemberAssignment((MemberAssignment)binding);
					}
				case BindingType.MemberMemberBinding:
					{
						return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
					}
				case BindingType.MemberListBinding:
					{
						return this.VisitMemberListBinding((MemberListBinding)binding);
					}
			}
			throw new InvalidOperationException(string.Format("Unhandled Binding Type: {0}", binding.BindingType));
		}

		internal virtual IEnumerable<Binding> VisitBindingList(ReadOnlyCollection<Binding> original)
		{
			List<Binding> list2 = null;
			int num1 = 0;
			int num2 = original.Count;
			while (num1 < num2)
			{
				Binding binding1 = this.VisitBinding(original[num1]);
				if (list2 != null)
				{
					list2.Add(binding1);
				}
				else if (binding1 != original[num1])
				{
					list2 = new List<Binding>(num2);
					for (int num3 = 0; num3 < num1; num3++)
					{
						list2.Add(original[num3]);
					}
					list2.Add(binding1);
				}
				num1++;
			}
			if (list2 != null)
			{
				return list2;
			}
			return original;
		}

		internal virtual Expression VisitConditional(ConditionalExpression c)
		{
			Expression expression4 = this.Visit(c.Test);
			Expression expression5 = this.Visit(c.IfTrue);
			Expression expression6 = this.Visit(c.IfFalse);
			if (((expression4 == c.Test) && (expression5 == c.IfTrue)) && (expression6 == c.IfFalse))
			{
				return c;
			}
			return Expression.Condition(expression4, expression5, expression6);
		}

		internal virtual Expression VisitConstant(ConstantExpression c)
		{
			return c;
		}

		internal virtual IEnumerable<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
		{
			List<Expression> list2 = null;
			int num1 = 0;
			int num2 = original.Count;
			while (num1 < num2)
			{
				Expression expression1 = this.Visit(original[num1]);
				if (list2 != null)
				{
					list2.Add(expression1);
				}
				else if (expression1 != original[num1])
				{
					list2 = new List<Expression>(num2);
					for (int num3 = 0; num3 < num1; num3++)
					{
						list2.Add(original[num3]);
					}
					list2.Add(expression1);
				}
				num1++;
			}
			if (list2 != null)
			{
				return list2;
			}
			return original;
		}

		internal virtual Expression VisitFunclet(FuncletExpression f)
		{
			return f;
		}

		internal virtual Expression VisitInvocation(InvocationExpression iv)
		{
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(iv.Args);
			Expression expression2 = this.Visit(iv.Expression);
			if ((enumerable2 != iv.Args) || (expression2 != iv.Expression))
			{
				Expression.Invoke((LambdaExpression)expression2, enumerable2);
			}
			return iv;
		}

		internal virtual Expression VisitLambda(LambdaExpression lambda)
		{
			Expression expression2 = this.Visit(lambda.Body);
			if (expression2 != lambda.Body)
			{
				return Expression.Lambda(lambda.Type, expression2, lambda.Parameters);
			}
			return lambda;
		}

		internal virtual Expression VisitListInit(ListInitExpression init)
		{
			NewExpression expression2 = this.VisitNew(init.NewExpression);
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(init.Expressions);
			if ((expression2 == init.NewExpression) && (enumerable2 == init.Expressions))
			{
				return init;
			}
			return Expression.ListInit(expression2, enumerable2);
		}

		internal virtual Expression VisitMemberAccess(MemberExpression m)
		{
			Expression expression2 = this.Visit(m.Expression);
			if (expression2 != m.Expression)
			{
				return ExpressionVisitor.MakeMemberExpression(expression2, m.Member);
			}
			return m;
		}

		internal virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			Expression expression2 = this.Visit(assignment.Expression);
			if (expression2 != assignment.Expression)
			{
				return Expression.Bind(assignment.Member, expression2);
			}
			return assignment;
		}

		internal virtual Expression VisitMemberInit(MemberInitExpression init)
		{
			NewExpression expression2 = this.VisitNew(init.NewExpression);
			IEnumerable<Binding> enumerable2 = this.VisitBindingList(init.Bindings);
			if ((expression2 == init.NewExpression) && (enumerable2 == init.Bindings))
			{
				return init;
			}
			return Expression.MemberInit(expression2, enumerable2);
		}

		internal virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(binding.Expressions);
			if (enumerable2 != binding.Expressions)
			{
				return Expression.ListBind(binding.Member, enumerable2);
			}
			return binding;
		}

		internal virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			IEnumerable<Binding> enumerable2 = this.VisitBindingList(binding.Bindings);
			if (enumerable2 != binding.Bindings)
			{
				return Expression.MemberBind(binding.Member, enumerable2);
			}
			return binding;
		}

		internal virtual Expression VisitMethodCall(MethodCallExpression m)
		{
			Expression expression2 = this.Visit(m.Object);
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(m.Parameters);
			if ((expression2 == m.Object) && (enumerable2 == m.Parameters))
			{
				return m;
			}
			return ExpressionVisitor.MakeMethodCallExpression(m.NodeType, expression2, m.Method, enumerable2);
		}

		internal virtual NewExpression VisitNew(NewExpression nex)
		{
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(nex.Args);
			if (enumerable2 != nex.Args)
			{
				return Expression.New(nex.Constructor, enumerable2);
			}
			return nex;
		}

		internal virtual Expression VisitNewArray(NewArrayExpression na)
		{
			IEnumerable<Expression> enumerable2 = this.VisitExpressionList(na.Expressions);
			if (enumerable2 == na.Expressions)
			{
				return na;
			}
			if (na.NodeType == ExpressionType.NewArrayInit)
			{
				return Expression.NewArrayInit(na.Type.GetElementType(), enumerable2);
			}
			return Expression.NewArrayBounds(na.Type.GetElementType(), enumerable2);
		}

		internal virtual Expression VisitParameter(ParameterExpression p)
		{
			return p;
		}

		internal virtual Expression VisitTypeIs(TypeBinaryExpression b)
		{
			Expression expression2 = b.Expression;
			if (expression2 != b.Expression)
			{
				return Expression.Is(expression2, b.TypeOperand);
			}
			return b;
		}

		internal virtual Expression VisitUnary(UnaryExpression u)
		{
			Expression expression2 = this.Visit(u.Operand);
			if (expression2 != u.Operand)
			{
				return ExpressionVisitor.MakeUnaryExpression(u.NodeType, expression2, u.Type);
			}
			return u;
		}

	}
}