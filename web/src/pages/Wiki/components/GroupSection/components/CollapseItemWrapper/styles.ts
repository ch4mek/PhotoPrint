import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const CONTENT_WRAPPER_PADDING = 7;

export const CollapseItemContentWrapper = styled('div')<{
	hasTopLevelStyles: boolean;
}>(({ hasTopLevelStyles, theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	borderRadius: BorderRadiuses.xs2,

	...(hasTopLevelStyles && {
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		padding: CONTENT_WRAPPER_PADDING,
		borderRadius: BorderRadiuses.m,
	}),
}));
