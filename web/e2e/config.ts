export const CREDENTIALS = {
	CEO: {
		email: 'modsen.portal.test.ceo@modsen-software.com',
		password: 'Qwerty*1*2*3!',
	},
	CTO: {
		email: 'modsen.portal.test.cto@modsen-software.com',
		password: 'Qwerty*1*2*3!',
	},
	HD: {
		email: 'modsen.portal.test.headofdepartment@modsen-software.com',
		password: 'Qwerty*1*2*3!',
	},
	SE: {
		email: 'modsen.portal.test.employee@modsen-software.com',
		password: 'Qwerty*1*2*3!',
	},
	SA: {
		email: 'modsen.portal.test.sysadmin@modsen-software.com',
		password: 'Qwerty*1*2*3!',
	},
	GM: {
		email: 'modsen.portal.test.groupmanager@modsen-software.com',
		password: 'Qwerty*1*2*3!#',
	},
} as const;

export const TIMEOUT = {
	XS2: 200,
	XS: 300,
	S: 500,
	M: 1500,
	L: 5000,
	XL: 10000,
	XXL: 60000,
} as const;

export const AUTH_HOST = 'microsoftonline.com';
