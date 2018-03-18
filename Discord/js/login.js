function getGET(name) {
	const regex = new RegExp(`[?&]${encodeURIComponent(name)}=([^&]*)`);
	const output = regex.exec(location.search);
	if (output[1]) return decodeURIComponent(output[1]);
	return false;
}

if (getGET('error')) {
	const err = document.getElementById('error');
	err.innerHTML = getGET('error');
}
