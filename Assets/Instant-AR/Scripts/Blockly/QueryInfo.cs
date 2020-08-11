using System;
public class QueryInfo
{
    string queryName;
	string selection;
	string queryNode;
	string condition;
	string a;
	string logicCompareList;
	string b;
    public string getQueryName() {
		return queryName;
	}
	public void setQueryName(string queryName) {
		this.queryName = queryName;
	}
	public string getSelection() {
		return selection;
	}
	public void setSelection(string selection) {
		this.selection = selection;
	}
	public string getQueryNode() {
		return queryNode;
	}
	public void setQueryNode(string queryNode) {
		this.queryNode = queryNode;
	}
	public string getCondition() {
		return condition;
	}
	public void setCondition(string condition) {
		this.condition = condition;
	}
	public string getA() {
		return a;
	}
	public void setA(string a) {
		this.a = a;
	}
	public string getLogicCompareList() {
		return logicCompareList;
	}
	public void setLogicCompareList(string logicCompareList) {
		this.logicCompareList = logicCompareList;
	}
	public string getB() {
		return b;
	}
	public void setB(string b) {
		this.b = b;
	}
    public void setQueryDetails(string qName, string sel, string qNode)
    {
		this.queryName = qName;
		this.selection = sel;
		this.queryNode = qNode;
    }
    public void setQueryCondition(string cond, string aSide, string logic, string bSide)
    {
		this.condition = cond;
		this.a = aSide;
		this.logicCompareList = logic;
		this.b = bSide;
    }
    public string createdId()
    {
		string id = "";
		id = (!string.IsNullOrEmpty(queryName)) ? id + queryName : id;
        id = (!string.IsNullOrEmpty(selection)) ? id + selection : id;
        id = (!string.IsNullOrEmpty(queryNode)) ? id + queryNode : id;
        id = (!string.IsNullOrEmpty(condition)) ? id + condition : id;
        id = (!string.IsNullOrEmpty(a)) ? id + a : id;
        id = (!string.IsNullOrEmpty(logicCompareList)) ? id + logicCompareList : id;
        id = (!string.IsNullOrEmpty(b)) ? id + b : id;
        return id;
    }
	
    public 	string toString()
    {
		string toString = "";
        toString = (!string.IsNullOrEmpty(queryName)) ? toString + "queryName : " + queryName : toString;
        toString = (!string.IsNullOrEmpty(selection)) ? toString + ", selection : " + selection : toString;
        toString = (!string.IsNullOrEmpty(queryNode)) ? toString + ", queryNode : " + queryNode : toString;
        toString = (!string.IsNullOrEmpty(condition)) ? toString + ", condition : " + condition : toString;
        toString = (!string.IsNullOrEmpty(a)) ? toString + ", A : " + a : toString;
        toString = (!string.IsNullOrEmpty(logicCompareList)) ?
                toString + ", logicCompareList : " + logicCompareList : toString;
        toString = (!string.IsNullOrEmpty(b)) ? toString + ", B : " + b : toString;
        return toString;
    }
    
}
