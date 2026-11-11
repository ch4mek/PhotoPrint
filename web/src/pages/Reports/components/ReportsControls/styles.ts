import styled from '@emotion/styled';

import { CONTROLLERS_BREAKPOINT } from '../../constants';

export const ControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'flex-start',
	justifyContent: 'flex-end',
	gap: theme.spaces.s,
	marginTop: 0,

	[theme.breakpoints.up(CONTROLLERS_BREAKPOINT)]: {
		justifyContent: 'space-between',
	},
}));
