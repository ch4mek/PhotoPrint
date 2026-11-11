import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import {
	DATA_GRID_WRAPPER_CLASS_NAME,
	DEFAULT_GRID_HEADER_ROW_HEIGHT,
	DEFAULT_GRID_PAGINATION_HEIGHT,
} from '@mms/mms-ui-library/constants/grid';

import { ThemeMode } from '../../theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';
import {
	getEvenRowBackgroundColor,
	getHoverBackgroundColor,
	getOddRowBackgroundColor,
} from '@/helpers/getColor';
import { NAME_CELL_BACKGROUND } from '@/pages/Settings/components/ClaimsTabs/constants';

import {
	CELL_FONT_SIZE,
	COMMON_GRID_PROPS,
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
	EDIT_COLUMN_SUMMARY_CLASS,
	EDIT_COLUMN_WIDTH,
	RELATIVE_EDIT_COLUMN_CELL_CLASS,
	CROSS_LINK_CELL_CLASS,
	STATIC_CROSS_LINK_CELL_CLASS,
} from './constants';

export const cellBottomBorderStyles = (background: string) => css`
	content: '';
	position: absolute;
	left: 0;
	bottom: 0;
	height: 1px;
	width: 100%;
	background: ${background};
`;

export const overrideGridStyles =
	(
		isPaginationShown: boolean,
		templateColumns?: string,
		setBorderBetweenRows = true
	) =>
	(theme: Theme) =>
		css`
			${templateColumns}

			height: calc(100% - ${DEFAULT_GRID_PAGINATION_HEIGHT}px);
			border-top-color: ${theme.palette['graphics-input-stroke']};

			.rdg-cell[role='columnheader'],
			.rdg-cell[role='columnheader'] div {
				background-color: ${theme.mode === ThemeMode.LIGHT
					? theme.palette['background-primary-1']
					: theme.palette['background-base-3']};
				... {
					${theme.typography.body['s-400']}
				}
				height: ${DEFAULT_GRID_HEADER_ROW_HEIGHT}px;
			}

			${isPaginationShown ? '' : 'block-size: 100%;'}

			.rdg-row {
				&-odd {
					background-color: ${getOddRowBackgroundColor(theme)};
				}

				&-even {
					background-color: ${getEvenRowBackgroundColor(theme)};
				}

				:hover .${CROSS_LINK_CELL_CLASS} {
					text-decoration: underline;
					cursor: pointer;
				}

				.${STATIC_CROSS_LINK_CELL_CLASS} {
					text-decoration: underline;
					cursor: pointer;
				}
			}

			.rdg-cell {
				font-size: ${CELL_FONT_SIZE}px;
				color: ${theme.palette['text-black']};
				padding: 0 ${theme.spaces.s}px 0 ${theme.spaces.s}px;

				&[aria-selected] {
					outline: unset;
				}

				&-array {
					overflow: initial;
				}

				&:not([role='columnheader']) {
					border-right: 1px solid ${theme.palette['graphics-divider-1']};
				}
			}
			::-webkit-scrollbar-track:vertical {
				margin-top: ${COMMON_GRID_PROPS.headerRowHeight}px;
				border-radius: ${BorderRadiuses.none}px;
				border-left: 1px solid ${theme.palette['graphics-divider-1']};
			}

			.rdg-cell[role='columnheader'] {
				padding: 0 ${theme.spaces.s}px;
				border-color: ${theme.palette['graphics-input-stroke']};
			}

			${setBorderBetweenRows &&
			css`
				& > .rdg-row > .rdg-cell {
					position: relative;
					border-bottom: none;
					border-top: none;

					&::after {
						${cellBottomBorderStyles(theme.palette['graphics-divider-2'])}
					}
				}

				& > .rdg-row > .rdg-cell:not([role='columnheader']):first-of-type {
					position: relative;
				}

				& > .rdg-row > .rdg-cell:not([role='columnheader']):last-of-type {
					position: relative;
					border-right: none;
				}
			`}
		`;

