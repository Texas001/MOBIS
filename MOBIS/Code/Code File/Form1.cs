using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace CA_COVID1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] FoodGate;
        string[] Obstatcle;
        string[] Seat;
        string[] Recycling;
        string[] Indoor;
        string[] Outdoor;
        string[] Infector;
        public static string FilePath;

        private void button1_Click(object sender, EventArgs e)
        {

            //总行数、总列数、元胞距离
            int rowDef = 24;
            int colDef = 80;
            double disDef = 5;
            double timestep = 0.5;
            double window_waiting_time = Convert.ToDouble(textBox5.Text);
            double seat_waiting_time = Convert.ToDouble(textBox6.Text);
            double recycle_waiting_time = Convert.ToDouble(textBox7.Text);
            double infection_rate_E = Convert.ToDouble(textBox2.Text);
            double infection_rate_I = Convert.ToDouble(textBox3.Text);
            double infection_threshold = Convert.ToDouble(textBox4.Text);
            int stage1 = Convert.ToInt32(textBox8.Text);
            int stage2 = Convert.ToInt32(textBox9.Text);
            int stage3 = Convert.ToInt32(textBox10.Text);

            //定义网格并初始化
            GridManager gridManager = new GridManager(rowDef, colDef, disDef);
            gridManager.createGrid();

            //所有网格对象字典集合
            Dictionary<string, CellClass> gridDic = gridManager.GridDic;

            /// <summary>
            /// 定义有人网格字典集合
            /// </summary>
            Dictionary<long, PersonCanteen> personDic = new Dictionary<long, PersonCanteen>();

            /// <summary>
            /// 定义目标窗口字典集合
            /// </summary>
            Dictionary<long, string > personDic_foodgate = new Dictionary<long, string >();

            //
            List<PersonCube> list = new List<PersonCube>();

            long pid = 1;
            double curTime = 2.1;
            string xyStr = "1_23";
            double Target = 2.1;
            double InfVar = 2.1;
            double Infectionp = 2.1;
            double stopping = 2.1;
            double time = 0.0;

            PersonCube pc = new PersonCube(pid, curTime, xyStr, Target, InfVar, Infectionp, stopping, time);


            //设置打饭窗口、障碍物、座位、回收点、入口和出口
            gridManager.setFoodGate( FoodGate );
            gridManager.setObstatcle( Obstatcle );
            gridManager.setSeat( Seat );
            gridManager.setRecycling( Recycling );
            gridManager.setIndoor( Indoor );
            gridManager.setOutdoor( Outdoor );

            double totalTime = 0.0;
            long personID = 0;
            long personID_copy = 0;
            long kk = 1;//用于控制目标窗口选哪个
            GridManager gridManager1 = null;

            int iterCount = Convert.ToInt32(textBox1.Text);

            //元胞循环-10次
            for (int i = 0; i < iterCount; i++)
            {

                //打饭窗口列表集合-用于选取窗口
                Dictionary<string, int> foodGateRankDic = new Dictionary<string, int>();

                //每个人下一步时间将更新的元胞字典集合
                Dictionary<long, string> personNextCellDic = new Dictionary<long, string>();
                Dictionary<string, CellClass> gridDic1 = gridManager.GridDic;

                totalTime += timestep;

                if (i == 0)
                {
                    #region 初始化新网格
                    //定义新网格并初始化
                    gridManager1 = new GridManager(rowDef, colDef, disDef);
                    gridManager1.createGrid();

                    //所有网格对象字典集合
                    gridDic1 = gridManager1.GridDic;

                    //设置打饭窗口、障碍物、回收点、入口和出口
                    gridManager1.setFoodGate(new string[] { "0_4","0_7","0_10","0_13","0_16","0_19","0_22","0_25","0_28","0_31","0_34","0_37","0_42","0_45","0_48",
                    "0_51","0_54","0_57","0_60","0_63","0_66","0_69","0_72","0_75"});
                    gridManager1.setObstatcle(new string[] {
                    "10_4","10_5","10_8","10_9","10_12","10_13","10_16","10_17","10_20","10_21","10_24","10_25","10_28","10_29","10_32","10_33","10_36","10_37",
                    "10_42","10_43","10_46","10_47","10_50","10_51","10_54","10_55","10_58","10_59","10_62","10_63","10_66","10_67","10_70","10_71","10_74","10_75",
                    "15_4","15_5","15_8","15_9","15_12","15_13","15_16","15_17","15_20","15_21","15_24","15_25","15_28","15_29","15_32","15_33","15_36","15_37",
                    "15_42","15_43","15_46","15_47","15_50","15_51","15_54","15_55","15_58","15_59","15_62","15_63","15_66","15_67","15_70","15_71","15_74","15_75",
                    "20_4","20_5","20_8","20_9","20_12","20_13","20_16","20_17","20_20","20_21","20_24","20_25","20_28","20_29","20_32","20_33","20_36","20_37",
                    "20_42","20_43","20_46","20_47","20_50","20_51","20_54","20_55","20_58","20_59","20_62","20_63","20_66","20_67","20_70","20_71","20_74","20_75"});
                    gridManager1.setSeat(new string[] {
                    "9_4","9_5","9_8","9_9","9_12","9_13","9_16","9_17","9_20","9_21","9_24","9_25","9_28","9_29","9_32","9_33","9_36","9_37",
                    "9_42","9_43","9_46","9_47","9_50","9_51","9_54","9_55","9_58","9_59","9_62","9_63","9_66","9_67","9_70","9_71","9_74","9_75",
                    "11_4","11_5","11_8","11_9","11_12","11_13","11_16","11_17","11_20","11_21","11_24","11_25","11_28","11_29","11_32","11_33","11_36","11_37",
                    "11_42","11_43","11_46","11_47","11_50","11_51","11_54","11_55","11_58","11_59","11_62","11_63","11_66","11_67","11_70","11_71","11_74","11_75",
                    "14_4","14_5","14_8","14_9","14_12","14_13","14_16","14_17","14_20","14_21","14_24","14_25","14_28","14_29","14_32","14_33","14_36","14_37",
                    "14_42","14_43","14_46","14_47","14_50","14_51","14_54","14_55","14_58","14_59","14_62","14_63","14_66","14_67","14_70","14_71","14_74","14_75",
                    "16_4","16_5","16_8","16_9","16_12","16_13","16_16","16_17","16_20","16_21","16_24","16_25","16_28","16_29","16_32","16_33","16_36","16_37",
                    "16_42","16_43","16_46","16_47","16_50","16_51","16_54","16_55","16_58","16_59","16_62","16_63","16_66","16_67","16_70","16_71","16_74","16_75",
                    "19_4","19_5","19_8","19_9","19_12","19_13","19_16","19_17","19_20","19_21","19_24","19_25","19_28","19_29","19_32","19_33","19_36","19_37",
                    "19_42","19_43","19_46","19_47","19_50","19_51","19_54","19_55","19_58","19_59","19_62","19_63","19_66","19_67","19_70","19_71","19_74","19_75",
                    "21_4","21_5","21_8","21_9","21_12","21_13","21_16","21_17","21_20","21_21","21_24","21_25","21_28","21_29","21_32","21_33","21_36","21_37",
                    "21_42","21_43","21_46","21_47","21_50","21_51","21_54","21_55","21_58","21_59","21_62","21_63","21_66","21_67","21_70","21_71","21_74","21_75"});
                    gridManager1.setRecycling(new string[] { "23_39", "23_40", "4_79", "5_79" });
                    gridManager1.setIndoor(new string[] { "23_0" });
                    gridManager1.setOutdoor(new string[] { "0_79", "1_79" });

                    #endregion
                }

                gridManager = gridManager1;
                gridDic = gridManager1.GridDic;

                if (i > 0)
                {
                    #region 初始化新网格
                    //定义新网格并初始化
                    gridManager1 = new GridManager(rowDef, colDef, disDef);
                    gridManager1.createGrid();

                    //所有网格对象字典集合
                    gridDic1 = gridManager1.GridDic;

                    //设置打饭窗口、障碍物、回收点、入口和出口
                    gridManager1.setFoodGate(new string[] { "0_4","0_7","0_10","0_13","0_16","0_19","0_22","0_25","0_28","0_31","0_34","0_37","0_42","0_45","0_48",
                    "0_51","0_54","0_57","0_60","0_63","0_66","0_69","0_72","0_75"});
                    gridManager1.setObstatcle(new string[] {
                    "10_4","10_5","10_8","10_9","10_12","10_13","10_16","10_17","10_20","10_21","10_24","10_25","10_28","10_29","10_32","10_33","10_36","10_37",
                    "10_42","10_43","10_46","10_47","10_50","10_51","10_54","10_55","10_58","10_59","10_62","10_63","10_66","10_67","10_70","10_71","10_74","10_75",
                    "15_4","15_5","15_8","15_9","15_12","15_13","15_16","15_17","15_20","15_21","15_24","15_25","15_28","15_29","15_32","15_33","15_36","15_37",
                    "15_42","15_43","15_46","15_47","15_50","15_51","15_54","15_55","15_58","15_59","15_62","15_63","15_66","15_67","15_70","15_71","15_74","15_75",
                    "20_4","20_5","20_8","20_9","20_12","20_13","20_16","20_17","20_20","20_21","20_24","20_25","20_28","20_29","20_32","20_33","20_36","20_37",
                    "20_42","20_43","20_46","20_47","20_50","20_51","20_54","20_55","20_58","20_59","20_62","20_63","20_66","20_67","20_70","20_71","20_74","20_75"});
                    gridManager1.setSeat(new string[] {
                    "9_4","9_5","9_8","9_9","9_12","9_13","9_16","9_17","9_20","9_21","9_24","9_25","9_28","9_29","9_32","9_33","9_36","9_37",
                    "9_42","9_43","9_46","9_47","9_50","9_51","9_54","9_55","9_58","9_59","9_62","9_63","9_66","9_67","9_70","9_71","9_74","9_75",
                    "11_4","11_5","11_8","11_9","11_12","11_13","11_16","11_17","11_20","11_21","11_24","11_25","11_28","11_29","11_32","11_33","11_36","11_37",
                    "11_42","11_43","11_46","11_47","11_50","11_51","11_54","11_55","11_58","11_59","11_62","11_63","11_66","11_67","11_70","11_71","11_74","11_75",
                    "14_4","14_5","14_8","14_9","14_12","14_13","14_16","14_17","14_20","14_21","14_24","14_25","14_28","14_29","14_32","14_33","14_36","14_37",
                    "14_42","14_43","14_46","14_47","14_50","14_51","14_54","14_55","14_58","14_59","14_62","14_63","14_66","14_67","14_70","14_71","14_74","14_75",
                    "16_4","16_5","16_8","16_9","16_12","16_13","16_16","16_17","16_20","16_21","16_24","16_25","16_28","16_29","16_32","16_33","16_36","16_37",
                    "16_42","16_43","16_46","16_47","16_50","16_51","16_54","16_55","16_58","16_59","16_62","16_63","16_66","16_67","16_70","16_71","16_74","16_75",
                    "19_4","19_5","19_8","19_9","19_12","19_13","19_16","19_17","19_20","19_21","19_24","19_25","19_28","19_29","19_32","19_33","19_36","19_37",
                    "19_42","19_43","19_46","19_47","19_50","19_51","19_54","19_55","19_58","19_59","19_62","19_63","19_66","19_67","19_70","19_71","19_74","19_75",
                    "21_4","21_5","21_8","21_9","21_12","21_13","21_16","21_17","21_20","21_21","21_24","21_25","21_28","21_29","21_32","21_33","21_36","21_37",
                    "21_42","21_43","21_46","21_47","21_50","21_51","21_54","21_55","21_58","21_59","21_62","21_63","21_66","21_67","21_70","21_71","21_74","21_75"});
                    gridManager1.setRecycling(new string[] { "23_39", "23_40", "4_79", "5_79" });
                    gridManager1.setIndoor(new string[] { "23_0" });
                    gridManager1.setOutdoor(new string[] { "0_79", "1_79" });

                    #endregion
                }

                //计算窗口队列人数, 获取最后一个没人的元胞坐标
                foreach (KeyValuePair<string, CellClass> kvp in gridManager.GridDic_foodGate)
                {
                    int foodGateRow = Convert.ToInt32(kvp.Key.Split('_')[0]);
                    int foodGateCol = Convert.ToInt32(kvp.Key.Split('_')[1]);
                    string key = "";

                    for (int ii = foodGateRow; ii < rowDef - 1; ii++)
                    {
                        string nextID = string.Format($"{ii}_{foodGateCol}");
                        int currentStatus = gridDic[nextID].StatusVar;
                        int currentFunction = gridDic[nextID].FuncVar;

                        if (currentStatus == 0 && currentFunction == 0)
                        {
                            if (gridDic[string.Format($"{ii + 1}_{foodGateCol}")].FuncVar == 2)
                            {
                                break;
                            }
                            key = string.Format($"{ii}_{foodGateCol}");
                            foodGateRankDic.Add(key, ii);

                            break;
                        }
                        if (currentFunction == 2)
                        {
                            break;
                        }
                    }
                }
                if (!foodGateRankDic.ContainsKey(string.Format("")))
                {
                    string key = string.Format($"{-1}_{-1}");
                    foodGateRankDic.Add(key, rowDef + 1);
                }

                string key0 = string.Format($"{-1}_{-1}");
                int c = 0;
                int cellOrder = -1;
                string MiniKey = "";
                double cellcoord_col;
                long kk_cp = 0;
                //判断当前时刻应选的食堂窗口
                foreach (KeyValuePair<string, int> kvp0 in foodGateRankDic)
                {
                    if (kvp0.Key != key0)
                    {
                        c = 1;
                    }
                }
                if (c == 0)
                {
                    MiniKey = "";
                }
                else
                {
                    long count2;
                    if (personID_copy != personID)
                    {
                        Random rd = new Random();
                        double p = rd.Next(1, 57);//按概率选择窗口
                        if (1 <= p && p <= 3)
                        {
                            kk = 1;
                        }
                        else if (4 <= p && p <= 6)
                        {
                            kk = 2;
                        }
                        else if (7 <= p && p <= 8)
                        {
                            kk = 3;
                        }
                        else if (9 <= p && p <= 10)
                        {
                            kk = 4;
                        }
                        else if (11 <= p && p <= 13)
                        {
                            kk = 5;
                        }
                        else if (14 <= p && p <= 16)
                        {
                            kk = 6;
                        }
                        else if (17 <= p && p <= 18)
                        {
                            kk = 7;
                        }
                        else if (19 <= p && p <= 20)
                        {
                            kk = 8;
                        }
                        else if (p == 21)
                        {
                            kk = 9;
                        }
                        else if (p == 22)
                        {
                            kk = 10;
                        }
                        else if (23 <= p && p <= 25)
                        {
                            kk = 11;
                        }
                        else if (26 <= p && p <= 28)
                        {
                            kk = 12;
                        }
                        else if (29 <= p && p <= 30)
                        {
                            kk = 13;
                        }
                        else if (31 <= p && p <= 32)
                        {
                            kk = 14;
                        }
                        else if (33 <= p && p <= 34)
                        {
                            kk = 15;
                        }
                        else if (35 <= p && p <= 36)
                        {
                            kk = 16;
                        }
                        else if (37 <= p && p <= 39)
                        {
                            kk = 17;
                        }
                        else if (40 <= p && p <= 42)
                        {
                            kk = 18;
                        }
                        else if (43 <= p && p <= 45)
                        {
                            kk = 19;
                        }
                        else if (46 <= p && p <= 48)
                        {
                            kk = 20;
                        }
                        else if (49 <= p && p <= 51)
                        {
                            kk = 21;
                        }
                        else if (52 <= p && p <= 54)
                        {
                            kk = 22;
                        }
                        else if (p == 55)
                        {
                            kk = 23;
                        }
                        else if (p == 56)
                        {
                            kk = 24;
                        }
                    }
                   
                    count2 = 0;
                    foreach (KeyValuePair<string, int> kvp1 in foodGateRankDic)
                    {                       
                        count2++;
                    }

                    kk_cp = kk;
                    foreach (KeyValuePair<string, int> kvp1 in foodGateRankDic)
                    {

                        if (kk > count2)
                        {
                            kk = count2;
                            kk_cp = kk;
                        }
                        
                        cellOrder = kvp1.Value;
                        cellcoord_col = Convert.ToDouble(kvp1.Key.Split('_')[1]);
                        MiniKey = kvp1.Key;
                        kk--;                      

                        if (kk == 0)
                        {
                            kk = kk_cp;
                            break;                           
                        }
                    }
                }

                personID_copy = personID;
                Console.WriteLine(string.Format($"窗口选择：{kk}"));

                //判断入口有没有人
                // 入口ID
                foreach (KeyValuePair<string, CellClass> kvp in gridManager.GridDic_indoor)
                {
                    string in_gateID = kvp.Key;

                    int hasPerson = gridDic[in_gateID].StatusVar;
                    if (i >= 0 && i < stage1)
                    {
                        if (hasPerson == 0 && i % 60 == 0)
                        {
                            personID = personID + 1;
                            personDic.Add(personID, new PersonCanteen(personID, in_gateID, 0, 0, 0));
                            gridDic1[in_gateID].TargetVar = 1;
                            personDic_foodgate.Add(personID,MiniKey);
                            //设置感染者、易感者
                            if (((IList<string>)Infector).Contains(Convert.ToString(personID)))
                            {
                                gridDic1[in_gateID].InfVar = 2;
                            }
                            else
                            {
                                gridDic1[in_gateID].InfVar = 1;
                            }
                            personNextCellDic.Add(personID, in_gateID);
                        }
                    }
                    if (i >= stage1 && i < stage2)
                    {
                        if (hasPerson == 0 && i % 15 == 0)
                        {
                            personID = personID + 1;
                            personDic.Add(personID, new PersonCanteen(personID, in_gateID, 0, 0, 0));
                            gridDic1[in_gateID].TargetVar = 1;
                            personDic_foodgate.Add(personID,MiniKey);
                            //设置感染者、易感者
                            if (((IList<string>)Infector).Contains(Convert.ToString(personID)))
                            {
                                gridDic1[in_gateID].InfVar = 2;
                            }
                            else
                            {
                                gridDic1[in_gateID].InfVar = 1;
                            }
                            personNextCellDic.Add(personID, in_gateID);
                        }
                    }
                    if (i >= stage2 && i < stage3)
                    {
                        if (hasPerson == 0 && i % 6 == 0)
                        {
                            personID = personID + 1;
                            personDic.Add(personID, new PersonCanteen(personID, in_gateID, 0, 0, 0));
                            gridDic1[in_gateID].TargetVar = 1;
                            personDic_foodgate.Add(personID,MiniKey);
                            //设置感染者、易感者
                            if (((IList<string>)Infector).Contains(Convert.ToString(personID)))
                            {
                                gridDic1[in_gateID].InfVar = 2;
                            }
                            else
                            {
                                gridDic1[in_gateID].InfVar = 1;
                            }
                            personNextCellDic.Add(personID, in_gateID);
                        }
                    }
                    if (i >= stage3)
                    {
                        if (hasPerson == 0 && i % 60 == 0)
                        {
                            personID = personID + 1;
                            personDic.Add(personID, new PersonCanteen(personID, in_gateID, 0, 0, 0));
                            gridDic1[in_gateID].TargetVar = 1;
                            personDic_foodgate.Add(personID,MiniKey);
                            //设置感染者、易感者
                            if (((IList<string>)Infector).Contains(Convert.ToString(personID)))
                            {
                                gridDic1[in_gateID].InfVar = 2;
                            }
                            else
                            {
                                gridDic1[in_gateID].InfVar = 1;
                            }
                            personNextCellDic.Add(personID, in_gateID);
                        }
                    }
                }

                //遍历havePerson--当前所有有人的元胞
                foreach (KeyValuePair<string, long> kvp in gridManager.GridDic_HavePerson)
                {
                    int targetVar = gridDic[kvp.Key].TargetVar;
                    personDic[kvp.Value].TotalTime += timestep;
                    if (targetVar == 1)
                    {
                        //找邻居元胞（座位,窗口,障碍物不能进）
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor1(gridDic[kvp.Key], gridManager);

                        List<string> NeighborCoordList = new List<string>();

                        foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                        {
                            foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                            {
                                if (kvp1.Key == kvp2.Key)
                                {
                                    if (kvp2.Value.StatusVar == 0)
                                    {
                                        NeighborCoordList.Add(kvp1.Key);
                                        break;
                                    }
                                }
                            }
                        }

                        if (personDic_foodgate[kvp.Value] == "")
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                            personDic_foodgate[kvp.Value] = MiniKey;
                        }
                        else
                        {
                            if (NeighborCoordList.Count == 0)
                            {
                                personNextCellDic.Add(kvp.Value, kvp.Key);
                            }
                            else if (NeighborCoordList.Count == 1)
                            {
                                personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                            }
                            else
                            {
                                //最小人数队尾坐标
                                double dx = Convert.ToDouble(personDic_foodgate[kvp.Value].Split('_')[1]) + 1.0;
                                double dy = Convert.ToDouble(personDic_foodgate[kvp.Value].Split('_')[0]) + 1.0;

                                int ii = 0;
                                double targetDis = 0.0;
                                string targetID = "";

                                foreach (var item in NeighborCoordList)
                                {

                                    double ox = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                    double oy = Convert.ToDouble(item.Split('_')[0]) + 1.0;

                                    if (ii == 0)
                                    {
                                        targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                        targetID = item;
                                    }
                                    else
                                    {
                                        double temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);

                                        if (targetDis > temDis)
                                        {
                                            targetDis = temDis;
                                            targetID = item;
                                        }
                                    }
                                    ii++;
                                }

                                Random rd = new Random();
                                int count2 = 0;
                                foreach (var item in NeighborCoordList)
                                {
                                    if(item==targetID)
                                    {
                                        if (rd.Next(2) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, targetID);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (rd.Next(3) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, item);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                }
                                if(count2==0)
                                {
                                    personNextCellDic.Add(kvp.Value, targetID);
                                }
                            }
                        }
                    }
                    if (targetVar == 15)
                    {
                        int row = Convert.ToInt32(kvp.Key.Split('_')[0]);
                        int col = Convert.ToInt32(kvp.Key.Split('_')[1]);

                        string previousKey = string.Format($"{row - 1}_{col}");

                        if (gridDic[previousKey].FuncVar == 2 || gridDic[previousKey].FuncVar == 6)
                        {
                            for (int mark = 2; mark != 0; mark++ )//排队跳过障碍物与椅子
                            {
                                previousKey = string.Format($"{row - mark}_{col}");
                                if (gridDic[previousKey].StatusVar == 0 && gridDic[previousKey].FuncVar != 2 && gridDic[previousKey].FuncVar != 6)
                                {
                                    personNextCellDic.Add(kvp.Value, previousKey);
                                    mark = 0;
                                    break;
                                }
                                if ( gridDic[previousKey].FuncVar == 1 && gridDic[previousKey].StatusVar != 0)
                                {
                                    personNextCellDic.Add(kvp.Value, kvp.Key);
                                    mark = 0;
                                    break;
                                }
                            }                         
                        }
                        else
                        {
                            if (gridDic[previousKey].StatusVar == 0)
                            {
                                personNextCellDic.Add(kvp.Value, previousKey);
                            }
                            else
                            {
                                personNextCellDic.Add(kvp.Value, kvp.Key);
                            }
                        }                     
                    }
                    if (targetVar == 17) //在窗口
                    {
                        //如果等待时间小于30，则不动
                        if (personDic[kvp.Value].StopingTime < window_waiting_time)
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else
                        {
                            int row = Convert.ToInt32(kvp.Key.Split('_')[0]);
                            int col = Convert.ToInt32(kvp.Key.Split('_')[1]);

                            string leftCellID = string.Format($"{row}_{col - 1}");
                            string rightCellID = string.Format($"{row}_{col + 1}");
                            int leftStatus = gridDic[leftCellID].StatusVar;
                            int rightStatus = gridDic[rightCellID].StatusVar;

                            if (leftStatus == 1 && rightStatus == 1)
                            {
                                personNextCellDic.Add(kvp.Value, kvp.Key);
                            }
                            else if (leftStatus == 1 && rightStatus == 0)
                            {
                                personNextCellDic.Add(kvp.Value, rightCellID);
                            }
                            else if (leftStatus == 0 && rightStatus == 1)
                            {
                                personNextCellDic.Add(kvp.Value, leftCellID);
                            }
                            else if (leftStatus == 0 && rightStatus == 0)
                            {
                                personNextCellDic.Add(kvp.Value, rightCellID);
                            }
                        }
                    }

                    if (targetVar == 2)
                    {
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor2(gridDic[kvp.Key], gridManager);
                        List<string> NeighborCoordList = new List<string>();
                        foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                        {
                            foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                            {
                                if (kvp1.Key == kvp2.Key)
                                {
                                    if (kvp2.Value.StatusVar == 0)
                                    { 
                                        NeighborCoordList.Add(kvp1.Key);
                                        break;
                                    }                                        
                                }
                            }                              
                        }

                        if (NeighborCoordList.Count == 0)
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else if (NeighborCoordList.Count == 1)
                        {
                            personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                        }
                        else
                        {
                            int iii = -1;
                            int ii = 0;
                            string seatcoord = "";
                            double dx;
                            double dy;
                            double ox;
                            double oy;
                            double targetDis = 0.0;
                            double temDis = 0.0;
                            string targetID = "";
                            foreach (KeyValuePair<string, CellClass> kvp1 in gridManager.GridDic_seat)
                            {
                                dx = Convert.ToDouble(kvp1.Key.Split('_')[1]) + 1.0;
                                dy = Convert.ToDouble(kvp1.Key.Split('_')[0]) + 1.0;
                                ox = Convert.ToDouble(kvp.Key.Split('_')[1]) + 1.0;
                                oy = Convert.ToDouble(kvp.Key.Split('_')[0]) + 1.0;

                                if (iii == -1)
                                {
                                    targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    iii++;
                                    seatcoord = kvp1.Key;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    if (temDis < targetDis)
                                    {
                                        if (kvp1.Value.StatusVar == 0)
                                        {
                                            targetDis = temDis;
                                            seatcoord = kvp1.Key;
                                        }
                                    }
                                }
                            }

                            targetDis = 0.0;
                            temDis = 0.0;

                            foreach (var item in NeighborCoordList)
                            {

                                double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                double nx = Convert.ToDouble(seatcoord.Split('_')[1]) + 1.0;
                                double ny = Convert.ToDouble(seatcoord.Split('_')[0]) + 1.0;

                                if (ii == 0)
                                {
                                    targetDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);
                                    targetID = item;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);

                                    if (targetDis >= temDis)
                                    {
                                        
                                        if (targetDis == temDis && Convert.ToDouble(item.Split('_')[1]) < Convert.ToDouble(targetID.Split('_')[1]))
                                        {
                                            targetDis = targetDis;
                                            targetID = targetID;
                                        }
                                        else 
                                        {
                                            targetDis = temDis;
                                            targetID = item;
                                        }
                                    }
                                }
                                ii++;
                            }

                            Random rd = new Random();
                            int count2 = 0;
                            foreach (var item in NeighborCoordList)
                            {
                                if (item == targetID)
                                {
                                    if (rd.Next(2) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, targetID);
                                        count2 = 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (rd.Next(3) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, item);
                                        count2 = 1;
                                        break;
                                    }
                                }
                            }
                            if (count2 == 0)
                            {
                                personNextCellDic.Add(kvp.Value, targetID);
                            }
                        }
                    }
                    if (targetVar == 25) //在座位
                    {
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor1(gridDic[kvp.Key], gridManager);
                        List<string> NeighborCoordList = new List<string>();
                        if (personDic[kvp.Value].StopingTime < seat_waiting_time) //如果停留时间小于3000，则不动
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                            {
                                foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                                {
                                    if (kvp1.Key == kvp2.Key)
                                    {
                                        if (kvp2.Value.StatusVar == 0)
                                        {
                                            NeighborCoordList.Add(kvp1.Key);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (NeighborCoordList.Count == 0)
                            {
                                personNextCellDic.Add(kvp.Value, kvp.Key);
                            }
                            else if (NeighborCoordList.Count == 1)
                            {
                                personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                            }
                            else
                            {
                                int iii = -1;
                                int ii = 0;
                                string recyclingcoord = "";
                                double dx;
                                double dy;
                                double ox;
                                double oy;
                                double targetDis = 0.0;
                                double temDis = 0.0;
                                string targetID = "";
                                foreach (KeyValuePair<string, CellClass> kvp1 in gridManager.GridDic_recycling)
                                {
                                    dx = Convert.ToDouble(kvp1.Key.Split('_')[1]) + 1.0;
                                    dy = Convert.ToDouble(kvp1.Key.Split('_')[0]) + 1.0;
                                    ox = Convert.ToDouble(kvp.Key.Split('_')[1]) + 1.0;
                                    oy = Convert.ToDouble(kvp.Key.Split('_')[0]) + 1.0;

                                    if (iii == -1)
                                    {
                                        targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                        iii++;
                                        recyclingcoord = kvp1.Key;
                                    }
                                    else
                                    {
                                        temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                        if (temDis < targetDis)
                                        {

                                            if (kvp1.Value.StatusVar == 0)
                                            {
                                                targetDis = temDis;
                                                recyclingcoord = kvp1.Key;
                                            }
                                        }
                                    }
                                }

                                foreach (var item in NeighborCoordList)
                                {

                                    double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                    double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                    double nx = Convert.ToDouble(recyclingcoord.Split('_')[1]) + 1.0;
                                    double ny = Convert.ToDouble(recyclingcoord.Split('_')[0]) + 1.0;

                                    if (ii == 0)
                                    {
                                        targetDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);
                                        targetID = item;
                                    }
                                    else
                                    {
                                        temDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);

                                        if (targetDis > temDis)
                                        {
                                            targetDis = temDis;
                                            targetID = item;
                                        }
                                    }
                                    ii++;
                                }

                                Random rd = new Random();
                                int count2 = 0;
                                foreach (var item in NeighborCoordList)
                                {
                                    if (item == targetID)
                                    {
                                        if (rd.Next(2) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, targetID);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (rd.Next(3) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, item);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                }
                                if (count2 == 0)
                                {
                                    personNextCellDic.Add(kvp.Value, targetID);
                                }
                            }
                        }
                    }

                    if (targetVar == 3)
                    {
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor1(gridDic[kvp.Key], gridManager);
                        List<string> NeighborCoordList = new List<string>();
                        foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                        {
                            foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                            {
                                if (kvp1.Key == kvp2.Key)
                                {
                                    if (kvp2.Value.StatusVar == 0)
                                    {
                                        NeighborCoordList.Add(kvp1.Key);
                                        break;
                                    }
                                }
                            }
                        }

                        if (NeighborCoordList.Count == 0)
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else if (NeighborCoordList.Count == 1)
                        {
                            personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                        }
                        else
                        {
                            int iii = -1;
                            int ii = 0;
                            string outdoorcoord = "";
                            double dx;
                            double dy;
                            double ox;
                            double oy;
                            double targetDis = 0.0;
                            double temDis = 0.0;
                            string targetID = "";
                            foreach (KeyValuePair<string, CellClass> kvp1 in gridManager.GridDic_recycling)
                            {
                                dx = Convert.ToDouble(kvp1.Key.Split('_')[1]) + 1.0;
                                dy = Convert.ToDouble(kvp1.Key.Split('_')[0]) + 1.0;
                                ox = Convert.ToDouble(kvp.Key.Split('_')[1]) + 1.0;
                                oy = Convert.ToDouble(kvp.Key.Split('_')[0]) + 1.0;

                                if (iii == -1)
                                {
                                    targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    iii++;
                                    outdoorcoord = kvp1.Key;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    if (temDis < targetDis)
                                    {
                                        if (kvp1.Value.StatusVar == 0)
                                        {
                                            targetDis = temDis;
                                            outdoorcoord = kvp1.Key;
                                        }
                                    }
                                }
                            }

                            foreach (var item in NeighborCoordList)
                            {
                                double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                double nx = Convert.ToDouble(outdoorcoord.Split('_')[1]) + 1.0;
                                double ny = Convert.ToDouble(outdoorcoord.Split('_')[0]) + 1.0;

                                if (ii == 0)
                                {
                                    targetDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);
                                    targetID = item;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);

                                    if (targetDis > temDis)
                                    {
                                        targetDis = temDis;
                                        targetID = item;
                                    }
                                }
                                ii++;
                            }

                            Random rd = new Random();
                            int count2 = 0;
                            foreach (var item in NeighborCoordList)
                            {
                                if (item == targetID)
                                {
                                    if (rd.Next(2) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, targetID);
                                        count2 = 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (rd.Next(3) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, item);
                                        count2 = 1;
                                        break;
                                    }
                                }
                            }
                            if (count2 == 0)
                            {
                                personNextCellDic.Add(kvp.Value, targetID);
                            }
                        }
                    }

                    if (targetVar == 35)//在餐具回收点
                    {
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor1(gridDic[kvp.Key], gridManager);
                        List<string> NeighborCoordList = new List<string>();
                        if (personDic[kvp.Value].StopingTime < recycle_waiting_time) //如果逗留时间小于12，则不动
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                            {
                                foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                                {
                                    if (kvp1.Key == kvp2.Key)
                                    {
                                        if (kvp2.Value.StatusVar == 0)
                                        {
                                            NeighborCoordList.Add(kvp1.Key);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (NeighborCoordList.Count == 0)
                            {
                                personNextCellDic.Add(kvp.Value, kvp.Key);
                            }
                            else if (NeighborCoordList.Count == 1)
                            {
                                personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                            }
                            else
                            {
                                int iii = -1;
                                int ii = 0;
                                string outdoorcoord = "";
                                double dx;
                                double dy;
                                double ox;
                                double oy;
                                double targetDis = 0.0;
                                double temDis = 0.0;
                                string targetID = "";
                                foreach (KeyValuePair<string, CellClass> kvp1 in gridManager.GridDic_outdoor)
                                {
                                    dx = Convert.ToDouble(kvp1.Key.Split('_')[1]) + 1.0;
                                    dy = Convert.ToDouble(kvp1.Key.Split('_')[0]) + 1.0;
                                    ox = Convert.ToDouble(kvp.Key.Split('_')[1]) + 1.0;
                                    oy = Convert.ToDouble(kvp.Key.Split('_')[0]) + 1.0;

                                    if (iii == -1)
                                    {
                                        targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                        iii++;
                                        outdoorcoord = kvp1.Key;
                                    }
                                    else
                                    {
                                        temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                        if (temDis < targetDis)
                                        {
                                            if (kvp1.Value.StatusVar == 0)
                                            {
                                                targetDis = temDis;
                                                outdoorcoord = kvp1.Key;
                                            }
                                        }
                                    }
                                }

                                foreach (var item in NeighborCoordList)
                                {

                                    double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                    double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                    double nx = Convert.ToDouble(outdoorcoord.Split('_')[1]) + 1.0;
                                    double ny = Convert.ToDouble(outdoorcoord.Split('_')[0]) + 1.0;

                                    if (ii == 0)
                                    {
                                        targetDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);
                                        targetID = item;
                                    }
                                    else
                                    {
                                        temDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);

                                        if (targetDis > temDis)
                                        {
                                            targetDis = temDis;
                                            targetID = item;
                                        }
                                    }
                                    ii++;
                                }

                                Random rd = new Random();
                                int count2 = 0;
                                foreach (var item in NeighborCoordList)
                                {
                                    if (item == targetID)
                                    {
                                        if (rd.Next(2) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, targetID);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (rd.Next(3) == 0)
                                        {
                                            personNextCellDic.Add(kvp.Value, item);
                                            count2 = 1;
                                            break;
                                        }
                                    }
                                }
                                if (count2 == 0)
                                {
                                    personNextCellDic.Add(kvp.Value, targetID);
                                }
                            }
                        }
                    }

                    if (targetVar == 4)//目标出口
                    {
                        Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor1(gridDic[kvp.Key], gridManager);
                        List<string> NeighborCoordList = new List<string>();
                        foreach (KeyValuePair<string, CellClass> kvp1 in neighborCellDic)
                        {
                            foreach (KeyValuePair<string, CellClass> kvp2 in gridDic)
                            {
                                if (kvp1.Key == kvp2.Key)
                                {
                                    if (kvp2.Value.StatusVar == 0)
                                    {
                                        NeighborCoordList.Add(kvp1.Key);
                                        break;
                                    }
                                }
                            }
                        }

                        if (NeighborCoordList.Count == 0)
                        {
                            personNextCellDic.Add(kvp.Value, kvp.Key);
                        }
                        else if (NeighborCoordList.Count == 1)
                        {
                            personNextCellDic.Add(kvp.Value, NeighborCoordList[0]);
                        }
                        else
                        {
                            int iii = -1;
                            int ii = 0;
                            string outdoorcoord = "";
                            double dx;
                            double dy;
                            double ox;
                            double oy;
                            double targetDis = 0.0;
                            double temDis = 0.0;
                            string targetID = "";
                            foreach (KeyValuePair<string, CellClass> kvp1 in gridManager.GridDic_outdoor)
                            {
                                dx = Convert.ToDouble(kvp1.Key.Split('_')[1]) + 1.0;
                                dy = Convert.ToDouble(kvp1.Key.Split('_')[0]) + 1.0;
                                ox = Convert.ToDouble(kvp.Key.Split('_')[1]) + 1.0;
                                oy = Convert.ToDouble(kvp.Key.Split('_')[0]) + 1.0;

                                if (iii == -1)
                                {
                                    targetDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    iii++;
                                    outdoorcoord = kvp1.Key;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(ox - dx, 2) + Math.Pow(oy - dy, 2)), 0.5);
                                    if (temDis < targetDis)
                                    {

                                        if (kvp1.Value.StatusVar == 0)
                                        {
                                            targetDis = temDis;
                                            outdoorcoord = kvp1.Key;
                                        }
                                    }
                                }
                            }

                            foreach (var item in NeighborCoordList)
                            {

                                double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                double nx = Convert.ToDouble(outdoorcoord.Split('_')[1]) + 1.0;
                                double ny = Convert.ToDouble(outdoorcoord.Split('_')[0]) + 1.0;

                                if (ii == 0)
                                {
                                    targetDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);
                                    targetID = item;
                                }
                                else
                                {
                                    temDis = Math.Pow((Math.Pow(mx - nx, 2) + Math.Pow(my - ny, 2)), 0.5);

                                    if (targetDis > temDis)
                                    {
                                        targetDis = temDis;
                                        targetID = item;
                                    }
                                }
                                ii++;
                            }

                            Random rd = new Random();
                            int count2 = 0;
                            foreach (var item in NeighborCoordList)
                            {
                                if (item == targetID)
                                {
                                    if (rd.Next(2) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, targetID);
                                        count2 = 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (rd.Next(3) == 0)
                                    {
                                        personNextCellDic.Add(kvp.Value, item);
                                        count2 = 1;
                                        break;
                                    }
                                }
                            }
                            if (count2 == 0)
                            {
                                personNextCellDic.Add(kvp.Value, targetID);
                            }
                        }
                    }
                }

                //使集合nextperson不重复
                Dictionary<long, ODCoord> NowNextCoordDic = new Dictionary<long, ODCoord>();
                foreach (KeyValuePair<long, string> kv in personNextCellDic)
                {
                    string nowCoord = personDic[kv.Key].CellCoord;
                    string nextCoord = kv.Value;

                    NowNextCoordDic.Add(kv.Key, new ODCoord(nowCoord, nextCoord));
                }

                Dictionary<string, Dictionary<long, string>> NowNextCoordDic1 = new Dictionary<string, Dictionary<long, string>>();

                Dictionary<long, ODCoord> copyDic = new Dictionary<long, ODCoord>(NowNextCoordDic);
                int flag = 0;

                do
                {
                    flag = 0;
                    NowNextCoordDic = copyDic;
                    NowNextCoordDic1 = new Dictionary<string, Dictionary<long, string>>();

                    foreach (KeyValuePair<long, ODCoord> kv in NowNextCoordDic)
                    {
                        long a = 0;
                        long key = kv.Key;
                        long key1 = 0;
                        string coord1 = "_";
                        ODCoord coord = kv.Value;
                        
                        foreach (KeyValuePair<string, Dictionary<long,string>> kv1 in NowNextCoordDic1)
                        {
                            if (kv1.Key == coord.dcoord)
                            {
                                foreach (KeyValuePair<long, string> kv2 in kv1.Value)
                                {
                                    //获取已存储中与当前判断项相同的人ID与原坐标
                                    key1 = kv2.Key;
                                    coord1 = kv2.Value;
                                    break;
                                }
                                if (coord.dcoord == coord.ocoord)
                                {
                                    copyDic[key1].dcoord = copyDic[key1].ocoord;
                                    flag++;
                                    a = 1;
                                    break; 
                                }
                                else if (kv1.Key == coord1)
                                {
                                    copyDic[key].dcoord = copyDic[key].ocoord;
                                    flag++;
                                    a = 1;
                                    break;
                                }
                                else if(coord.dcoord != coord.ocoord && kv1.Key != coord1)
                                {
                                    if (personDic[key].TotalTime >= personDic[key1].TotalTime)
                                    {
                                        copyDic[key1].dcoord = copyDic[key1].ocoord;
                                        flag++;
                                        a = 1;
                                        break;
                                    }
                                    else
                                    {
                                        copyDic[key].dcoord = copyDic[key].ocoord;
                                        flag++;
                                        a = 1;
                                        break;
                                    }
                                }
                            }
                        }
                        if(a == 0)
                        {
                            NowNextCoordDic1.Add(coord.dcoord, new Dictionary<long, string> { { key, coord.ocoord } });
                        }
                    }
                } while (flag > 0);

                foreach (KeyValuePair<long, ODCoord> kv in copyDic)
                {
                    personNextCellDic[kv.Key] = kv.Value.dcoord;
                }
                //
                Console.WriteLine(string.Format($"第{i * 0.5}秒"));
                Console.WriteLine(string.Format($"当前循环总人数：{personNextCellDic.Count}"));
                foreach (KeyValuePair<long, PersonCanteen> kv2 in personDic)
                {
                    pid = kv2.Key;
                    curTime = kv2.Value.TotalTime;
                    xyStr = kv2.Value.CellCoord;
                    Target = gridDic[kv2.Value.CellCoord].TargetVar;
                    InfVar = gridDic[kv2.Value.CellCoord].InfVar;
                    Infectionp = kv2.Value.Infectionp;
                    stopping = kv2.Value.StopingTime;
                    pc = new PersonCube(pid, curTime, xyStr, Target, InfVar, Infectionp, stopping,i*0.5);
                    list.Add(pc);
                    Console.WriteLine(string.Format($"人编号：{kv2.Key},实体位置：{kv2.Value.CellCoord},实体目标：" + $"{gridDic[kv2.Value.CellCoord].TargetVar },实体stopping时间：{kv2.Value.StopingTime},感染概率：{kv2.Value.Infectionp}"));
                }
                //foreach (KeyValuePair<long, string> kv2 in personDic_foodgate)
                //{
                //    Console.WriteLine(string.Format($"人编号：{kv2.Key},实体目标位置：{kv2.Value}"));
                //}

                //改变新表属性
                foreach (KeyValuePair<long, string> kvp1 in personNextCellDic)
                {
                    //当前坐标
                    string coord_1 = "_";
                    long coord_id = 0;
                    gridDic1[kvp1.Value].StatusVar = 1;
                    foreach (KeyValuePair<string, long> kv1 in gridManager.GridDic_HavePerson)
                    {
                        if (kv1.Value == kvp1.Key)
                        {
                            coord_1 = kv1.Key;
                            coord_id = kv1.Value;
                        }
                    }
                    int targetVar;
                    //string in_gateID2 = string.Format($"{3}_{0}");
                    if (coord_1 == "_")
                    {
                        targetVar = 0;                 
                    }
                    else
                    {
                        targetVar = gridDic[coord_1].TargetVar;
                    }
                 
                    //判断并改变新表感染情况
                    if (coord_1 != "_")
                    {
                        if (gridDic[coord_1].InfVar == 1)
                        {
                            Dictionary<string, CellClass> neighborCellDic = gridManager.getCellNeibor_n_order(gridDic[coord_1], gridManager, 2);
                            List<string> NeighborCoordList = new List<string>();
                            List<string> NeighborCoordList2 = new List<string>();
                            foreach (KeyValuePair<string, CellClass> kvp3 in neighborCellDic)
                            {                                
                                foreach (KeyValuePair<string, CellClass> kvp4 in gridDic)
                                {
                                    if (kvp3.Key == kvp4.Key)
                                    {
                                        //搜索邻近潜伏者
                                        if (kvp4.Value.InfVar == 2)
                                        {
                                            NeighborCoordList.Add(kvp3.Key);
                                            break;
                                        }
                                        //搜索邻近感染者
                                        if (kvp4.Value.InfVar == 3)
                                        {
                                            NeighborCoordList2.Add(kvp3.Key);
                                            break;
                                        }
                                    }
                                }
                            }
                            
                            double p = 0.0;
                            double p2 = 0.0;
                            foreach (var item in NeighborCoordList)
                            {
                                double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                double x = Convert.ToDouble(coord_1.Split('_')[1]) + 1.0;
                                double y = Convert.ToDouble(coord_1.Split('_')[0]) + 1.0;

                                p = p + ((1/Math.Pow((Math.Pow(mx - x, 2) + Math.Pow(my - y, 2)), 0.5)) / 13.8203) * 1 * infection_rate_E;
                            }
                            foreach (var item in NeighborCoordList2)
                            {
                                double mx = Convert.ToDouble(item.Split('_')[1]) + 1.0;
                                double my = Convert.ToDouble(item.Split('_')[0]) + 1.0;
                                double x = Convert.ToDouble(coord_1.Split('_')[1]) + 1.0;
                                double y = Convert.ToDouble(coord_1.Split('_')[0]) + 1.0;

                                p2 = p2 + ((1/Math.Pow((Math.Pow(mx - x, 2) + Math.Pow(my - y, 2)), 0.5)) / 13.8203) * 1 * infection_rate_I;
                            }
                            personDic[kvp1.Key].Infectionp = personDic[kvp1.Key].Infectionp + p + p2;

                            if (personDic[kvp1.Key].Infectionp > infection_threshold)
                            {
                                gridDic1[kvp1.Value].InfVar = 2;
                                personDic[kvp1.Key].Infectionp = 0;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].InfVar = 1;
                            }
                        }
                        else
                        {
                            gridDic1[kvp1.Value].InfVar = gridDic[coord_1].InfVar;
                        }
                    }

                    if (targetVar == 1)
                    {
                        int col = Convert.ToInt32(kvp1.Value.Split('_')[1]);
                        int footgatecol = Convert.ToInt32(personDic_foodgate[coord_id].Split('_')[1]);

                        if (col == footgatecol)
                        {
                            gridDic1[kvp1.Value].TargetVar = 15;
                            footgatecol = 0;
                            col = 0;                           
                        }
                        else
                        {
                            gridDic1[kvp1.Value].TargetVar = 1;
                            footgatecol = 0;
                            col = 0;
                        }
                    }
                    else if (targetVar == 15)
                    {
                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_foodGate)
                        {
                            string coord = kvp1.Value;
                            string foodCoord = kvp2.Key;

                            if (coord == foodCoord)
                            {
                                gridDic1[kvp1.Value].TargetVar = 17;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 15;
                            }
                        }
                    }
                  
                    else if (targetVar == 17)
                    {                       
                        int ii = 0;
                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_foodGate)
                        {
                            string coord = kvp1.Value;
                            string foodCoord = kvp2.Key;

                            if (coord == foodCoord)
                            { 
                                gridDic1[kvp1.Value].TargetVar = 17;
                                ii = 0;
                                break;                               
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 2;
                                ii = 1;                                                          
                            }
                        }
                        if (ii == 0)
                        {
                            personDic[kvp1.Key].StopingTime += timestep;
                        }
                        if(ii==1)
                        {
                            personDic[kvp1.Key].StopingTime = 0;
                        }
                    }

                    else if (targetVar == 2)
                    {
                        //找座位坐标
                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_seat)
                        {
                            string coord = kvp1.Value;
                            string seatCoord = kvp2.Key;
                            if (coord == seatCoord)
                            {
                                gridDic1[kvp1.Value].TargetVar = 25;
                                personDic[kvp1.Key].StopingTime = 0;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 2;
                            }
                        }
                    }

                    else if (targetVar == 25)
                    {
                        int iiiii = 0;

                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_seat)
                        {
                            string coord = kvp1.Value;
                            string seatCoord = kvp2.Key;

                            if (coord == seatCoord)
                            {  
                                gridDic1[kvp1.Value].TargetVar = 25;
                                iiiii = 0;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 3;
                                iiiii = 1;                              
                            }
                        }
                        if (iiiii == 0)
                        {
                            personDic[kvp1.Key].StopingTime += timestep;
                        }
                        if(iiiii==1)
                        {
                            personDic[kvp1.Key].StopingTime = 0;
                        }
                    }

                    else if (targetVar == 3)
                    {
                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_recycling)
                        {
                            string coord = kvp1.Value;
                            string recyclingCoord = kvp2.Key;
                            if (coord == recyclingCoord)
                            {
                                gridDic1[kvp1.Value].TargetVar = 35;
                                personDic[kvp1.Key].StopingTime = 0;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 3;
                            }
                        }
                    }

                    else if (targetVar == 35)
                    {
                        int iiiiii = 0;

                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_seat)
                        {
                            string coord = kvp1.Value;
                            string recyclingCoord = kvp2.Key;

                            if (coord != recyclingCoord)
                            {
                                gridDic1[kvp1.Value].TargetVar = 4;
                                personDic[kvp1.Key].StopingTime = 0;
                                iiiiii = 1;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 35;
                            }
                        }
                        if (iiiiii == 0)
                        {
                            personDic[kvp1.Key].StopingTime += timestep;
                        }
                    }

                    else if (targetVar == 4)
                    {
                        foreach (KeyValuePair<string, CellClass> kvp2 in gridManager.GridDic_outdoor)
                        {
                            string coord = kvp1.Value;
                            string outdoorCoord = kvp2.Key;
                            if (coord == outdoorCoord)
                            {
                                gridDic1[kvp1.Value].TargetVar = 45;
                                personDic[kvp1.Key].StopingTime = 0;
                                break;
                            }
                            else
                            {
                                gridDic1[kvp1.Value].TargetVar = 4;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<long, string> kv in personNextCellDic)
                {
                    //更新personDic坐标
                    personDic[kv.Key].CellCoord = kv.Value;
                }
                gridManager1.GridDic = gridDic1;

                //更新haveperson
                foreach (KeyValuePair<long, string> kv in personNextCellDic)
                {
                    gridManager1.GridDic_HavePerson.Add(kv.Value, kv.Key);
                }

                ////Console.WriteLine(string.Format($"第{i}次循环"));
                //Console.WriteLine(string.Format($"第{i*0.25}秒"));
                //Console.WriteLine(string.Format($"当前循环总人数：{personNextCellDic.Count}"));

                ////foreach (KeyValuePair<long, string> kv in personNextCellDic)
                ////{
                ////    Console.WriteLine(string.Format($"人编号：{kv.Key},元胞位置：{kv.Value}"));

                ////}
                //foreach (KeyValuePair<long, PersonCanteen> kv2 in personDic)
                //{
                //    Console.WriteLine(string.Format($"人编号：{kv2.Key},实体位置：{kv2.Value.CellCoord},感染概率：{kv2.Value.Infectionp}"));
                //    //Console.WriteLine(personDic[kv2.Key].Infectionp);
                //}
            }

            Output(list);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string line;
            string[] data;
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt文件（*.txt）|*.txt|csv文件（*.csv）|*.csv|日志文件（*.ldf）|*.ldf";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                textBox11.Text = Path.GetFileName(openFileDialog.FileName);

                StreamReader fileStream = new StreamReader(fName);
                while ((line = fileStream.ReadLine()) != null)
                {
                    data = line.Split(' ');
                    foreach (var item in data)
                    {
                        switch(Convert.ToInt32(data[0]))
                        {
                            case 1: FoodGate = data[1].Split(',');break;
                            case 2: Obstatcle = data[1].Split(','); break;
                            case 3: Seat = data[1].Split(','); break;
                            case 4: Recycling = data[1].Split(','); break;
                            case 5: Indoor = data[1].Split(','); break;
                            case 6: Outdoor = data[1].Split(','); break;
                        }
                    }
                }
                fileStream.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            //设置文件类型 
            saveFile.Filter = "txt文件（*.txt）|*.txt|csv文件（*.csv）|*.csv|日志文件（*.ldf）|*.ldf";

            //设置默认文件类型显示顺序 
            saveFile.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            saveFile.RestoreDirectory = true;

            //设置默认的文件名

            //sfd.DefaultFileName = "YourFileName";// in wpf is  sfd.FileName = "YourFileName";

            //点了保存按钮进入 
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                 FilePath = saveFile.FileName.ToString(); //获得文件路径 
                //string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
            }
            textBox17.Text = Path.GetFileName(FilePath);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string line;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt文件（*.txt）|*.txt|csv文件（*.csv）|*.csv|日志文件（*.ldf）|*.ldf";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                textBox21.Text = Path.GetFileName(openFileDialog.FileName);

                StreamReader fileStream = new StreamReader(fName);
                while ((line = fileStream.ReadLine()) != null)
                {
                    Infector = line.Split(',');
                }
            }
        }

        public static void Output(List<PersonCube> list)
        {
            string path = FilePath;
            StreamWriter sw = new StreamWriter(path);

            foreach (var item in list)
            {
                string str = string.Format($"{item.personID},{item.curTime},{Convert.ToInt32(item.xyStr.Split('_')[1]) + 1},{Convert.ToInt32(item.xyStr.Split('_')[0]) + 1},{item.Target},{item.InfVar},{item.Infectionp},{item.stopping},{item.time}");

                sw.WriteLine(str);
            }
            sw.Close();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox19_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class PersonIDRepeatCount
    {

    }

    public class ODCoord
    {
        public string ocoord;
        public string dcoord;

        public ODCoord(string ocoord, string dcoord)
        {
            this.ocoord = ocoord;
            this.dcoord = dcoord;
        }
    }

    public class PersonCanteen
    {
        /// <summary>
        /// 人的ID
        /// </summary>
        private long personID;
        /// <summary>
        /// 人所在元胞的坐标
        /// </summary>
        private string cellCoord;
        /// <summary>
        /// /总时间
        /// </summary>
        private double totalTime = 0.0;
        /// <summary>
        /// /停留时间
        /// </summary>
        private double stopingTime = 0.0;
        /// <summary>
        /// /停留时间
        /// </summary>
        private double infectionp = 0.0;


        /// <summary>
        /// 人信息的构造函数
        /// </summary>
        /// <param name="personID">人的ID</param>
        /// <param name="cellCoord">人当前所在元胞的坐标</param>
        /// <param name="stopingTime">停留时间时间</param>
        public PersonCanteen(long personID, string cellCoord, double stopingTime, double totalTime, double infectionp)
        {
            this.personID = personID;
            this.cellCoord = cellCoord;
            this.stopingTime = stopingTime;
            this.totalTime = totalTime;
            this.infectionp = infectionp;
        }

        public long PersonID { get => personID; set => personID = value; }
        public string CellCoord { get => cellCoord; set => cellCoord = value; }
        public double StopingTime { get => stopingTime; set => stopingTime = value; }
        public double TotalTime { get => totalTime; set => totalTime = value; }
        public double Infectionp { get => infectionp; set => infectionp = value; }

    }

    public class GridManager
    {
        /// <summary>
        /// 总行数
        /// </summary>
        private int rowCount;
        /// <summary>
        /// 总列数
        /// </summary>
        private int colCount;
        /// <summary>
        /// 元胞距离
        /// </summary>
        private double cellDistance;

        /// <summary>
        /// 元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic = null;
        /// <summary>
        /// 窗口所在元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic_foodGate = null;
        /// <summary>
        /// 有人元胞字典
        /// </summary>
        private Dictionary<string, long> gridDic_HavePerson = null;
        /// <summary>
        /// 座位所在元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic_seat = null;
        /// <summary>
        /// 回收点所在元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic_recycling = null;
        /// <summary>
        /// 出口所在元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic_outdoor = null;
        /// <summary>
        /// 出口所在元胞字典
        /// </summary>
        private Dictionary<string, CellClass> gridDic_indoor = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount">总行数</param>
        /// <param name="colCount">总列数</param>
        /// <param name="cellDistance">元胞距离</param>
        public GridManager(int rowCount, int colCount, double cellDistance)
        {
            this.RowCount = rowCount;
            this.ColCount = colCount;
            this.CellDistance = cellDistance;

            GridDic = new Dictionary<string, CellClass>();
            gridDic_foodGate = new Dictionary<string, CellClass>();
            gridDic_HavePerson = new Dictionary<string, long>();
            gridDic_seat = new Dictionary<string, CellClass>();
            gridDic_recycling = new Dictionary<string, CellClass>();
            gridDic_outdoor = new Dictionary<string, CellClass>();
            gridDic_indoor = new Dictionary<string, CellClass>();

        }

        public int RowCount { get => rowCount; set => rowCount = value; }
        public int ColCount { get => colCount; set => colCount = value; }
        public double CellDistance { get => cellDistance; set => cellDistance = value; }
        public Dictionary<string, CellClass> GridDic { get => gridDic; set => gridDic = value; }
        public Dictionary<string, CellClass> GridDic_foodGate { get => gridDic_foodGate; set => gridDic_foodGate = value; }
        public Dictionary<string, CellClass> GridDic_seat { get => gridDic_seat; set => gridDic_seat = value; }
        public Dictionary<string, CellClass> GridDic_recycling { get => gridDic_recycling; set => gridDic_recycling = value; }
        public Dictionary<string, CellClass> GridDic_outdoor { get => gridDic_outdoor; set => gridDic_outdoor = value; }
        public Dictionary<string, CellClass> GridDic_indoor { get => gridDic_indoor; set => gridDic_indoor = value; }
        public Dictionary<string, long> GridDic_HavePerson { get => gridDic_HavePerson; set => gridDic_HavePerson = value; }

        /// <summary>
        /// 构建并初始化元胞格网
        /// </summary>
        public void createGrid()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColCount; col++)
                {
                    string cellID = string.Format($"{row}_{col}");

                    CellClass cell = new CellClass(cellID, 0, 0, 0, 0.0, 0);


                    GridDic.Add(cellID, cell);

                }
            }
        }

        /// <summary>
        /// 通过人的ID移除有人元胞字典中的元素
        /// </summary>
        /// <param name="id">人的ID</param>
        public void RemovePersonByID(string id)
        {
            this.gridDic_HavePerson.Remove(id);
        }

        /// <summary>
        /// 通过人的ID向有人元胞字典中添加新元素
        /// </summary>
        /// <param name="id">人的ID</param>
        public void AddPersonByID(string id)
        {
            //this.gridDic_HavePerson.Add(id, this.gridDic[id]);
        }

        /// <summary>
        /// 通过元胞行列号获取元胞网格
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="col">列号</param>
        /// <returns></returns>
        public CellClass getCellByCoord(int row, int col)
        {
            string id = string.Format($"{row}_{col}");

            CellClass cell = this.GridDic[id];

            return cell;
        }

        /// <summary>
        /// 通过元胞对象获取其所有邻近元胞对象的字典（座位，窗口，障碍物不可进）
        /// </summary>
        /// <param name="cell">元胞对象</param>
        /// <returns></returns>
        public Dictionary<string, CellClass> getCellNeibor1(CellClass cell, GridManager gridManager)
        {
            Dictionary<string, CellClass> dic = new Dictionary<string, CellClass>();

            //目标元胞坐标
            string cellID = cell.CellID;

            //左下角；右下角；左上角；右上角 坐标
            string vexID1 = string.Format($"{0}_{0}");
            string vexID2 = string.Format($"{0}_{ColCount - 1}");
            string vexID3 = string.Format($"{RowCount - 1}_{ColCount - 1}");
            string vexID4 = string.Format($"{RowCount - 1}_{0}");

            int rowIndex = Convert.ToInt32(cellID.Split('_')[0]);
            int colIndex = Convert.ToInt32(cellID.Split('_')[1]);

            //左下角
            if (cellID == vexID1)
            {
                string id = string.Format($"{0}_{1}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(0, 1));
                }

                id = string.Format($"{1}_{0}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(1, 0));
                }
            }
            // 右下角
            else if (cellID == vexID2)
            {
                string id = string.Format($"{0}_{ColCount - 2}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(0, ColCount - 2));
                }

                id = string.Format($"{1}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(1, ColCount - 1));
                }
            }
            // 右上角
            else if (cellID == vexID3)
            {
                string id = string.Format($"{RowCount - 1}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, ColCount - 1));
                }
                
                id = string.Format($"{RowCount - 2}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, ColCount - 2));
                }
            }
            // 左上角
            else if (cellID == vexID4)
            {
                string id = string.Format($"{RowCount - 1}_{1}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 1, 1));
                }

                id = string.Format($"{RowCount - 2}_{0}");
                if (gridManager.gridDic[id].FuncVar != 2 && gridManager.gridDic[id].FuncVar != 1 && gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, 0));
                }
            }
            else if (rowIndex == 0 && colIndex != 0 && colIndex != (colCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 2 && gridManager.gridDic[indexLeft].FuncVar != 1 && gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft] );
                }
                if (gridManager.gridDic[indexRight].FuncVar != 2 && gridManager.gridDic[indexRight].FuncVar != 1 && gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 2 && gridManager.gridDic[indexTop].FuncVar != 1 && gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
            }
            else if (rowIndex != 0 && rowIndex != (rowCount - 1) && colIndex == 0)
            {
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexRight].FuncVar != 2 && gridManager.gridDic[indexRight].FuncVar != 1 && gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 2 && gridManager.gridDic[indexTop].FuncVar != 1 && gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 2 && gridManager.gridDic[indexBottom].FuncVar != 1 && gridManager.gridDic[indexBottom].FuncVar != 6) 
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            else if (rowIndex != 0 && rowIndex != (rowCount - 1) && colIndex == (colCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 2 && gridManager.gridDic[indexLeft].FuncVar != 1 && gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 2 && gridManager.gridDic[indexTop].FuncVar != 1 && gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 2 && gridManager.gridDic[indexBottom].FuncVar != 1 && gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            else if (colIndex != 0 && colIndex != (colCount - 1) && rowIndex == (rowCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 2 && gridManager.gridDic[indexLeft].FuncVar != 1 && gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexRight].FuncVar != 2 && gridManager.gridDic[indexRight].FuncVar != 1 && gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 2 && gridManager.gridDic[indexBottom].FuncVar != 1 && gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom] );
                }
            }
            else
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 2 && gridManager.gridDic[indexLeft].FuncVar != 1 && gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexRight].FuncVar != 2 && gridManager.gridDic[indexRight].FuncVar != 1 && gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 2 && gridManager.gridDic[indexTop].FuncVar != 1 && gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 2 && gridManager.gridDic[indexBottom].FuncVar != 1 && gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            return dic;
        }

        //找邻居（座位可进）
        public Dictionary<string, CellClass> getCellNeibor2(CellClass cell, GridManager gridManager)
        {
            Dictionary<string, CellClass> dic = new Dictionary<string, CellClass>();

            //目标元胞坐标
            string cellID = cell.CellID;

            //左下角；右下角；左上角；右上角 坐标
            string vexID1 = string.Format($"{0}_{0}");
            string vexID2 = string.Format($"{0}_{ColCount - 1}");
            string vexID3 = string.Format($"{RowCount - 1}_{ColCount - 1}");
            string vexID4 = string.Format($"{RowCount - 1}_{0}");

            int rowIndex = Convert.ToInt32(cellID.Split('_')[0]);
            int colIndex = Convert.ToInt32(cellID.Split('_')[1]);

            //左下角
            if (cellID == vexID1)
            {
                string id = string.Format($"{0}_{1}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(0, 1));
                }

                id = string.Format($"{1}_{0}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(1, 0));
                }
            }
            // 右下角
            else if (cellID == vexID2)
            {
                string id = string.Format($"{0}_{ColCount - 2}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(0, ColCount - 2));
                }

                id = string.Format($"{1}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(1, ColCount - 1));
                }
            }
            // 右上角
            else if (cellID == vexID3)
            {
                string id = string.Format($"{RowCount - 1}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, ColCount - 1));
                }

                id = string.Format($"{RowCount - 2}_{ColCount - 1}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, ColCount - 2));
                }
            }
            // 左上角
            else if (cellID == vexID4)
            {
                string id = string.Format($"{RowCount - 1}_{1}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 1, 1));
                }

                id = string.Format($"{RowCount - 2}_{0}");
                if (gridManager.gridDic[id].FuncVar != 6)
                {
                    dic.Add(id, getCellByCoord(RowCount - 2, 0));
                }
            }
            else if (rowIndex == 0 && colIndex != 0 && colIndex != (colCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
            }
            else if (rowIndex != 0 && rowIndex != (rowCount - 1) && colIndex == 0)
            {
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            else if (rowIndex != 0 && rowIndex != (rowCount - 1) && colIndex == (colCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            else if (colIndex != 0 && colIndex != (colCount - 1) && rowIndex == (rowCount - 1))
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            else
            {
                string indexLeft = string.Format($"{rowIndex}_{colIndex - 1}");
                string indexRight = string.Format($"{rowIndex}_{colIndex + 1}");
                string indexTop = string.Format($"{rowIndex + 1}_{colIndex}");
                string indexBottom = string.Format($"{rowIndex - 1}_{colIndex}");

                if (gridManager.gridDic[indexLeft].FuncVar != 6)
                {
                    dic.Add(indexLeft, this.GridDic[indexLeft]);
                }
                if (gridManager.gridDic[indexRight].FuncVar != 6)
                {
                    dic.Add(indexRight, this.GridDic[indexRight]);
                }
                if (gridManager.gridDic[indexTop].FuncVar != 6)
                {
                    dic.Add(indexTop, this.GridDic[indexTop]);
                }
                if (gridManager.gridDic[indexBottom].FuncVar != 6)
                {
                    dic.Add(indexBottom, this.GridDic[indexBottom]);
                }
            }
            return dic;
        }

        /// <summary>
        /// 搜索n阶邻域
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="gridManager"></param>
        /// <returns></returns>
        public Dictionary<string, CellClass> getCellNeibor_n_order(CellClass cell, GridManager gridManager, long n)
        {
            Dictionary<string, CellClass> dic = new Dictionary<string, CellClass>();
            string cellID = cell.CellID;
            int rowIndex = Convert.ToInt32(cellID.Split('_')[0]); 
            int colIndex = Convert.ToInt32(cellID.Split('_')[1]);
            for (long j = n; j >= 0; j--)
            {
                if (rowIndex - j >= 0 && rowIndex - j < rowCount)
                {
                    for (long m = n; m >= 0; m--)
                    {
                        if (colIndex - m >= 0 && colIndex - m < colCount)
                        {
                            if (rowIndex - j != rowIndex || colIndex - m != colIndex)
                            {
                                string id = string.Format($"{rowIndex - j}_{colIndex - m}");
                                dic.Add(id, this.GridDic[id]);                               
                            }                          
                        }
                    }
                    for (long m = n; m > 0; m--)
                    {
                        if (colIndex + m >= 0 && colIndex + m < colCount)
                        {
                            if (rowIndex - j != rowIndex || colIndex + m != colIndex)
                            {
                                string id = string.Format($"{rowIndex - j}_{colIndex + m}");
                                dic.Add(id, this.GridDic[id]);
                            }
                        }
                    }
                }
            }
            for (long j = n; j > 0; j--)
            {
                if (rowIndex + j >= 0 && rowIndex + j < rowCount)
                {
                    for (long m = n; m >= 0; m--)
                    {
                        if (colIndex - m >= 0 && colIndex - m < colCount)
                        {
                            if (rowIndex + j != rowIndex || colIndex - m != colIndex)
                            {
                                string id = string.Format($"{rowIndex + j}_{colIndex - m}");
                                dic.Add(id, this.GridDic[id]);
                            }
                        }
                    }
                    for (long m = n; m > 0; m--)
                    {
                        if (colIndex + m >= 0 && colIndex + m < colCount)
                        {
                            if (rowIndex + j != rowIndex || colIndex + m != colIndex)
                            {
                                string id = string.Format($"{rowIndex + j}_{colIndex + m}");
                                dic.Add(id, this.GridDic[id]);
                            }
                        }
                    }
                }
            }
            return dic;
        }


        /// <summary>
        /// 通过元胞ID设置窗口元胞网格
        /// </summary>
        /// <param name="gateIDs">窗口所在元胞ID</param>
        public void setFoodGate(string[] gateIDs)
        {
            foreach (var item in gateIDs)
            {
                gridDic[item].FuncVar = 1;

                GridDic_foodGate.Add(item, gridDic[item]);
            }
        }

        /// <summary>
        /// 设置障碍物网格
        /// </summary>
        /// <param name="obsIDs">障碍物所在元胞ID</param>
        public void setObstatcle(string[] obsIDs)
        {
            foreach (var item in obsIDs)
            {
                gridDic[item].FuncVar = 6;
            }
        }

        /// <summary>
        /// 设置入口网格
        /// </summary>
        /// <param name="indoorIDs">入口网格ID</param>
        public void setIndoor(string[] indoorIDs)
        {
            foreach (var item in indoorIDs)
            {
                gridDic[item].FuncVar = 3;
                GridDic_indoor.Add(item, gridDic[item]);
            }
        }

        /// <summary>
        /// 设置座位网格
        /// </summary>
        /// <param name="outdoorIDs">出口网格ID</param>
        public void setSeat(string[] seatIDs)
        {
            foreach (var item in seatIDs)
            {
                gridDic[item].FuncVar = 2;
                GridDic_seat.Add(item, gridDic[item]);
            }
        }

        /// <summary>
        /// 设置回收点网格
        /// </summary>
        /// <param name="recyclingIDs">回收点网格ID</param>
        public void setRecycling(string[] recyclingIDs)
        {
            foreach (var item in recyclingIDs)
            {
                gridDic[item].FuncVar = 5;
                GridDic_recycling.Add(item, gridDic[item]);
            }
        }

        /// <summary>
        /// 设置出口网格
        /// </summary>
        /// <param name="outdoorIDs">出口网格ID</param>
        public void setOutdoor(string[] outdoorIDs)
        {
            foreach (var item in outdoorIDs)
            {
                gridDic[item].FuncVar = 4;
                GridDic_outdoor.Add(item, gridDic[item]);
            }
        }
    }

    /// <summary>
    /// 元胞对象类
    /// </summary>
    public class CellClass
    {
        /// <summary>
        /// 元胞ID
        /// </summary>
        private string cellID = "";
        /// <summary>
        /// 是否有人的状态变量. 
        /// 1：有人; 0:无人;
        /// </summary>
        private int statusVar = 0;
        /// <summary>
        /// 功能变量.
        /// 1:窗口等待元胞
        /// 2:座位停留元胞
        /// 3:入口元胞
        /// 4:出口元胞
        /// 5:回收点元胞
        /// 6:障碍物
        /// 0:其它元胞
        /// </summary>
        private int funcVar = 0;
        /// <summary>
        /// 个体患病情况.
        /// 0:
        /// 1: S，健康状态
        /// 2: E，潜伏状态
        /// 3: I, 感染状态
        /// 4: R，康复者
        /// </summary>
        private int infVar = 0;
        /// <summary>
        /// 速度状态
        /// </summary>
        private double speedVar = 0.0;
        /// <summary>
        /// 目标状态
        /// 0：无目标
        /// 1：目标窗口
        /// 15：排队中
        /// 17：处于窗口元胞等待
        /// 2：目标座位
        /// 25：处于作为元胞等待
        /// 3：目标餐具回收点
        /// 35：处于目标餐具回收点等待
        /// 4：目标出口
        /// 45: 到达出口
        /// </summary>
        private int targetVar = 0;

        public CellClass(string cellID, int statusVar, int funcVar, int infVar, double speedVar, int targetVar)
        {
            this.cellID = cellID;
            this.statusVar = statusVar;
            this.funcVar = funcVar;
            this.infVar = infVar;
            this.speedVar = speedVar;
            this.targetVar = targetVar;
        }

        public CellClass() { }

        public string CellID { get => cellID; set => cellID = value; }
        public int StatusVar { get => statusVar; set => statusVar = value; }
        public int FuncVar { get => funcVar; set => funcVar = value; }
        public int InfVar { get => infVar; set => infVar = value; }
        public double SpeedVar { get => speedVar; set => speedVar = value; }
        public int TargetVar { get => targetVar; set => targetVar = value; }
    }
    //
    public class PersonCube
    {
        public long personID;
        public double curTime;
        public string xyStr;
        public double Target;
        public double InfVar;
        public double Infectionp;
        public double stopping;
        public double time;

        public PersonCube(long personID, double curTime, string xyStr, double Target, double InfVar, double Infectionp, double stopping, double time)
        {
            this.personID = personID;
            this.curTime = curTime;
            this.xyStr = xyStr;
            this.Target = Target;
            this.InfVar = InfVar;
            this.Infectionp = Infectionp;
            this.stopping = stopping;
            this.time = time;
        }
    }
}

/**
 * 
 * 
 * */