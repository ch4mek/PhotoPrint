import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	IGNORE_SELECTION_CLASS_NAME,
} from '@/components/Grid/constants';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';
import {
	createActiveBorderColor,
	createDefaultBorderColor,
	getHoverBackgroundColor,
} from '@/helpers/getColor';

import {
	CLIENT_EDIT_COLUMN_INDEX,
	CLIENT_PROJECTS_ROW_CLASS_NAME,
	CLIENT_ROW_NAME_CLASS_NAME,
	clientColumnKeys,
	clientColumnsClassNames,
	HEAD_CLIENT_EDIT_COLUMN_INDEX,
} from './constants';

const nameColumnClassName = clientColumnsClassNames[clientColumnKeys.name];
const factTimeColumnClassName =
	clientColumnsClassNames[clientColumnKeys.factTime];
const billableTimeColumnClassName =
	clientColumnsClassNames[clientColumnKeys.billableTime];

export const clientsGridStyles =
	(
		isPaginationShown: boolean,
		displayTrackedColumn: boolean,
		templateColumns: string
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				displayTrackedColumn
					? CLIENT_EDIT_COLUMN_INDEX
					: HEAD_CLIENT_EDIT_COLUMN_INDEX,
				templateColumns
			)(theme)}

			.rdg-cell:has(+ .rdg-cell[aria-colspan='4']) {
				background-color: ${theme.palette['background-white']};
			}

			.rdg-cell.${CLIENT_ROW_NAME_CLASS_NAME} > div {
				width: 100%;
				height: 100%;

				div[role='grid'] {
					overflow-x: hidden;
				}

				button,
				button > svg {
					height: ${SMALL_ICON_SIZE}px;
					width: ${SMALL_ICON_SIZE}px;
				}
			}

			.rdg-cell.${CLIENT_PROJECTS_ROW_CLASS_NAME} {
				padding: 0;

				& > div {
					width: 100%;
				}

				.rdg-cell[role='gridcell']:last-of-type {
					border-right: none;
				}
			}

			div[aria-rowindex='1'] {
				margin-top: 100px;
			}
		`;

export const ClientGridRowHoverWrapper = styled('div')(
	({ theme }) => css`
		min-height: 0;
		height: 100%;
		& > div {
			height: 100%;

			& > div {
				.rdg-row:hover > .rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
					button {
						display: block;
					}
				}

				& > .rdg-row:hover > .rdg-cell:not(.${IGNORE_SELECTION_CLASS_NAME}) {
					background-color: ${getHoverBackgroundColor(theme)};
				}

				&
					> .rdg-row:hover
					> .rdg-cell:not([role='columnheader']):not(
						.${IGNORE_SELECTION_CLASS_NAME}
					) {
					&::after {
						background: ${createActiveBorderColor(theme)};
					}
				}

				&
					> .rdg-row:has(+ .rdg-row > .rdg-cell:hover)
					> .rdg-cell:not([role='columnheader']) {
					&:after {
						background: ${createActiveBorderColor(theme)};
					}
				}

				&
					> .rdg-row:has(+ .rdg-row .rdg-row:hover)
					.rdg-cell:not([role='columnheader']) {
					&:after {
						background: ${createDefaultBorderColor(theme)};
					}
				}

				.rdg-row:has(+ .rdg-row ${`.${IGNORE_SELECTION_CLASS_NAME}:hover`})
					.rdg-cell:not([role='columnheader']) {
					&:after {
						background: ${createDefaultBorderColor(theme)};
					}
				}

				.rdg-row:has(+ .rdg-row .rdg-row:nth-of-type(2):hover)
					.rdg-cell:not([role='columnheader']) {
					&:after {
						background: ${createDefaultBorderColor(theme)};
					}
					&:is(
							.${nameColumnClassName},
								.${factTimeColumnClassName},
								.${billableTimeColumnClassName}
						):after {
						background: ${createActiveBorderColor(theme)};
					}
				}

				.${`${CLIENT_PROJECTS_ROW_CLASS_NAME}`} {
					&:has(.rdg-row:hover):not(:has(.rdg-row:hover + .rdg-row)):after {
						background: ${createActiveBorderColor(theme)};
					}
				}
			}
		}
	`
);

export const ClientHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	justifyContent: 'space-between',
	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		gap: theme.spaces.m,
	},
}));

export const PageHeaderWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	flexGrow: 1,
});
