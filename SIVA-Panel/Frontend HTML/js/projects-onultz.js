function exitModal(){
	M.Modal.getInstance(document.getElementById("exit")).open();
}

function openProfile() {
	if (window.getComputedStyle(document.getElementById("profile"), null).getPropertyValue("display") != "none"){
		M.Sidenav.getInstance(document.getElementById("mobileprofile")).open();
	}else{
		M.Dropdown.getInstance(document.getElementById("pcprofile")).open();
	}
}

function closeProfile(){
	M.Dropdown.getInstance(document.getElementById("pcprofile")).close();
	M.Sidenav.getInstance(document.getElementById("mobileprofile")).close();
}