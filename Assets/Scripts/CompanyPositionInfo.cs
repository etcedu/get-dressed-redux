using UnityEngine;
using System.Collections;

[System.Serializable]
public class CompanyPositionInfo
{
	[SerializeField]
	private string companyName = "Default Company Name";
	public string CompanyName
	{
		get{ return companyName; }
		set{ companyName = value; }
	}
	[SerializeField]
	private string positionName = "Default Position";
	public string PositionName 
	{
		get{ return positionName; }
		set{ positionName = value; }
	}
	[SerializeField]
	private Clothing.Tier.TierEnum positionDressTier = Clothing.Tier.TierEnum.BUSINESS_CASUAL;
	public Clothing.Tier.TierEnum PositionDressTier
	{
		get{ return positionDressTier; }
		set{ positionDressTier = value; }
	}
	[SerializeField]
	private string positionInfo = "Default Info";
	public string PositionInfo
	{
		get{ return positionInfo; }
		set{ positionInfo = value; }
	}
	
	public CompanyPositionInfo()
	{
		this.companyName = "";
		this.positionName = "";
		this.positionInfo = "";
	}
	public CompanyPositionInfo(string companyName, string positionName)
	{
		this.companyName = companyName;
		this.positionName = positionName;
	}
}

