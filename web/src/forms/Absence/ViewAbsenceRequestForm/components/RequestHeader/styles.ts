import styled from '@emotion/styled';

import { MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from '@/forms/Absence/ViewAbsenceRequestForm/constants';

export const RequestHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,

	[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
		flexDirection: 'column-reverse',
	},
}));

export const ControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.s,

	[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
		width: '100%',
	},
}));
