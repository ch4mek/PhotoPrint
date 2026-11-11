export const CLIENT_PROJECTS_COLUMN_WIDTHS = {
	client: {
		width: 240,
		maxWidth: 350,
	},
	factTime: {
		width: 121,
		maxWidth: 137,
	},
	billableTime: {
		width: 137,
		fixed: true,
	},
};

export const HEAD_CLIENT_PROJECTS_COLUMN_WIDTHS = {
	...CLIENT_PROJECTS_COLUMN_WIDTHS,
	factTime: {
		width: 0,
		fixed: false,
	},
	billableTime: {
		width: 0,
		fixed: false,
	},
};

export const CELL_DIVIDER_HEIGHT = 1;
export const CELL_SCROLLBAR_HEIGHT = 6;
