using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class ConfigFile : MonoBehaviour
{
    private DirectoryInfo confDir;
    private string confPattern = "*.conf";
    public string confSubDir = "/../conf/";

    private void Awake()
    {
        confDir = new DirectoryInfo(Application.dataPath + confSubDir);
    }

    public void SaveConfig(string fileName, string conf)
    {
        File.WriteAllText(confDir + fileName + ".conf", conf);
    }

    public void SaveFile(string fileName, string file)
    {
        File.WriteAllText(confDir + fileName, file);
    }


    public void DeleteAllConfigFiles()
    {
        try
        {
            foreach (var item in confDir.GetFiles(confPattern))
            {
                File.Delete(item.FullName);
            }
        }
        catch
        {
            //exception;
        }
    }

    private void ConfigIter(string pattern, Action<string, string> action)
    {
        if (confDir.Exists)
        {
            foreach (var item in confDir.GetFiles(pattern))
            {
                action(item.Name, File.ReadAllText(confDir + item.Name, System.Text.Encoding.UTF8));
            }
        }
    }




    public Dictionary<string, string> GetConfigDict()
    {
        Dictionary<string, string> configs = new Dictionary<string, string>(); ;
        ConfigIter(confPattern, (n, c) => ReadConfig(c, ref configs));
        return configs;
    }


    public static string ToStringConfig(Dictionary<string, string> configDic)
    {

        string strConfig = null;

        Dictionary<string, Dictionary<string, string>> sectionConfigDic = new Dictionary<string, Dictionary<string, string>>();

        {
            string[] sectionKeyPair = null;
            char[] dot = { '.' };
            foreach (KeyValuePair<string, string> valuePair in configDic)
            {
                sectionKeyPair = valuePair.Key.Split(dot, 2);

                if (sectionKeyPair.Length == 2)
                {
                    if (sectionConfigDic.ContainsKey(sectionKeyPair[0]))
                    {
                        sectionConfigDic[sectionKeyPair[0]].Add(sectionKeyPair[1], valuePair.Value);
                    }
                    else
                    {
                        sectionConfigDic[sectionKeyPair[0]] = new Dictionary<string, string>() { { sectionKeyPair[1], valuePair.Value } };
                    }
                }
            }
        }


        StringWriter strWriter = new StringWriter();
        try
        {

            foreach (KeyValuePair<string, Dictionary<string, string>> sectionPair in sectionConfigDic)
            {

                if (sectionPair.Value.Count > 0)
                {
                    strWriter.WriteLine();
                    strWriter.Write('[');
                    strWriter.Write(sectionPair.Key);
                    strWriter.WriteLine(']');

                    foreach (KeyValuePair<string, string> valuePair in sectionPair.Value)
                    {
                        strWriter.Write(valuePair.Key);
                        strWriter.Write('=');
                        strWriter.WriteLine(valuePair.Value);
                    }

                }
            }
            strConfig = strWriter.ToString();
        }
        finally
        {
            sectionConfigDic.Clear();
            sectionConfigDic = null;
            strWriter.Close();
            strWriter = null;
        }

        return strConfig;
    }




    private bool ReadConfig(string iniString, ref Dictionary<string, string> config)
    {
        StringReader strReader = new StringReader(iniString);
        try
        {
            char[] equal = { '=' };
            string currentSectionName = null;
            string str;
            string sectionName;
            string[] valuePair;
            while ((str = strReader.ReadLine()) != null)
            {
                str = str.Trim();
                sectionName = ParseSectionName(str);
                if (sectionName != null)
                {
                    currentSectionName = sectionName;
                    continue;
                }
                else if (currentSectionName != null)
                {
                    valuePair = str.Split(equal, 2);
                    if (valuePair.Length == 2)
                    {
                        config[currentSectionName + "." + valuePair[0]] = valuePair[1];
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            strReader.Close();
            strReader = null;
        }

        return true;
    }


    private string ParseSectionName(string line)
    {
        if (!line.StartsWith("[")) return null;
        if (!line.EndsWith("]")) return null;
        if (line.Length < 3) return null;
        return line.Substring(1, line.Length - 2);
    }



}
