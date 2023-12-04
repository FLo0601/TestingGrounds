function load() {
    getCurrentContainer().then((data) => { setHeaderTitle(data.name); });
    getContainerData().then((data) => { loadContainerDiv(data); });
}

async function getContainerData() {
    let response = await fetch("http://localhost:5678/containers/", {
		method: "GET",
		mode: "cors",
		cache: "no-cache",
		credentials: "omit",
		headers: { "Content-Type": "application/json" },
		redirect: "follow",
		referredPolicy: "no-referrer",
    });
    let responseJSON = await response.json();
    return responseJSON;
}

async function getCurrentContainer() {
    let response = await fetch("http://localhost:5678/container/", {
		method: "GET",
		mode: "cors",
		cache: "no-cache",
		credentials: "omit",
		headers: { "Content-Type": "application/json" },
		redirect: "follow",
		referredPolicy: "no-referrer",
    })
    let responseJSON = await response.json();
    return responseJSON;
}

function setHeaderTitle(name) {
    document.getElementById("containerHeading").innerText = name;
}

function loadContainerDiv(data) {
    for (let i = 0; i < data.length; i++) {
	const elem = document.createElement('div');
	const idCont = document.createTextNode(`Id: ${data[i].id}`);
	elem.appendChild(idCont);
	elem.appendChild(document.createElement("br"))
	const nameCont = document.createTextNode(`Name: ${data[i].name}`);
	elem.appendChild(nameCont);
	elem.appendChild(document.createElement("br"))
	const ipAddrCont = document.createTextNode(`IP: ${data[i].ipAddr}`);
	elem.appendChild(ipAddrCont);
	elem.appendChild(document.createElement("br"))
	document.body.insertAdjacentElement('beforeend', elem)
    }
}
