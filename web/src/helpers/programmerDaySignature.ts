export const PROGRAMMER_DAY_SIGNATURE_SECRET =
	'7qEG(Ip&5mHv?;LBZ>fN^kU+Ghu9hoG?';

export async function createProgrammerDaySignature(
	userId: number,
	score: number,
	timestamp: number,
	secret: string
) {
	const encoder = new TextEncoder();
	const payload = `${userId}:${score}:${timestamp}`;
	const key = await crypto.subtle.importKey(
		'raw',
		encoder.encode(secret),
		{
			name: 'HMAC',
			hash: 'SHA-256',
		},
		false,
		['sign']
	);
	const hashBuffer = await crypto.subtle.sign(
		'HMAC',
		key,
		encoder.encode(payload)
	);

	return Array.from(new Uint8Array(hashBuffer))
		.map((byte) => byte.toString(16).padStart(2, '0'))
		.join('')
		.toUpperCase();
}
