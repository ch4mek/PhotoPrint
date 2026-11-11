import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';
import {
	EDITABLE_COLUMN_CLASS_NAME,
	FILLER_CELL_CLASS_NAME,
	SOFT_SKILLS_CELL_CLASS_NAME,
} from '@/pages/TeamBoardUser/components/SoftSkillsTab/components/SoftSkillsTable/constants';

export const SoftSillsTableHoverWrapper = styled(DataGridHoverRowWrapper)(
	() => css`
		flex-grow: 1;
		min-height: 0;
		display: flex;
		flex-direction: column;
		& > div {
			height: 100%;

			& > div {
				&
					> .rdg-row:not(:hover):has(
						+ .rdg-row > .rdg-cell.${SOFT_SKILLS_CELL_CLASS_NAME}
					)
					> .rdg-cell::after {
					content: none;
				}

				& > .rdg-row:hover > .rdg-cell.${SOFT_SKILLS_CELL_CLASS_NAME} {
					::after {
						content: none;
					}
				}
			}
		}
	`
);

export const overrideSoftSkillsStyles =
	(
		templateColumns: string,
		isPaginationShow: boolean,
		editColumnIndex: number
	) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShow,
				editColumnIndex,
				templateColumns
			)(theme).styles};

			.rdg-cell[role='columnheader'].${EDITABLE_COLUMN_CLASS_NAME} > div {
				justify-content: space-between;
			}

			.${SOFT_SKILLS_CELL_CLASS_NAME} {
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
				pointer-events: none;
				border-left: none;
				border-right: none;
			}
		`;
