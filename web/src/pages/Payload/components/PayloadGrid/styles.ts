import { css, Theme } from '@emotion/react';

import { PayloadZIndexes } from '../WorkloadCell/constants';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
	EDIT_COLUMN_SUMMARY_CLASS,
} from '@/components/Grid/constants';
import { BASE_LINE_HEIGHT } from '@/constants/commonStyles';
import { zIndexes } from '@/constants/zIndexes';
import {
	PAYLOAD_BORDER_WIDTH,
	PAYLOAD_FREEZE_BREAKPOINT,
	SUMMARY_ROW_BOX_SHADOW_HEIGHT,
	SUMMARY_ROW_HEIGHT,
} from '@/pages/Payload/contants';

export const overridePayloadGridStyles =
	(
		isPaginationShown: boolean,
		columnIndex: number,
		templateColumns: string,
		editColumnWidth?: number,
		freezeColumnWidth?: number[]
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				columnIndex,
				templateColumns,
				editColumnWidth
			)(theme).styles}

			${theme.breakpoints.up(PAYLOAD_FREEZE_BREAKPOINT)} {
				${freezeColumnWidth?.map((_, index) => {
					const sum = freezeColumnWidth
						.slice(0, index)
						.reduce((acc, curr) => acc + curr, 0);

					return `
	
	.rdg-cell[aria-colindex='${index + 2}'][role='columnheader'] {
	z-index: ${zIndexes.header + 1};
	}

	.rdg-summary-row > .rdg-cell[aria-colindex='${index + 2}'] {
	z-index: ${zIndexes.summaryRow + 1};
	}
	
	.rdg-cell[aria-colindex='${index + 2}'] {
			position: sticky !important;
			z-index: ${PayloadZIndexes.ten};
			left: ${sum}px;
			}`;
				})}

				::-webkit-scrollbar-track-piece:horizontal {
					margin-left: ${freezeColumnWidth?.reduce(
						(acc, curr) => acc + curr,
						0
					)}px;
				}
			}

			.rdg-summary-row > .rdg-cell {
				position: sticky !important;
				z-index: ${zIndexes.summaryRow};
				background-color: ${theme.palette['background-base-2']};

				box-shadow: 0 ${SUMMARY_ROW_BOX_SHADOW_HEIGHT}px 0 0
					${theme.palette['background-base-2']};
			}

			.rdg-summary-row > .${EDIT_COLUMN_SUMMARY_CLASS} {
				z-index: ${zIndexes.summaryRow};
				border-left: transparent;
				position: sticky !important;
			}

			.rdg-summary-row > div[role='gridcell']:not(:has(> *)) {
				border-right-color: transparent !important;
			}

			& > .rdg-row:is(.rdg-summary-row):hover > .rdg-cell {
				&:after {
					background: none !important;
					box-shadow: none !important;
				}

				background-color: ${theme.palette['background-base-2']};
			}

			.rdg-cell[role='columnheader']:has(div[data-activemonth='true']) {
				background-color: ${theme.palette['background-white']};
			}

			.rdg-cell[role='columnheader'] div {
				background-color: transparent;
			}

			.rdg-cell[role='gridcell'] {
				line-height: ${BASE_LINE_HEIGHT};
				display: flex;
				align-items: flex-start;
				padding: 0;
				overflow: initial;
				justify-content: center;
				border-inline-end: none;
				&::before {
					content: '';
					position: absolute;
					width: 1px;
					height: 100%;
					top: 0;
					right: 0;
					z-index: ${PayloadZIndexes.l};
					background: ${theme.palette['graphics-divider-1']};
				}
			}

			.${EDIT_COLUMN_HEADER_CLASS} {
				&::before {
					left: 0;
					right: unset;
				}
			}

			.rdg-cell[role='columnheader'] {
				&::before {
					content: '';
					position: absolute;
					width: 1px;
					height: 100%;
					top: 0;
					right: 0;
					z-index: ${PayloadZIndexes.l};
					background: ${theme.palette['graphics-input-stroke']};
				}
			}

			.rdg-cell[role='gridcell']:not([role='columnheader']):is(
					.${EDIT_COLUMN_CELL_CLASS}
				) {
				border-right: none;
				position: relative !important;
			}

			.${EDIT_COLUMN_CELL_CLASS} {
				z-index: ${zIndexes.sidebar};
				button {
					margin-top: ${theme.spaces.xs}px;
				}
			}

			.${EDIT_COLUMN_HEADER_CLASS} {
				z-index: ${zIndexes.sidebar + 1};
			}

			.rdg-cell:not(.${EDIT_COLUMN_CELL_CLASS}):not(
					.${EDIT_COLUMN_HEADER_CLASS}
				):not(.${EDIT_COLUMN_SUMMARY_CLASS}):has(
					+ .rdg-cell div[data-activemonth='true']
				),
			.rdg-cell:has(div[data-activemonth='true']) {
				&::before {
					content: '';
					position: absolute;
					width: ${PAYLOAD_BORDER_WIDTH}px !important;
					height: 100%;
					top: 0;
					right: 0;
					z-index: ${PayloadZIndexes.l};
					background: ${theme.palette['graphics-accent']};
				}
			}

			& > .rdg-row:hover .rdg-cell {
				background-color: inherit;
			}

			::-webkit-scrollbar-track:vertical {
				margin-bottom: ${SUMMARY_ROW_HEIGHT}px;
			}

			::-webkit-scrollbar:vertical {
				box-shadow: inset 0 -${SUMMARY_ROW_HEIGHT}px 0 0 ${theme.palette['background-base-2']};
			}
		`;
