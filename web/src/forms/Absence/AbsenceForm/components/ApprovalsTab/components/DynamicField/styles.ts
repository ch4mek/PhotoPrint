import styled from '@emotion/styled';

import { ICON_SIZE, MOBILE_L_BREAKPOINT } from '../../constants';

export const DynamicFieldWrapper = styled('div')<{
	isShowIcon: boolean;
	isShowUserField: boolean;
}>(({ theme, isShowIcon, isShowUserField }) => ({
	flex: 1,
	flexGrow: isShowUserField ? 1 : 0.5,
	display: 'grid',
	gap: theme.spaces.s,
	gridTemplateColumns: `1fr ${isShowUserField ? '1fr' : ''} ${
		isShowIcon ? `${ICON_SIZE}px` : ''
	}`,

	[theme.breakpoints.down(MOBILE_L_BREAKPOINT)]: {
		flexGrow: 1,
		gridTemplateColumns: `1fr`,
		gap: theme.spaces.l,
	},
}));
