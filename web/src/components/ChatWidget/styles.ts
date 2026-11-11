import { Theme } from '@emotion/react';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const getChatWidgetPopoverStyles = (
	theme: Theme,
	isMobileView: boolean
) => ({
	background: theme.palette['background-white'],
	width: '100%',

	...(isMobileView && {
		borderRadius: BorderRadiuses.m,
		border: `1px solid ${theme.palette['graphics-divider-1']}`,
		boxShadow: theme.namedShadows.dropDownListWrapper,
	}),
});
