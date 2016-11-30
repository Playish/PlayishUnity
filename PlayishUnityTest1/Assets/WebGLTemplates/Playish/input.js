var vTrue = "true";
var vFalse = "false";

var up = "false";
var down = "false";
var left = "false";
var right = "false";
var a = "false";
var b = "false";

var data = "";			

function SetData()
{
	data = "input_update#test#button;button;" + up
}

$(document).keydown(function(e)
{

	if(e.key === "ArrowUp")
	{
		up = vTrue;	
	}
	if(e.key === "ArrowDown")
	{
		down = vTrue;	
	}
	if(e.key === "ArrowLeft")
	{
		left = vTrue;	
	}
	if(e.key === "ArrowRight")
	{
		right = vTrue;	
	}
	if(e.key === "a")
	{
		a = vTrue;	
	}
	if(e.key === "b")
	{
		b = vTrue;	
	}

	SetData();
	
	SendMessage("PlayishManager", "UpdateController", data);
});

$(document).keyup(function(e)
{

	if(e.key === "ArrowUp")
	{
		up = vFalse;	
	}
	if(e.key === "ArrowDown")
	{
		down = vFalse;	
	}
	if(e.key === "ArrowLeft")
	{
		left = vFalse;	
	}
	if(e.key === "ArrowRight")
	{
		right = vFalse;	
	}
	if(e.key === "a")
	{
		a = vFalse;	
	}
	if(e.key === "b")
	{
		b = vFalse;	
	}

	SetData();
	
	SendMessage("PlayishManager", "R_ControllerInput", data);
});