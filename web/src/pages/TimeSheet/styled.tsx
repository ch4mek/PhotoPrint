import styled from '@emotion/styled';

import { TimeSheetBreakpoints } from './constants';
import { TimesheetHeaderWrapperProps } from './types';

const { DESKTOP_MD } = TimeSheetBreakpoints;

export const TimesheetHeaderWrapper = styled(
	'div'
)<TimesheetHeaderWrapperProps>(
	({
		theme,
		shouldGetTimesheetStatus,
		isMonthFilter,
		isEmployeeOrSysAdminRole,
	}) => ({
		display: 'flex',
		flexDirection: 'column',
		position: 'relative',
		...((shouldGetTimesheetStatus && isMonthFilter) || isEmployeeOrSysAdminRole
			? {
					marginBottom: theme.spaces.m,
			  }
			: {
					[theme.breakpoints.up(DESKTOP_MD)]: {
						marginBottom: theme.spaces.m,
					},
			  }),
	})
);

export const TimesheetPageWrapper = styled('div')({
	position: 'relative',
	height: '100%',
	display: 'flex',
	flexDirection: 'column',
});
