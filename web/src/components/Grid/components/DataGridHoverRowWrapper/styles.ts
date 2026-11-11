import styled from '@emotion/styled';
import { DEFAULT_GRID_PAGINATION_HEIGHT } from '@mms/mms-ui-library/constants/grid';

import { EDIT_COLUMN_CELL_CLASS } from '../../constants';

import {
	createActiveBorderColor,
	getHoverBackgroundColor,
} from '@/helpers/getColor';

export const DataGridHoverRowWrapper = styled('div')<{
	defineHeight?: boolean;
	flexGrow?: boolean;
	defineGridHeight?: boolean;
	withoutBackground?: boolean;
}>(
	({
		theme,
		defineHeight = true,
		flexGrow = false,
		defineGridHeight = false,
		withoutBackground = false,
	}) => ({
		height: '100%',

		'@media (hover:hover)': {
			'.rdg-row:hover > .rdg-cell ': {
				backgroundColor: `${
					!withoutBackground ? getHoverBackgroundColor(theme) : ''
				} !important`,
			},

			'.rdg-row:hover .rdg-cell:not([role="columnheader"])': {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			'.rdg-row:has(+ .rdg-row:hover) .rdg-cell:not([role="columnheader"])': {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			[`.rdg-row:hover .rdg-cell:${EDIT_COLUMN_CELL_CLASS}`]: {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			[`.rdg-row:has(+ .rdg-row:hover) .rdg-cell:${EDIT_COLUMN_CELL_CLASS})`]: {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},
		},

		'@media (hover:none)': {
			'.rdg-row:active > .rdg-cell ': {
				backgroundColor: `${getHoverBackgroundColor(theme)}`,
			},

			'.rdg-row:active .rdg-cell:not([role="columnheader"])': {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			'.rdg-row:has(+ .rdg-row:active) .rdg-cell:not([role="columnheader"])': {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			[`.rdg-row:active .rdg-cell:${EDIT_COLUMN_CELL_CLASS}`]: {
				'&:after': {
					background: `${createActiveBorderColor(theme)}`,
				},
			},

			[`.rdg-row:has(+ .rdg-row:active) .rdg-cell:${EDIT_COLUMN_CELL_CLASS})`]:
				{
					'&:after': {
						background: `${createActiveBorderColor(theme)}`,
					},
				},
		},

		'& > div': {
			height: defineHeight
				? `calc(100% - ${DEFAULT_GRID_PAGINATION_HEIGHT}px)`
				: `100%`,
		},

		'& > div > .rdg': {
			height: defineGridHeight
				? `calc(100% - ${DEFAULT_GRID_PAGINATION_HEIGHT}px)`
				: undefined,
		},

		flexGrow: flexGrow ? 1 : undefined,
		minHeight: flexGrow ? 0 : undefined,
	})
);
