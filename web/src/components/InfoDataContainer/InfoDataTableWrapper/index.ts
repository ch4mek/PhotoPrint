import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { getDataGridResetStyles } from '@/components/Grid/styles';
import { BorderRadiuses } from '@/constants/borderRadiuses';

export const InfoDataTableWrapper = styled('div')(
	({ theme }) => css`
		& > div > div {
			width: 100%;
			border-radius: 0px 0px ${BorderRadiuses.m}px ${BorderRadiuses.m}px;
			border-color: ${theme.palette['graphics-input-stroke']};
		}

		& > div:has(div[role='grid']) {
			border-radius: 0px 0px ${BorderRadiuses.m}px ${BorderRadiuses.m}px;
		}

		${getDataGridResetStyles()}
	`
);
