function openAppSwitcher() {
	M.Modal.getInstance(document.getElementById("app-switcher")).open();
}
function closeAppSwitcher() {
	M.Modal.getInstance(document.getElementById("app-switcher")).close();
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