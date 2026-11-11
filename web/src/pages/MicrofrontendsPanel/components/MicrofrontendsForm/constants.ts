export const MICROFRONTEND_FIELD_TITLE = 'Microfrontend';

export const FORM_NAME = 'Microfrontend form';

export const ROUTE_PREFIX = '/';
export const MODULE_PREFIX = './';
export const MIN_MODULE_VALUE_LENGTH = MODULE_PREFIX.length + 1;
export const MIN_FORM_VALUE_LENGTH = 2;

export const initialFormValues = {
	name: '',
	scope: '',
	modules: null,
	routes: null,
	version: '',
	remoteEntries: '',
	claims: null,
};

export const microfrontendsFieldData = {
	name: {
		name: 'name',
		label: 'Name',
	},
	scope: {
		name: 'scope',
		label: 'Scope',
	},
	modules: {
		name: 'modules',
		label: 'Modules',
	},
	routes: {
		name: 'routes',
		label: 'Routes',
	},
	version: {
		name: 'version',
		label: 'Version',
	},
	remoteEntries: {
		name: 'remoteEntries',
		label: 'Remote Entries',
	},
	claims: {
		name: 'claims',
		label: 'Claims',
	},
} as const;

export const mfeConfigValidationErrors = {
	routes: {
		startsWithSlash: `Route must start with ${ROUTE_PREFIX}`,
		minLength: `Route must be at least ${MIN_FORM_VALUE_LENGTH} characters long`,
	},
	remoteEnties: {
		required: 'Remote Entry URL is required',
		invalidUrl: 'Remote Entry URL must be a valid URL',
	},
	modules: {
		startsWithDotSlash: `Module file must start with ${MODULE_PREFIX}`,
		minLength: `Module must be at least ${MIN_MODULE_VALUE_LENGTH} characters long`,
	},
} as const;
