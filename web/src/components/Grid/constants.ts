// eslint-disable-next-line import/no-extraneous-dependencies
import { BasePlacement, VariationPlacement } from '@popperjs/core';

export const CLIENTS_PROJECT_GRID_ROW_HEIGHT = 24;
export const DEFAULT_GRID_SKELETON_ROWS_AMOUNT = 50;
export const DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT = 22;

export const GRID_TIME_CELL_ROW_HEIGHT = 72;
export const GRID_ROW_WITH_ADD_BUTTON_HEIGHT = 50;
export const GRID_ROW_WITH_ADD_BUTTON_HEIGHT_NARROW_VIEWPORT = 80;

export const EDIT_COLUMN_HEADER_CLASS = 'edit-column-header';
export const EDIT_COLUMN_CELL_CLASS = 'edit-column-cell';
export const EDIT_COLUMN_SUMMARY_CLASS = 'edit-column-summary';
export const RELATIVE_EDIT_COLUMN_CELL_CLASS = 'relative-edit-column-cell';
export const DYNAMIC_CELL_CLASS = 'dynamic-rgd-cell';
export const CROSS_LINK_CELL_CLASS = 'cross-link-cell';
export const STATIC_CROSS_LINK_CELL_CLASS = 'static-cross-link-cell';

export const CELL_FONT_SIZE = 14;
export const SMALL_COLOR_CIRCLE_SIZE = 10;
export const CLIENT_PROJECTS_CELL_FONT_SIZE = 12;

export const EDIT_ICON_CLASS_NAME = 'edit-icon';

export const EDIT_COLUMN_WIDTH = 37;
const ID_COLUMN_WIDTH = 67;

const GRID_HEADER_ROW_HEIGHT = 28;
export const GRID_ROW_HEIGHT = 26;
const GRID_PAGINATION_HEIGHT = 52;
export const HEADER_PAGINATION_HEIGHT =
	GRID_HEADER_ROW_HEIGHT + GRID_PAGINATION_HEIGHT;

export const COMMON_GRID_PROPS = {
	headerRowHeight: GRID_HEADER_ROW_HEIGHT,
	rowHeight: GRID_ROW_HEIGHT,
};

const DEFAULT_ID_COLUMN_WIDTH = 72;
export const DEFAULT_ID_COLUMN_CONFIG = {
	width: DEFAULT_ID_COLUMN_WIDTH,
	fixed: true,
};

export const defaultIdColumnWidth = {
	width: ID_COLUMN_WIDTH,
	fixed: true,
};

export const DEFAULT_ID_COLUMN_WITH_COLOR_WIDTH = 80;
export const MODAL_WIDTH = 244;

export const TOOLTIP_FALLBACK_PLACEMENTS: Array<
	BasePlacement | VariationPlacement
> = [
	'top',
	'right',
	'left',
	'bottom',
	'top-start',
	'top-end',
	'bottom-start',
	'bottom-end',
	'left-start',
	'left-end',
	'right-start',
	'right-end',
];

export const IGNORE_SELECTION_CLASS_NAME = 'ignore-selection';

export const EDIT_CELL_RIGHT_PADDING = 10;

export const displayMinimalGridRowsProp = { minRowsCount: 1 };

export const EMPTY_GRID_CELL_VALUE = '';

export const WIDGET_GRID_HEIGHT = 303;

export enum CellTextCutOrientation {
	Vertical = 'vertical',
	Horizontal = 'horizontal',
}

export enum CellCrosslinkType {
	Dynamic = 'dynamic',
	Static = 'static',
}

export const TOOLTIP_SHOW_DELAY = 100;
