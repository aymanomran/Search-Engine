<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchForm.aspx.cs" Inherits="Search_Engine.SearchForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<style type="text/css">
    body {
    background-image:url('searchEngin.png')
        background-repeat: no-repeat;

    }
        #searchtxtboxID {
            margin-left: 0px;
        }
        #searchtxtboxID {
        border:3px solid ;
        }
        #SearchingResultsDiv {
       width:400px;
       height:2000px;
        }

    </style>
</head>

<body>
   <form id="form1" runat="server">
    <div>
    
&nbsp;
        <asp:Image ID="Image1" ImageUrl="~/google.jpg"  runat="server" Width="663px" Height="62px" />
       
    
    </div>

        <asp:TextBox ID="SearchTxt" runat="server" Width="500px" Height="30px"></asp:TextBox>
        <asp:Button ID="SearchBtn" runat="server" Height="30px" Text="Search" OnClick="SearchBtn_Click" Width="112px" />
        <br />
        <asp:CheckBox ID="SpellCorrectionCB" AutoPostBack="True" runat="server" Text="Spell Correction" OnCheckedChanged="SpellCorrectionCB_CheckedChanged" />
        <br />
        <asp:CheckBox ID="SoundxID" AutoPostBack="True" runat="server" Text="SoundxCB" OnCheckedChanged="SoundxID_CheckedChanged" />
        <br />
        <asp:Label ID="DidUMeanLabelID" runat="server" ForeColor="Red"></asp:Label>
        <br />
        <asp:ListBox ID="DidUMeanLBID" AutoPostBack="True" AutoEventWireup="true" runat="server" Height="77px" Width="365px" OnSelectedIndexChanged="DidUMeanLBID_SelectedIndexChanged"></asp:ListBox>
        <asp:ListBox ID="ResultsBoxID" AutoPostBack="True" AutoEventWireup="true" runat="server" Width="397px" style="margin-top: 0px" OnSelectedIndexChanged="ResultsBoxID_SelectedIndexChanged" Visible="False"></asp:ListBox>
 <div id="SearchingResultsDiv" runat="server">


 </div>
           </form>
</body>
</html>
