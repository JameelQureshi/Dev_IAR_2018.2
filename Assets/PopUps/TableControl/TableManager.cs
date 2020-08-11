using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

using LitJson;
using SLS.Widgets.Table;

public class TableManager : MonoBehaviour
{
    public Tabel_Description tabel_Description;
    public Selections selections;
    public Data data;

    public string m_ServerURL = "https://dl.dropboxusercontent.com/s/ju6lci5zjdpom06/TableData_update.json";
    public string m_JsonText;
    public Table table;
    public Sprite iconUp;
    public Sprite iconDown;
    private Dictionary<string, Sprite> spriteDict;
    private List<string> spriteNames;

    [Header("Header Settings")]
    public int m_HeaderFontSize = 20;
    public TextAlignment m_HeaderAlignment = TextAlignment.Center;
    public int m_HeaderHeight = 50;

    [Header("Cell Settings")]
    public int m_CellFontSize = 20;
    public TextAlignment m_CellAlignment = TextAlignment.Left;
    public int m_CellHeight = 100;

    // Start is called before the first frame update



    public void MakeTable()
    {
        MakeDefaults.Set();
        StartCoroutine(ParseJson());
    }

    //void Start()
    //{
    //    MakeDefaults.Set();
    //    StartCoroutine(ParseJson());
    //}

    // Update is called once per frame
    void Update()
    {

    }

    #region JSON
    IEnumerator ParseJson()
    {
        string strUrl = m_ServerURL;
        string jsonText = m_JsonText;

        if (string.IsNullOrEmpty(jsonText))
        {
            WWW www = new WWW(strUrl);
            yield return www;
            if( www.error == null)
            {
                jsonText = www.text;
            }
        }

        JsonData json = JsonMapper.ToObject(jsonText);
        Debug.Log("<color=green> ################## jsonText </color>" + jsonText);

        //JsonData json = JsonMapper.ToObject(www.text);

        tabel_Description.ColumnSize = int.Parse(json["Tabel_Description"]["ColumnSize"].ToString());
        for (int i = 0; i < json["Tabel_Description"]["DataStructure"].Count; i++)
        {
            Tabel_DescriptionItem tabel_DescriptionItem = new Tabel_DescriptionItem();
            tabel_DescriptionItem.Name = json["Tabel_Description"]["DataStructure"][i]["Name"].ToString();
            tabel_DescriptionItem.Type = json["Tabel_Description"]["DataStructure"][i]["Type"].ToString().ToLower();
            tabel_Description.DataStructure.Add(tabel_DescriptionItem);
        }

        for (int i = 0; i < json["Column_Selections"]["SelectedColumns"].Count; i++)
        {
            SelectionsItem selectionsItem = new SelectionsItem();
            selectionsItem.Name = json["Column_Selections"]["SelectedColumns"][i]["Name"].ToString();
            selectionsItem.Title = json["Column_Selections"]["SelectedColumns"][i]["Title"].ToString();

            string m_temp = json["Column_Selections"]["SelectedColumns"][i]["Sortable"].ToString();
            if (m_temp.ToLower().Contains("true"))
            {
                selectionsItem.Sortable = true;
            }
            else
            {
                selectionsItem.Sortable = false;
            }

            m_temp = json["Column_Selections"]["SelectedColumns"][i]["Width"].ToString();
            if (m_temp.ToLower().Contains("auto"))
            {
                selectionsItem.Width = 0;
            }
            else
            {
                selectionsItem.Width = int.Parse(m_temp);
            }

            selections.SelectedColumns.Add(selectionsItem);
        }

        //data.Rows = int.Parse(json["Data"]["Rows"].ToString());
        data.Rows = json["Data"]["DataList"].Count;
        for (int i = 0; i < json["Data"]["DataList"].Count; i++)
        {
            DataItem dataItem = new DataItem();

            for (int j = 0; j < json["Data"]["DataList"][i]["GroupList"].Count; j++)
            {
                Group groupItem = new Group();
                groupItem.ColumnName = json["Data"]["DataList"][i]["GroupList"][j]["ColumnName"].ToString();
                groupItem.ColumnData = json["Data"]["DataList"][i]["GroupList"][j]["ColumnData"].ToString();
                dataItem.GroupList.Add(groupItem);
            }

            data.DataList.Add(dataItem);
        }

        GenerateTable();
    }
    #endregion

