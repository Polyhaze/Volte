// Make a HTML array to store all the relevant HTML elements.
const html = [];

// Find the following IDs in the document
const parts = [
	'stats',
	'guilds',
	'modal',
	'percentagebots',
	'minimumusers',
	'users',
	'guilds'
];

const graphs = [
	'user',
	'guild'
];

const canvas = {};

// Find every relevant element that needs to be used.
parts.forEach((part) => {
	html[part] = document.getElementById(part);
});

const chartconfig = {
	responsive: true,
	grid: {
		fillStyle: '#ffffff',
		strokeStyle: '#aaaaaa',
		millisPerLine: 6000,
	},
	labels: {
		fillStyle: '#000000'
	},
	millisPerPixel: 100,
	verticalSections: 0,
	timestampFormatter: SmoothieChart.timeFormatter
};

const lineconfig = {
	lineWidth: 2,
	strokeStyle: '#7000FB',
	fillStyle: 'rgba(0,0,255,0.28)'
};

graphs.forEach((graph) => {
	console.log(graph);
	const chart = new SmoothieChart(chartconfig);
	canvas[graph] = new TimeSeries();
	chart.addTimeSeries(canvas[graph], lineconfig);
	chart.streamTo(document.getElementById(`${graph}chart`), 500);
});

const get = (name) => {
	const regex = new RegExp(`[?&]${encodeURIComponent(name)}=([^&]*)`);
	const output = regex.exec(location.search);
	if (output && output[1]) return decodeURIComponent(output[1]);
	return false;
};
const botStats = bot => ({
	users: bot.users.filter(guildMember => !guildMember.bot).size,
	bots: bot.users.filter(guildMember => guildMember.bot).size,
	total: bot.users.size,
	guilds: bot.guilds.size
});
const guildsStats = bot => ({
	guilds: bot.guilds.map(guild => ({
		name: guild.name,
		id: guild.id,
		ownerId: guild.owner.id,
		users: guild.members.filter(guildMember => !guildMember.user.bot).size,
		bots: guild.members.filter(guildMember => guildMember.user.bot).size,
		total: guild.members.size,
		percentage: Math.floor((guild.members.filter(guildMember => guildMember.user.bot).size / guild.members.size) * 100)
	})).sort((a, b) =>
		b.percentage - a.percentage
	) });
