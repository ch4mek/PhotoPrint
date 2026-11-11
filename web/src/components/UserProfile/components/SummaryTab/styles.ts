import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';

import { EDIT_COLUMN_CELL_CLASS } from '@/components/Grid/constants';

export const SummaryTabWrapper = styled('div')(
	({ theme }) => css`
		width: 100%;
		height: 100%;
		display: grid;
		grid-template-columns: repeat(2, minmax(0, 1fr));
		gap: ${theme.spaces.m}px;

		${theme.breakpoints.down(theme.breakpoints.desktopXS)} {
			display: flex;
			flex-direction: column;
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

export const SummaryTabColumn = styled('div')(
	({ theme }) => css`
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.m}px;
	`
);
