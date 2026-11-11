export const MICROFRONTENDS_SHARED_SCOPE = {
	react: {
		'19.2.4': {
			get: () => import('react').then((m) => () => m),
			loaded: true,
		},
	},
	'react-dom': {
		'19.2.4': {
			get: () => import('react-dom').then((m) => () => m),
			loaded: true,
		},
	},
	'@emotion/react': {
		'11.14.0': {
			get: () => import('@emotion/react').then((m) => () => m),
			loaded: true,
		},
	},
	'@tanstack/react-query': {
		'5.74.3': {
			get: () => import('@tanstack/react-query').then((m) => () => m),
			loaded: true,
		},
	},
	axios: {
		'1.13.2': {
			get: () => import('axios').then((m) => () => m),
			loaded: true,
		},
	},
	'@mui/material': {
		'5.16.14': {
			get: () => import('@mui/material').then((m) => () => m),
			loaded: true,
		},
	},
};