export const overrideGridStylesWithEditColumn =
	(
		isPaginationShown: boolean,
		columnIndex?: number,
		templateColumns?: string,
		editColumnWidth?: number,
		isClaimsTab?: boolean
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStyles(isPaginationShown, templateColumns)(theme).styles}

			& .rdg-cell.${EDIT_COLUMN_HEADER_CLASS}, 
      .rdg-cell.${EDIT_COLUMN_CELL_CLASS}, 
      .rdg-cell.${EDIT_COLUMN_SUMMARY_CLASS} {
				width: ${editColumnWidth || EDIT_COLUMN_WIDTH}px;
				right: -0.1px;
				position: sticky !important;
				box-shadow: none;
				grid-column: ${columnIndex} !important;
				padding: 0px ${theme.spaces.s}px;
				border-right: none;
				border-left: 1px solid ${theme.palette['graphics-divider-1']};
				display: flex;
				justify-content: center;

				button {
					height: ${SMALL_ICON_SIZE}px;
					width: ${SMALL_ICON_SIZE}px;
					justify-content: flex-start;
					display: flex;
					align-items: center;
					justify-content: center;
				}
			}

			& .rdg-cell.${RELATIVE_EDIT_COLUMN_CELL_CLASS} {
				grid-column: ${columnIndex} !important;
			}

			& .rdg-cell.${EDIT_COLUMN_HEADER_CLASS} {
				user-select: none;
				border: none;
				border-left: 1px solid ${theme.palette['graphics-input-stroke']};
			}

			&
				.rdg-cell.${RELATIVE_EDIT_COLUMN_CELL_CLASS}:not([role='columnheader']) {
				border-right: none;
			}

			::-webkit-scrollbar-track:vertical {
				margin-top: ${COMMON_GRID_PROPS.headerRowHeight}px;
				border-radius: ${BorderRadiuses.none}px;
				border-left: 1px solid ${theme.palette['graphics-divider-1']};
			}

			${isClaimsTab
				? `& .rdg-cell[aria-colindex='2'][role='gridcell'], & .rdg-cell[aria-colindex='3'][role='gridcell'] {
				background-color: ${NAME_CELL_BACKGROUND[theme.mode]};
			}`
				: ``}
		`;

export const DataGridHoverRowWrapper = styled('div')<{
	defineHeight?: boolean;
}>(({ theme, defineHeight = true }) => ({
	height: '100%',
	'.rdg-row>.rdg-cell:not([role="columnheader"])': {
		borderBottom: '1px solid transparent !important',
		borderTop: '1px solid transparent !important',
	},
	'.rdg-row:hover > .rdg-cell ': {
		backgroundColor: `${getHoverBackgroundColor(theme)} !important`,
		borderTop: `1px solid ${theme.palette['graphics-divider-1']} !important`,
		borderBottom: `1px solid ${theme.palette['graphics-divider-1']} !important`,
		':not([role="columnheader"]):after': {
			background: 'transparent',
		},
	},

	'& > div': {
		height: defineHeight
			? `calc(100% - ${DEFAULT_GRID_PAGINATION_HEIGHT}px)`
			: '100%',
	},
}));

const nullBordersStyles = {
	borderRight: 'none',
	borderLeft: 'none',
};

export const getDataGridResetStyles = (removeBorders?: boolean) => {
	const baseStyles = {
		borderRadius: 0,
		...(removeBorders ? nullBordersStyles : {}),
	};

	return {
		[`.${DATA_GRID_WRAPPER_CLASS_NAME}`]: {
			borderRadius: `0 0 ${BorderRadiuses.m}px ${BorderRadiuses.m}px;`,
			'&::after': baseStyles,
			'&::before': baseStyles,
			'div[role="grid"]': baseStyles,
		},
	};
};

export const overrideSectionTableStyles =
	(
		isPaginationShown: boolean,
		columnIndex?: number,
		templateColumns?: string,
		editColumnWidth?: number
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns,
				editColumnWidth
			)(theme)}

			.rdg-row {
				background-color: ${theme.palette['background-white']};
			}

			border-color: ${theme.palette['graphics-input-stroke']};
		`;
