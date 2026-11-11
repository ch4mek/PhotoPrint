import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import {
	PAYLOAD_BORDER_WIDTH,
	PROJECT_SUBROW_HOVERED_CLASS_NAME,
	WORKLOAD_SUBROW_HOVERED_CLASS_NAME,
	WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME,
} from '../../contants';
import { PayloadZIndexes } from '../WorkloadCell/constants';

import { EDIT_COLUMN_HEADER_CLASS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import {
	getHoverBorderColor,
	createDefaultBorderColor,
} from '@/helpers/getColor';

const LEFT_BORDER_BOX_SHADOW = '-2px 0 0 0';
const BOTTOM_BORDER_BOX_SHADOW = '0 2px 0 0';
const BOTTOM_PROJE_BORDER_BOX_SHADOW = '-2px 2px 0 0';
const NUMBER_OF_HOVER_COLUMNS = 14;
const FIRST_HOVER_COLUMN_INDEX = 6;

const getBoxShadowAsBorder = (boxShadow: string, theme: Theme) =>
	`${boxShadow} ${getHoverBorderColor(theme)}`;

const getColumnsHoverStyles = (theme: Theme) =>
	new Array(NUMBER_OF_HOVER_COLUMNS).fill(0).reduce(
		(hovers, _, columnIndex) => `
		${hovers}
		&:has([data-hovercolindex='${columnIndex + FIRST_HOVER_COLUMN_INDEX}']:hover) {
			.rdg-cell:has(+ .rdg-cell > [data-hovercolindex='${
				columnIndex + FIRST_HOVER_COLUMN_INDEX
			}']),
			.rdg-cell:has(> [data-hovercolindex='${
				columnIndex + FIRST_HOVER_COLUMN_INDEX
			}']) {
				&::before {
					content: '';
					pointer-events: none;
					width: ${PAYLOAD_BORDER_WIDTH}px !important;
					background-color: ${getHoverBorderColor(theme)} !important;
				}
			}
		}
	`
	);

export const PayloadGridColumnHoverWrapper = styled('div')(
	({ theme }) => css`
		.rdg-row:hover:has(.${WORKLOAD_SUBROW_HOVERED_CLASS_NAME})
			.rdg-cell:not([role='columnheader']),
		.rdg-row:has(
				+ .rdg-row:hover .rdg-cell div.${WORKLOAD_SUBROW_HOVERED_CLASS_NAME}
			)
			.rdg-cell:not([role='columnheader']) {
			&::after {
				content: '';
				background-color: ${createDefaultBorderColor(theme)};
			}
		}

		${getColumnsHoverStyles(theme)}
	`
);

export const Wrapper = styled('div')(
	({ theme }) => css`
		display: contents;
		height: 100%;

		& > div {
			display: contents;
			height: 100%;

			.${EDIT_COLUMN_HEADER_CLASS} {
				z-index: ${zIndexes.header + 1} !important;
			}

			.rdg-cell:is([role='columnheader']) {
				z-index: ${zIndexes.header};
				border: none;
			}

			.rdg-row:hover .rdg-cell:not([role='columnheader']),
			.rdg-row:has(+ .rdg-row:hover:not(.rdg-bottom-summary-row))
				.rdg-cell:not([role='columnheader']) {
				&::after {
					content: '';
					background-color: ${getHoverBorderColor(theme)};
					height: ${PAYLOAD_BORDER_WIDTH}px;
				}
			}

			div:has(+ .${PROJECT_SUBROW_HOVERED_CLASS_NAME}) {
				box-shadow: ${getBoxShadowAsBorder(BOTTOM_BORDER_BOX_SHADOW, theme)};
			}

			.${PROJECT_SUBROW_HOVERED_CLASS_NAME} {
				box-shadow: ${getBoxShadowAsBorder(LEFT_BORDER_BOX_SHADOW, theme)},
					${getBoxShadowAsBorder(BOTTOM_PROJE_BORDER_BOX_SHADOW, theme)};
				z-index: ${PayloadZIndexes.l};

				:last-child {
					box-shadow: ${getBoxShadowAsBorder(LEFT_BORDER_BOX_SHADOW, theme)};
				}

				:last-child::before {
					content: '';
					position: absolute;
					left: 0;
					bottom: 0;
					width: 100%;
					height: ${PAYLOAD_BORDER_WIDTH}px;
					background-color: ${getHoverBorderColor(theme)};
				}
			}

			div:has(+ .${WORKLOAD_SUBROW_HOVERED_CLASS_NAME}) {
				box-shadow: ${getBoxShadowAsBorder(BOTTOM_BORDER_BOX_SHADOW, theme)};
				position: relative;
				z-index: ${PayloadZIndexes.m};

				&:last-child {
					box-shadow: unset;
					::before {
						content: '';
						width: 100%;
						height: ${PAYLOAD_BORDER_WIDTH}px;
						position: absolute;
						bottom: 0;
					}
				}
			}

			.${WORKLOAD_SUBROW_HOVERED_CLASS_NAME} {
				box-shadow: ${getBoxShadowAsBorder(BOTTOM_BORDER_BOX_SHADOW, theme)};
				position: relative;
				z-index: ${PayloadZIndexes.s};

				:last-child {
					box-shadow: unset;
				}

				:last-child::before {
					content: '';
					position: absolute;
					left: 0;
					bottom: 0;
					width: 100%;
					height: ${PAYLOAD_BORDER_WIDTH}px;
					background-color: ${getHoverBorderColor(theme)};
				}
			}

			.rdg-cell:not(:has([data-activemonth='true']))
				.${WORKLOAD_SUBROW_HOVERED_CLASS_NAME} {
				&.${WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME} {
					:last-child {
						box-shadow: inset 0px -${PAYLOAD_BORDER_WIDTH}px ${getHoverBorderColor(theme)};
					}

					z-index: ${PayloadZIndexes.xl};
					box-shadow: 0px ${PAYLOAD_BORDER_WIDTH}px
						${getHoverBorderColor(theme)};
					::before {
						content: '';
						top: 0;
						position: absolute;
						width: 100%;
						height: ${PAYLOAD_BORDER_WIDTH}px;
						background-color: ${getHoverBorderColor(theme)};
					}

					::after {
						content: '';
						position: absolute;
						height: 100%;
						right: 0;
						width: ${PAYLOAD_BORDER_WIDTH}px;
						background-color: ${getHoverBorderColor(theme)};
					}
				}
			}
		}
	`
);
