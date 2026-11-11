import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { FormWrapper } from '@/forms/styles';

export const DevelopmentPlanFormWrapper = styled(FormWrapper)(
	() => css`
		width: 100%;
	`
);

export const DatesWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		column-gap: ${theme.spaces.s}px;
		row-gap: ${theme.spaces.l}px;
		flex-wrap: wrap;
	`
);