    #region Table
    public void GenerateTable()
    {
        // build our sprite cross-reference
        this.spriteDict = new Dictionary<string, Sprite>();
        this.spriteNames = new List<string>(this.spriteDict.Keys);

        this.spriteDict.Add("UP", this.iconUp);
        this.spriteDict.Add("DOWN", this.iconDown);

        table.ResetTable();

        table.minHeaderHeight = m_HeaderHeight;
        table.minRowHeight = m_CellHeight;
        table.defaultFontSize = m_HeaderFontSize;

        Column c;

        RectTransform rect = transform.GetComponent<RectTransform>();

        float m_Width = 0;
        int m_Auto = 0;
        float m_AutoWidth = 0;

        for (int i = 0; i < selections.SelectedColumns.Count; i++)
        {
            if(selections.SelectedColumns[i].Width == 0)
            {
                m_Auto++;
            }
            else
            {
                m_Width += rect.rect.width * (float)selections.SelectedColumns[i].Width / 100f;
            }
        }
        m_AutoWidth = (rect.rect.width - m_Width) / m_Auto;

        for (int i = 0; i < selections.SelectedColumns.Count; i++)
        {
            if (selections.SelectedColumns[i].Width == 0)
            {
                c = table.AddTextColumn(selections.SelectedColumns[i].Title, null, m_AutoWidth, m_AutoWidth);
            }
            else
            {
                c = table.AddTextColumn(selections.SelectedColumns[i].Title, null, 
                    rect.rect.width * (float)selections.SelectedColumns[i].Width / 100f,
                    rect.rect.width * (float)selections.SelectedColumns[i].Width / 100f);
            }
            

            switch(m_HeaderAlignment)
            {
                case TextAlignment.Left:
                    c.horAlignment = Column.HorAlignment.LEFT;
                    break;
                case TextAlignment.Center:
                    c.horAlignment = Column.HorAlignment.CENTER;
                    break;
                case TextAlignment.Right:
                    c.horAlignment = Column.HorAlignment.RIGHT;
                    break;
            }
           
            if (selections.SelectedColumns[i].Sortable)
            {
                c.headerIcon = "UP";
            }

            for(int j = 0; j < tabel_Description.ColumnSize; j++)
            {
                if(selections.SelectedColumns[i].Name.Contains(tabel_Description.DataStructure[j].Name))
                {
                    switch(tabel_Description.DataStructure[j].Type)
                    {
                        case "String":
                            c.dataType = Column.DataType.TEXT;
                            break;
                        case "Number":
                            c.dataType = Column.DataType.NUMERIC;
                            break;
                        case "Image":
                            c.dataType = Column.DataType.IMAGE;
                            break;
                    }
                }
            }
        }

        table.defaultFontSize = m_CellFontSize;

        // Initialize Your Table
        this.table.Initialize(this.OnTableSelectedWithCol, this.spriteDict, true, this.OnHeaderClick);

        // Just activate our default sort
        this.OnHeaderClick(this.table.columns[0], null);

        // Draw Your Table
        this.table.StartRenderEngine();
    }

    private void OnInputFieldChange(Datum d, Column c, string oldVal, string newVal)
    {
        print("Change from " + oldVal + " to " + newVal);
    }

    private void OnTableSelectedWithCol(Datum datum, Column column)
    {
        if (datum == null) return;
        string cidx = "N/A";
        if (column != null)
            cidx = column.idx.ToString();
        print("You Clicked: " + datum.uid + " Column: " + cidx);
    }

    public void MoveSelection()
    {
        Element e = this.table.GetSelectedElement();
        if (e == null) return; // no selected cell
        this.table.MoveSelectionDown(false);
    }

