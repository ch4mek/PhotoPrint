import { Theme } from '@emotion/react';

import { MerchStatuses } from '../../constants';

import { ElementsTabColumnsPropsFilters } from './types';

export const TWO_COLUMNS_GRID_CORRECTION = 2;
export const FIRST_ROWS_HEIGHT = 48;
export const FIRST_ROW_CONTENT_MAX_HEIGHT = 40;
export const EXPANDED_ROWS_HEIGHT = 24;
export const EXPANDED_STORAGE_HEIGHT = 192;
const FULL_PHOTO_SIZE = 350;
const FULL_PHOTO_MIN_SIZE = 308;
const FULL_PHOTO_BORDER_RADIUS = 10;

export const ELEMENT_TAB_COLUMNS_WIDTH = {
	id: {
		width: 67,
	},
	name: {
		width: 250,
		maxWidth: 350,
	},
	storage: {
		width: 168,
	},
	size: {
		width: 196,
	},
	minAmount: {
		width: 100,
	},
	status: {
		width: 88,
		maxWidth: 150,
	},
	supplier: {
		width: 200,
		fixed: true,
	},
	description: {
		width: 260,
	},
};

export const ELEMENTS_TAB_GRID_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	name: {
		key: 'name',
		name: 'Name',
		sortable: true,
	},
	storage: {
		key: 'storage',
		name: 'Storage',
		sortable: true,
	},
	size: {
		key: 'size',
		name: 'Size',
		sortable: false,
	},
	minAmount: {
		key: 'minAmount',
		name: 'Min. amount',
		sortable: true,
	},
	status: {
		key: 'status',
		name: 'Status',
		sortable: true,
	},
	supplier: {
		key: 'supplier',
		name: 'Supplier',
		sortable: true,
	},
	description: {
		key: 'description',
		name: 'Description',
		sortable: false,
	},
};

export const DEFAULT_MERCH_ELEMENTS_GRID_FILTERS: ElementsTabColumnsPropsFilters =
	{
		idFilter: '',
		nameFilter: '',
		statusFilter: [MerchStatuses.available],
		descriptionFilter: '',
		storageFilter: [],
		supplierFilter: '',
	};

export const getGridPhotoStyleConfig = (theme: Theme) => ({
	width: FIRST_ROW_CONTENT_MAX_HEIGHT,
	height: FIRST_ROW_CONTENT_MAX_HEIGHT,
	borderRadius: theme.spaces.xs,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	backgroundColor: theme.palette['background-white'],
});

export const getFullPhotoStyleConfig = (theme: Theme) => ({
	width: FULL_PHOTO_SIZE,
	minWidth: FULL_PHOTO_MIN_SIZE,
	height: FULL_PHOTO_SIZE,
	borderRadius: FULL_PHOTO_BORDER_RADIUS,
	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
});
