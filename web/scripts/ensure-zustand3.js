/**
 * @udecode/zustood declares `zustand >=3.5.10` as a peerDependency, so npm
 * resolves it against the root zustand@5 — but zustood is written against
 * the zustand v3 API (default exports from 'zustand' / 'zustand/vanilla',
 * `create(store)` wrapping). npm `overrides` do not apply to peer deps,
 * so we nest a copy of zustand@3.7.2 (the exact version plate-core pins)
 * inside @udecode/zustood so Node/Vite resolve it locally.
 *
 * Runs on every `npm install` (wired as "postinstall" in package.json);
 * no-op when the nested copy already exists.
 */
const fs = require('fs');
const path = require('path');

const nodeModules = path.resolve(__dirname, '..', 'node_modules');
const source = path.join(nodeModules, '@udecode', 'plate-core', 'node_modules', 'zustand');
const dest = path.join(nodeModules, '@udecode', 'zustood', 'node_modules', 'zustand');

function nestedVersion() {
	try {
		return require(path.join(dest, 'package.json')).version;
	} catch {
		return null;
	}
}

const nested = nestedVersion();
if (nested === '3.7.2') {
	process.exit(0);
}

if (!fs.existsSync(source)) {
	console.error(
		'[ensure-zustand3] source zustand@3.7.2 not found at ' + source +
		' — @udecode/zustood will resolve zustand@5 and vite build will fail on its v3 imports.'
	);
	process.exit(1);
}

fs.mkdirSync(path.dirname(dest), { recursive: true });
fs.cpSync(source, dest, { recursive: true });
console.log('[ensure-zustand3] nested zustand@3.7.2 into @udecode/zustood (was: ' + (nested || 'missing') + ')');