    private void OnHeaderClick(Column column, PointerEventData e)
    {
        bool isAscending = false;
        // Reset current sort UI
        for (int i = 0; i < this.table.columns.Count; i++)
        {
            if (column == this.table.columns[i])
            {
                if (column.headerIcon == "UP")
                {
                    isAscending = true;
                    column.headerIcon = "DOWN";
                }
                else
                {
                    isAscending = false;
                    column.headerIcon = "UP";
                }
            }
            else
                this.table.columns[i].headerIcon = null;
        }

        for (int j = 0; j < selections.SelectedColumns.Count; j++)
        {
            int m_index = -1;
            for (int k = 0; k < data.DataList[0].GroupList.Count; k++)
            {
                if (selections.SelectedColumns[j].Name.Contains(data.DataList[0].GroupList[k].ColumnName))
                {
                    m_index = k;
                }
            }

            if (m_index == -1)
            {
            }
            else
            {
                if(column.idx == j)
                {
                    if (isAscending)
                    {
                        //data.DataList.OrderBy(x => x.GroupList[m_index].ColumnData).ToList();

                        for (int l1 = 0; l1 < data.DataList.Count; l1++)
                        {
                            for (int l2 = l1 + 1; l2 < data.DataList.Count; l2++)
                            {
                                if (data.DataList[l1].GroupList[m_index].ColumnData.CompareTo(data.DataList[l2].GroupList[m_index].ColumnData) > 0)
                                {
                                    DataItem tempdata = data.DataList[l1];
                                    data.DataList[l1] = data.DataList[l2];
                                    data.DataList[l2] = tempdata;
                                }
                            }
                        }
                    }
                    else
                    {
                        //data.DataList.OrderByDescending(x => x.GroupList[m_index].ColumnData).ToList();

                        for(int l1 = 0; l1 < data.DataList.Count; l1++)
                        {
                            for (int l2 = l1 + 1; l2 < data.DataList.Count; l2++)
                            {
                                if(data.DataList[l2].GroupList[m_index].ColumnData.CompareTo(data.DataList[l1].GroupList[m_index].ColumnData) > 0)
                                {
                                    DataItem tempdata = data.DataList[l1];
                                    data.DataList[l1] = data.DataList[l2];
                                    data.DataList[l2] = tempdata;
                                }
                            }
                        }
                    }
                }
            }
        }

        table.data.Clear();

        for (int i = 0; i < data.Rows; i++)
        {
            Datum d = Datum.Body(i.ToString());
           
            for (int j = 0; j < selections.SelectedColumns.Count; j++)
            {
                int m_index = -1;
                for (int k = 0; k < data.DataList[i].GroupList.Count; k++)
                {
                    if(selections.SelectedColumns[j].Name.Contains(data.DataList[i].GroupList[k].ColumnName))
                    {
                        m_index = k;
                    }
                }

                if(m_index == -1)
                {
                    d.elements.Add("");
                }
                else
                {
                    d.elements.Add(data.DataList[i].GroupList[m_index].ColumnData);
                    
                }
            }
            table.data.Add(d);
        }
        Invoke("AlignTable", 0.1f);
    }

    void AlignTable()
    {
        var textComponents = gameObject.GetComponentsInChildren<Text>(true);

        foreach (var component in textComponents)
        {
            if (component.gameObject.transform.parent.gameObject.GetComponent<HeaderCell>() == null)
            {
                switch (m_CellAlignment)
                {
                    case TextAlignment.Left:
                        component.gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        break;
                    case TextAlignment.Center:
                        component.gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        break;
                    case TextAlignment.Right:
                        component.gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                        break;
                }
            }
        }
    }
    #endregion
}

[System.Serializable]
public class TableReport
{
    public Tabel_Description Tabel_Description;
    public Selections Column_Selections;
    public Data Data;
    public TableReport(Tabel_Description desc, Selections selections, Data data)
    {
        this.Tabel_Description = desc;
        this.Column_Selections = selections;
        this.Data = data;
    }
}

[System.Serializable]
public class Tabel_Description
{
    public int ColumnSize;
    public List<Tabel_DescriptionItem> DataStructure;
    public Tabel_Description()
    {
        ColumnSize = 0;
        DataStructure = new List<Tabel_DescriptionItem>();
    }
}

[System.Serializable]
public class Tabel_DescriptionItem
{
    public string Name;
    public string Type;
    public Tabel_DescriptionItem()
    {
        Name = "";
        Type = "String";
    }
}

[System.Serializable]
public class Selections
{
    public List<SelectionsItem> SelectedColumns;
    public Selections()
    {
        SelectedColumns = new List<SelectionsItem>();
    }
}

[System.Serializable]
public class SelectionsItem
{
    public string Name;
    public string Title;
    public int Width;
    public bool Sortable;

    public SelectionsItem()
    {
        Name = "";
        Title = "";
        Sortable = true;
        Width = 0;
    }
}

[System.Serializable]
public class Data
{
    public int Rows;
    public List<DataItem> DataList;
    public Data()
    {
        Rows = 0;
        DataList = new List<DataItem>();
    }
}

[System.Serializable]
public class DataItem
{
    public List<Group> GroupList;
    public DataItem()
    {
        GroupList = new List<Group>();
    }
}

[System.Serializable]
public class Group{
    public string ColumnName;
    public string ColumnData;
    public Group()
    {
        ColumnName = "";
        ColumnData = "";
    }
}