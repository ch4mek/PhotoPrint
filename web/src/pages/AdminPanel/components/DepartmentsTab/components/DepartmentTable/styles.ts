import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';

import { overrideGridStyles } from '@/components/Grid';

import { GROUP_SUBROW_HOVERED_CLASS_NAME } from './constants';

export const overrideDepartmetsTabStyles =
	(gridColumnTemplate: string) => (theme: Theme) =>
		css`
			${overrideGridStyles(false, gridColumnTemplate)(theme).styles};

			.rdg-cell {
				padding: 0;
			}
		`;

export const DataDepartmentGridHoverRowWrapper = styled('div')(
	({ theme }) => `
	height: 100%;

	& .rdg-cell .${GROUP_SUBROW_HOVERED_CLASS_NAME} {
		background: ${theme.palette['background-base-2']};
	}

	.${DATA_GRID_WRAPPER_CLASS_NAME} {
		height: 100%;
		max-height: 100%;
	}
`
);
