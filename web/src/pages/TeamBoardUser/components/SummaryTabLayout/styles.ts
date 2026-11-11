import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';

import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';

import { GRID_AREA_NAMES } from './constants';

export const Wrapper = styled('div')(
	({ theme }) => css`
		width: 100%;
		height: 100%;
		display: grid;
		grid-template-columns: repeat(2, minmax(0, 1fr));
		grid-template-rows: min-content 1fr;
		grid-template-areas: '${GRID_AREA_NAMES.firstGroup} ${GRID_AREA_NAMES.secondGroup}' '${GRID_AREA_NAMES.thirdGroup} ${GRID_AREA_NAMES.secondGroup}';
		gap: ${theme.spaces.m}px;
		margin-top: ${theme.spaces.s}px;

		${theme.breakpoints.down(theme.breakpoints.desktopXS)} {
			grid-template-areas: '${GRID_AREA_NAMES.firstGroup}' '${GRID_AREA_NAMES.secondGroup}' '${GRID_AREA_NAMES.thirdGroup}';
			grid-template-columns: minmax(0, 1fr);
			grid-template-rows: repeat(1, minmax(0, 1fr));
		}

		.${DATA_GRID_WRAPPER_CLASS_NAME} {
			.rdg-cell.${EDIT_COLUMN_CELL_CLASS} {
				border-right: 1px;
			}

			&::after,
			&::before {
				border-radius: 0;
			}
		}
	`
);

export const Column = styled('div')<{ orderName?: string }>(
	({ orderName, theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.m}px;
		${orderName ? `grid-area: ${orderName}` : ''};
	`
);
