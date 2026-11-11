export const MICROFRONTENDS_PAGE_TITLE = 'Microfrontends Configuration';

export const MICROFRONTENDS_TAB_GRID_COLUMNS = {
	name: {
		key: 'name',
		name: 'Name',
		sortable: true,
	},
	scope: {
		key: 'scope',
		name: 'Scope',
		sortable: true,
	},
	modules: {
		key: 'modules',
		name: 'Modules',
		sortable: true,
	},
	remoteEntries: {
		key: 'remoteEntries',
		name: 'Remote Entry',
		sortable: true,
	},
	routes: {
		key: 'routes',
		name: 'Routes',
		sortable: false,
	},
	claims: {
		key: 'claims',
		name: 'Claims',
		sortable: false,
	},
	version: {
		key: 'version',
		name: 'Version',
		sortable: true,
	},
	actions: {
		key: 'actions',
		name: 'Actions',
		sortable: false,
	},
} as const;

export const MICROFRONTENDS_TABLE_COLUMNS_WIDTH = {
	name: {
		width: 200,
		maxWidth: 200,
		fixed: true,
	},
	scope: {
		width: 200,
		fixed: false,
	},
	modules: {
		width: 120,
		fixed: false,
	},
	remoteEntries: {
		width: 250,
		fixed: false,
	},
	routes: {
		width: 200,
		fixed: false,
	},
	claims: {
		width: 200,
		fixed: false,
	},
	version: {
		width: 80,
		fixed: true,
	},
	actions: {
		width: 60,
		fixed: true,
	},
};

export const MicrofrontendsEditCellOptionValues = {
	edit: 'Edit',
	delete: 'Delete',
};