const guildStats = (bot, guildid) => {
	const guild = bot.guilds.get(guildid);
	return {
		name: guild.name,
		id: guild.id,
		ownerId: guild.owner.id,
		users: guild.members.filter(guildMember => !guildMember.user.bot).size,
		bots: guild.members.filter(guildMember => guildMember.user.bot).size,
		total: guild.members.size,
		percentage: Math.floor((guild.members.filter(guildMember => guildMember.user.bot).size / guild.members.size) * 100),
		textchannels: guild.channels.filterArray(channel => channel.type === 'text'),
		owner: guild.owner
	};
};
const renderBotStats = (bot) => {
	html.stats.innerHTML = Mustache.render(`
			<span>Users: {{ users }}</span><br>
			<span>Bots: {{ bots }}</span><br>
			<span>Total: {{ total }}</span><br>
			<span>Guilds: {{ guilds }}</span><br>
		`, botStats(bot));
};
const renderGuildsStats = (bot) => {
	html.guilds.innerHTML = Mustache.render(`
			{{ #guilds }}
			<li>
				<div class="collapsible-header">{{ name }}</div>
				<div class="collapsible-body">
					<span>ID: {{ id }}</span><br>
					<span>Owner ID: {{ ownerId }}</span><br>
					<span>Users: {{ users }}</span><br>
					<span>Bots: {{ bots }}</span><br>
					<span>Total: {{ total }}</span><br>
					<span>Percentage of bots: {{ percentage }}%</span><br>
					<a class="waves-effect waves-light btn red" onclick="removeGuild(client, '{{ id }}')">Leave</a>
					<a class="waves-effect waves-light btn" onclick="messageModal(client, '{{ id }}')">Message</a>
				</div>
			</li>
			{{ /guilds }}
		`, guildsStats(bot));
};
const removeGuild = (bot, guildid) => { // eslint-disable-line no-unused-vars
	bot.guilds.get(guildid).leave()
		.then((g) => {
			renderBotStats(bot);
			renderGuildsStats(bot);
			Materialize.toast(`Left ${g.name}`, 4000);
		});
};
const removeModal = (bot, minimum, percentage) => { // eslint-disable-line no-unused-vars
	const info = {
		guilds: guildsStats(bot).guilds.filter(guild => guild.total > minimum && guild.percentage > percentage),
		count: guildsStats(bot).guilds.filter(guild => guild.total > minimum && guild.percentage > percentage).length,
		minimum,
		percentage
	};

	if (info.count) {
		html.modal.innerHTML = Mustache.render(`
			<div class="modal-content">
				<h4>Prune guilds with more than {{ percentage }}% bots and at least {{ minimum }} total accounts</h4>
				<p>This action will get rid of the following {{ count }} guild(s).</p>
				<ul>
					{{ #guilds }}
					<li>{{ name }} with {{ percentage }}% bots.</li>
					{{ /guilds }}
				</ul>
			</div>
			<div class="modal-footer">
				<a class="modal-action modal-close waves-effect waves-light btn-flat">Cancel</a>
				<a class="modal-action modal-close waves-effect waves-light btn-flat red" onclick="pruneBots(client, {{ minimum }}, {{ percentage }})">Ban</a>
			</div>
		`, info);
		$('#modal').modal('open');
	} else {
		Materialize.toast('No guilds fit the criteria.', 4000);
	}
};
const messageModal = (bot, guildid) => { // eslint-disable-line no-unused-vars
	const stats = guildStats(bot, guildid);
	if (stats.owner && stats.users === 0) {
		Materialize.toast('There is nobody to message! (No owner or users)', 4000);
	} else {
		html.modal.innerHTML = Mustache.render(`
			<div class="modal-content">
				<h4>Send message to {{ name }}</h4>
				<div class="input-field col s12">
					<textarea id="message" class="materialize-textarea"></textarea>
					<label for="message">Message</label>
				</div>
				<div class="input-field col s12">
					<select class="browser-default" id="location">
						<option value="none" disabled selected>Select a channel</option>
						<optgroup label="Locations">
							<option value="owner" {{ ^owner }}disabled{{ /owner }}>Owner</option>
							<option value="default">Default Channel</option>
						</optgroup>
						<optgroup label="Channels">
							{{ #textchannels }}
							<option value="{{ id }}">#{{ name }}</option>
							{{ /textchannels }}
						</optgroup>
					</select>
				</div>
			</div>
			<div class="modal-footer">
				<a class="modal-action modal-close waves-effect waves-light btn-flat">Cancel</a>
				<a class="modal-action waves-effect waves-green btn-flat" onclick="messageGuild(client, '{{ id }}', document.getElementById('location').value, document.getElementById('message').value)">Send</a>
			</div>
		`, stats);
		$('#modal').modal('open');
	}
};
const usernameModal = () => { // eslint-disable-line no-unused-vars
	html.modal.innerHTML = `
		<div class="modal-content">
			<h4>Set username</h4>
			<div class="input-field col s12">
				<input id="username" type="text">
				<label for="username">Message</label>
			</div>
		</div>
		<div class="modal-footer">
			<a class="modal-action modal-close waves-effect waves-light btn-flat">Cancel</a>
			<a class="modal-action modal-close waves-effect waves-green btn-flat" onclick="changeUsername(client, document.getElementById('username').value)">Set Username</a>
		</div>
	`;
	$('#modal').modal('open');
};
const avatarModal = () => { // eslint-disable-line no-unused-vars
	html.modal.innerHTML = `
		<div class="modal-content">
			<h4>Set avatar</h4>
			<div class="file-field input-field col s12 m8">
				<div class="btn">
					<span>File</span>
					<input type="file" onchange="previewAvatar(document.getElementById('avatarPreview'), document.getElementById('file').files[0])" id="file">
				</div>
				<div class="file-path-wrapper">
					<input class="file-path validate" type="text">
				</div>
			</div>
			<div class="col s12 m4 center">
				<img src="https://discordapp.com/assets/2c21aeda16de354ba5334551a883b481.png" style="height: 100px" alt="Preview" id="avatarPreview">
			</div>
		</div>
		<div class="modal-footer">
			<a class="modal-action modal-close waves-effect waves-light btn-flat">Cancel</a>
			<a class="modal-action modal-close waves-effect waves-green btn-flat" onclick="changeAvatar(client, document.getElementById('file').files[0])">Set Profile</a>
		</div>
	`;
	$('#modal').modal('open');
};
const previewAvatar = (img, file) => { // eslint-disable-line no-unused-vars
	const reader = new FileReader();

	reader.addEventListener('load', () => {
		img.src = reader.result;
	}, false);

	if (file) {
		reader.readAsDataURL(file);
	}
};
const messageGuild = (bot, guildid, where, text) => { // eslint-disable-line no-unused-vars
	const guild = bot.guilds.get(guildid);

	if (where === 'none') {
		Materialize.toast('Please select a guild', 4000);
	} else if (!guild) {
		Materialize.toast('This guild does not exist.', 4000);
		$('#modal').modal('close');
	} else if (where === 'owner') {
		if (guild.owner) {
			guild.owner.createDM().then((dm) => {
				dm.send(text)
					.then(() => {
						Materialize.toast('Sent message to owner', 4000);
					})
					.catch((err) => {
						Materialize.toast(`Error in sending message: ${err.message}`, 4000);
					});
			});
		} else {
			Materialize.toast('This guild doesn\'t have an owner', 4000);
		}
		$('#modal').modal('close');
	} else {
		let channel = null;
		if (where === 'default') {
			channel = guild.defaultChannel;
		} else {
			channel = guild.channels.get(where);
		}

		if (channel) {
			channel.send(text)
				.catch((err) => {
					Materialize.toast(err.message, 4000);
				});
		} else {
			Materialize.toast('This guild doesn\'t have this channel', 4000);
		}
		$('#modal').modal('close');
	}
};
const changeUsername = (bot, text) => { // eslint-disable-line no-unused-vars
	if (!text) {
		Materialize.toast('Did not change username', 4000);
	} else {
		bot.user.setUsername(text)
			.then((user) => {
				Materialize.toast(`Changed username to ${user.username}`, 4000);
			})
			.catch((err) => {
				Materialize.toast(`Error in changing username: ${err.message}`, 4000);
			});
	}
};
const changeAvatar = (bot, file) => { // eslint-disable-line no-unused-vars
	if (!file) {
		Materialize.toast('Did not change profile', 4000);
	} else {
		const reader = new FileReader();
		reader.readAsDataURL(file);

		reader.addEventListener('load', () => {
			console.dir(reader.result);

			bot.user.setAvatar(reader.result)
				.then(() => {
					Materialize.toast('Changed avatar', 4000);
				})
				.catch((err) => {
					Materialize.toast(`Error in changing avatar: ${err.message}`, 4000);
				});
		}, false);
	}
};
const pruneBots = (bot, minimum, percentage) => { // eslint-disable-line no-unused-vars
	guildsStats(bot).guilds.filter(guild => guild.total > minimum && guild.percentage > percentage).forEach((guild) => {
		removeGuild(bot, guild.id);
	});
};

// Include Discord
const client = new Discord.Client();
if (!get('token')) window.location.href = 'index.html?error=Error:%20Login%20details%20were%20not%20provided.';

client.login(get('token'))
	.catch((error) => {
		window.location.href = `index.html?error=${encodeURIComponent(error)}`;
	});

// Wait until the client is ready
client.on('ready', () => {
	Materialize.toast('Successfully connected to Discord!', 4000);
	renderBotStats(client);
	renderGuildsStats(client);
	setInterval(() => {
		canvas.user.append(new Date(), client.users.size);
		canvas.guild.append(new Date(), client.guilds.size);
	}, 100);
});
