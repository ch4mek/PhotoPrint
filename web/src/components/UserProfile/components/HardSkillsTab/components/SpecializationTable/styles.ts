import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';
import {
	getEvenRowBackgroundColor,
	getOddRowBackgroundColor,
} from '@/helpers/getColor';

import {
	EDITABLE_COLUMN_CLASS_NAME,
	EVEN_SPECIALIZATION_ROW_CLASS_NAME,
	FILLER_CELL_CLASS_NAME,
	HARD_SKILLS_CELL_CLASS_NAME,
	ODD_SPECIALIZATION_ROW_CLASS_NAME,
} from './constants';

export const SpecializationTableHoverWrapper = styled(DataGridHoverRowWrapper)(`
	flex-grow: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;

	& > div {
		height: 100%;

		& > div {
			&
				> .rdg-row:not(:hover):has(
					+ .rdg-row > .rdg-cell.${HARD_SKILLS_CELL_CLASS_NAME}
				)
				> .rdg-cell::after {
					content: none;
				}

			& > .rdg-row:hover > .rdg-cell.${HARD_SKILLS_CELL_CLASS_NAME} {
				::after {
					content: none;
				}
			}
		}
	}
`);

export const overrideSpecializationsStyles =
	(
		templateColumns: string,
		editColumnIndex: number,
		isPaginationShown: boolean
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				editColumnIndex,
				templateColumns
			)(theme).styles};

			.rdg-cell[role='columnheader'].${EDITABLE_COLUMN_CLASS_NAME} > div {
				justify-content: space-between;
			}

			.rdg-row {
				&.${ODD_SPECIALIZATION_ROW_CLASS_NAME} {
					background-color: ${getOddRowBackgroundColor(theme)};
				}

				&.${EVEN_SPECIALIZATION_ROW_CLASS_NAME} {
					background-color: ${getEvenRowBackgroundColor(theme)};
				}
			}

			.${HARD_SKILLS_CELL_CLASS_NAME} {
				padding: 0;

				& > div {
					height: 100%;
				}

				div:has(> .rdg) {
					border-radius: 0;
					border: none;
				}

				div.rdg-cell {
					&:not([role='columnheader']):last-of-type {
						border-right: 1px solid ${theme.palette['graphics-divider-1']};
					}
				}
			}

			&
				> .rdg-row
				> .rdg-cell.${FILLER_CELL_CLASS_NAME},
				&
				> .rdg-row
				> .rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
				::after {
					content: none;
				}

				pointer-events: none;

				background: none;
			}

			& > .rdg-row > .rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
				border-left: none;
				border-right: none;
			}
		`;
