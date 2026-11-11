import styled from '@emotion/styled';

import { MOBILE_VIEW_REQUEST_FORM_BREAKPOINT } from '@/forms/Absence/ViewAbsenceRequestForm/constants';

export const RequestCalendarWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.s,
}));

export const CalendarRangeWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,

	[theme.breakpoints.down(MOBILE_VIEW_REQUEST_FORM_BREAKPOINT)]: {
		flexDirection: 'column',
	},
}));
